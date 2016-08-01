#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;
using PokemonGo.RocketAPI.Extensions;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using System.Data.SQLite;
using System.Globalization;
using POGOProtos.Enums;

#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public static class FarmPokestopsTask
    {
        public static int TimesZeroXPawarded;
        private static int storeRI;
        public static async Task Execute(ISession session, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var distanceFromStart = LocationUtils.CalculateDistanceInMeters(
                session.Settings.DefaultLatitude, session.Settings.DefaultLongitude,
                session.Client.CurrentLatitude, session.Client.CurrentLongitude);

            // Edge case for when the client somehow ends up outside the defined radius
            if (session.LogicSettings.MaxTravelDistanceInMeters != 0 &&
                distanceFromStart > session.LogicSettings.MaxTravelDistanceInMeters)
            {
                Logger.Write(
                    session.Translation.GetTranslation(TranslationString.FarmPokestopsOutsideRadius, distanceFromStart),
                    LogLevel.Warning);
                
                await session.Navigation.Move(
                    new GeoCoordinate(session.Settings.DefaultLatitude, session.Settings.DefaultLongitude),
                    session.LogicSettings.WalkingSpeedInKilometerPerHour, null, cancellationToken, session.LogicSettings.DisableHumanWalking);
            }

            var pokestopList = await GetPokeStops(session);

            var stopsHit = 0;
            var rc = new Random(); //initialize pokestop random cleanup counter first time
            storeRI = rc.Next(3, 9);
            var eggWalker = new EggWalker(1000, session);

            if (pokestopList.Count <= 0)
            {
                await resetLocation(session, cancellationToken);
                pokestopList = await GetPokeStops(session);
            }

            session.EventDispatcher.Send(new PokeStopListEvent { Forts = pokestopList });

            HashSet<string> hsVisitedStops = new HashSet<string>();

            while (true)
            {
                if (GlobalSettings.blCriticalBall == false && blWentAnyLoc == true)
                    break;

                cancellationToken.ThrowIfCancellationRequested();

                //resort
                pokestopList =
                    pokestopList.Where(i => hsVisitedStops.Contains(i.Id) == false).OrderBy(
                        i =>
                            LocationUtils.CalculateDistanceInMeters(session.Client.CurrentLatitude,
                                session.Client.CurrentLongitude, i.Latitude, i.Longitude)).Distinct().ToList();

                if (pokestopList.Count == 0)
                    break;

                Logging.Logger.Write("Current PokeStop Count to visit: " + pokestopList.Count, LogLevel.Self, ConsoleColor.DarkYellow);

                var pokeStop = pokestopList[0];

                hsVisitedStops.Add(pokeStop.Id);

                var distance = LocationUtils.CalculateDistanceInMeters(session.Client.CurrentLatitude,
                    session.Client.CurrentLongitude, pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await session.Client.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);

                session.EventDispatcher.Send(new FortTargetEvent { Name = fortInfo.Name, Distance = distance });

                    await session.Navigation.Move(new GeoCoordinate(pokeStop.Latitude, pokeStop.Longitude),
                    session.LogicSettings.WalkingSpeedInKilometerPerHour,
                    async () =>
                    {
                        // Catch normal map Pokemon
                        await CatchNearbyPokemonsTask.Execute(session, cancellationToken);
                        //Catch Incense Pokemon
                        await CatchIncensePokemonsTask.Execute(session, cancellationToken);
                        return true;
                    }, cancellationToken, session.LogicSettings.DisableHumanWalking);

                //Catch Lure Pokemon
                if (pokeStop.LureInfo != null)
                {
                    await CatchLurePokemonsTask.Execute(session, pokeStop, cancellationToken);
                }

                FortSearchResponse fortSearch;
                var timesZeroXPawarded = 0;
                var fortTry = 0; //Current check
                const int retryNumber = 50; //How many times it needs to check to clear softban
                const int zeroCheck = 5; //How many times it checks fort before it thinks it's softban
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    fortSearch =
                        await session.Client.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                    if (fortSearch.ExperienceAwarded > 0 && timesZeroXPawarded > 0) timesZeroXPawarded = 0;
                    if (fortSearch.ExperienceAwarded == 0)
                    {
                        await RecycleItemsTask.Execute(session, cancellationToken);

                        timesZeroXPawarded++;

                        if (timesZeroXPawarded > zeroCheck)
                        {
                            if ((int)fortSearch.CooldownCompleteTimestampMs != 0)
                            {
                                break;
                                // Check if successfully looted, if so program can continue as this was "false alarm".
                            }

                            fortTry += 1;

                            session.EventDispatcher.Send(new FortFailedEvent
                            {
                                Name = fortInfo.Name,
                                Try = fortTry,
                                Max = retryNumber - zeroCheck
                            });

                            DelayingUtils.Delay(session.LogicSettings.DelayBetweenPlayerActions, 400);
                        }
                    }
                    else
                    {
                        session.EventDispatcher.Send(new FortUsedEvent
                        {
                            Id = pokeStop.Id,
                            Name = fortInfo.Name,
                            Exp = fortSearch.ExperienceAwarded,
                            Gems = fortSearch.GemsAwarded,
                            Items = StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded),
                            Latitude = pokeStop.Latitude,
                            Longitude = pokeStop.Longitude,
                            InventoryFull = fortSearch.Result == FortSearchResponse.Types.Result.InventoryFull
                        });

                        break; //Continue with program as loot was succesfull.
                    }
                } while (fortTry < retryNumber - zeroCheck);
                //Stop trying if softban is cleaned earlier or if 40 times fort looting failed.

                await eggWalker.ApplyDistance(distance, cancellationToken);

                if (++stopsHit >= storeRI) //TODO: OR item/pokemon bag is full //check stopsHit against storeRI random without dividing.
                {
                    storeRI = rc.Next(2, 8); //set new storeRI for new random value
                    stopsHit = 0;
                    if (fortSearch.ItemsAwarded.Count > 0)
                    {
                        await session.Inventory.RefreshCachedInventory();
                    }

                    await RecycleItemsTask.Execute(session, cancellationToken);

                    if (session.LogicSettings.EvolveAllPokemonWithEnoughCandy ||
                        session.LogicSettings.EvolveAllPokemonAboveIv)
                    {
                        await EvolvePokemonTask.Execute(session, cancellationToken);
                    }
                    await GetPokeDexCount.Execute(session, cancellationToken);

                    if (session.LogicSettings.AutomaticallyLevelUpPokemon)
                    {
                        await LevelUpPokemonTask.Execute(session, cancellationToken);
                    }

                    if (session.LogicSettings.UseLuckyEggConstantly)
                    {
                        await UseLuckyEggConstantlyTask.Execute(session, cancellationToken);
                    }
                    if (session.LogicSettings.UseIncenseConstantly)
                    {
                        await UseIncenseConstantlyTask.Execute(session, cancellationToken);
                    }

                    if (session.LogicSettings.TransferDuplicatePokemon)
                    {
                        await TransferDuplicatePokemonTask.Execute(session, cancellationToken);
                    }

                    if (session.LogicSettings.EvolveAllPokemonWithEnoughCandy || session.LogicSettings.EvolveAllPokemonAboveIv)
                    {
                        await EvolvePokemonTask.Execute(session, cancellationToken);
                    }

                    if (session.LogicSettings.RenamePokemon)
                    {
                        await RenamePokemonTask.Execute(session, cancellationToken);
                    }

                    if (session.LogicSettings.AutoFavoritePokemon)
                    {
                        await FavoritePokemonTask.Execute(session, cancellationToken);
                    }
                }

                if (session.LogicSettings.SnipeAtPokestops || session.LogicSettings.UseSnipeLocationServer)
                {
                    await SnipePokemonTask.Execute(session, cancellationToken);
                }

                pokestopList.AddRange(await GetPokeStops(session));
            }
        }

        private static async Task<List<FortData>> GetPokeStops(ISession session)
        {
            var mapObjects = await session.Navigation.GetMapObjects();

            var pokeStops2 = mapObjects.MapCells.SelectMany(i => i.Forts).ToList();

            Logging.Logger.Write("found total PokeStops Count: " + pokeStops2.Count+" current location "+session.Client.CurrentLatitude +" , "+
                session.Client.CurrentLongitude
                , LogLevel.Self, ConsoleColor.DarkCyan);

            // Wasn't sure how to make this pretty. Edit as needed.
            var pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts)
                .Where(
                    i =>
                        i.Type == FortType.Checkpoint &&
                        i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime() &&
                        ( // Make sure PokeStop is within max travel distance, unless it's set to 0.
                            LocationUtils.CalculateDistanceInMeters(
                                session.Settings.DefaultLatitude, session.Settings.DefaultLongitude,
                                i.Latitude, i.Longitude) < session.LogicSettings.MaxTravelDistanceInMeters ||
                        session.LogicSettings.MaxTravelDistanceInMeters == 0) 
                );

            pokeStops = pokeStops.ToList();

            Logging.Logger.Write("found useable PokeStops Count: " + pokeStops2.Count, LogLevel.Self, ConsoleColor.Cyan);

            return pokeStops.ToList();
        }

        private static List<string> funcReturnPokeLoc()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");
            List<string> lstPokeInfo = new List<string>();
            SQLiteConnection m_dbConnection = null;
            try
            {
                m_dbConnection =
       new SQLiteConnection(@"Data Source=" + GlobalSettings.srPokemonGoMap_Directory + "pogom.db;Version=3;");
                m_dbConnection.Open();
            }
            catch (Exception E)
            {
                Logger.Write("Error Database " + E.Message?.ToString(), LogLevel.Self, ConsoleColor.Yellow);
                Logger.Write("Error " + E.InnerException?.Message?.ToString(), LogLevel.Self, ConsoleColor.Yellow);
                return lstPokeInfo;
            }

            string sql = "select * from pokemon where disappear_time > '" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                //if (Client.hsVisitedPokeSpawnIds.Contains(reader["spawn_id"]))
                //    continue;
                string srResult = reader["latitude"] + ";" + reader["longitude"] + ";" + reader["encounter_id"] + ";" + reader["disappear_time"] + ";" + reader["pokemon_id"];
                lstPokeInfo.Add(srResult);
            }
            return lstPokeInfo;
        }

        private static HashSet<string> hsGonaLocations = new HashSet<string>();

        private static bool blWentAnyLoc = false;

        public static async Task ExeCuteMyFarm(ISession session, CancellationToken cancellationToken)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");

            var vrList = funcReturnPokeLoc();

            Logger.Write("Location found count " + vrList.Count, LogLevel.Self, ConsoleColor.DarkGray);
            int irLoop = 1;
            double dblMinDistance = 9999999;
            double dblMinDistLat = 0;
            double dblMinDistLng = 0;
            string srMinDistLoc = "na";
            double dblRareIndex = 999;
            int irRarePokeId = 0;


            for (int i = 0; i < vrList.Count; i++)
            {
                foreach (var vrloc in vrList)
                {
                    if (hsGonaLocations.Contains(vrloc))
                        continue;

                    List<string> lstData = vrloc.Split(';').ToList();

                    if (DateTime.UtcNow.AddSeconds(-5) > Convert.ToDateTime(lstData[3]))
                        continue;

                    double dblLat;
                    double dblLong;

                    double.TryParse(vrloc.Split(';')[0].Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out dblLat);
                    double.TryParse(vrloc.Split(';')[1].Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out dblLong);


                    if (dblLat == 0 || dblLong == 0)
                    {
                        continue;
                    }

                    int irPokemonId = Convert.ToInt32(lstData[4]);
                    int irThisRareIndex = 999;
                    if (GlobalSettings.lstPriorityPokemon.Contains(irPokemonId) == true)
                    {
                        irThisRareIndex = GlobalSettings.lstPriorityPokemon.IndexOf(irPokemonId);
                    }

                    var sourceLocation = new GeoCoordinate(session.Client.CurrentLatitude, session.Client.CurrentLongitude);
                    var targetLocation = new GeoCoordinate(dblLat, dblLong);
                    var distance = LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation);

                    if (distance < dblMinDistance && dblRareIndex >= irThisRareIndex)
                    {
                        dblMinDistance = distance;
                        dblMinDistLat = dblLat;
                        dblMinDistLng = dblLong;
                        srMinDistLoc = vrloc;
                        irRarePokeId = irPokemonId;
                    }
                    if (dblRareIndex > irThisRareIndex && GlobalSettings.blEnableRareHunt == true)
                    {
                        dblMinDistance = distance;
                        dblMinDistLat = dblLat;
                        dblMinDistLng = dblLong;
                        srMinDistLoc = vrloc;
                        dblRareIndex = irThisRareIndex;
                        irRarePokeId = irPokemonId;
                    }
                }

                if (dblMinDistLat != 0 && dblMinDistLng != 0)
                {
                    blWentAnyLoc = true;
                    Logger.Write("Target Poke Loop " + irLoop + " | Target Poke " + (PokemonId)irRarePokeId + " (" + irRarePokeId + ") | Target: " + srMinDistLoc, LogLevel.Self, ConsoleColor.DarkGray);
                    if (dblRareIndex != 999)
                        Logger.Write("Going for rare Index " + dblRareIndex + "| Target Poke " + (PokemonId)irRarePokeId + " (" + irRarePokeId + ") | Target: " + srMinDistLoc, LogLevel.Self, ConsoleColor.DarkMagenta);

                    hsGonaLocations.Add(srMinDistLoc);

                    await session.Navigation.Move(new GeoCoordinate(dblMinDistLat, dblMinDistLng),
                        session.LogicSettings.WalkingSpeedInKilometerPerHour,
                        async () =>
                        {
                            // Catch normal map Pokemon
                            await CatchNearbyPokemonsTask.Execute(session, cancellationToken);
                            //Catch Incense Pokemon
                            await CatchIncensePokemonsTask.Execute(session, cancellationToken);
                            return true;
                        }, cancellationToken);

                    irLoop++;
                    dblMinDistance = 9999999;
                    dblMinDistLat = 0;
                    dblMinDistLng = 0;

                    if (GlobalSettings.blCriticalBall == true)
                    {
                        Logger.Write("Critical BALL check...", LogLevel.Self, ConsoleColor.Yellow);
                        await Execute(session, cancellationToken);
                    }
                }
            }

            if (blWentAnyLoc == false)
            {
                Logger.Write("No location found make sure that poke miner is running!", LogLevel.Self, ConsoleColor.Yellow);
                await Execute(session, cancellationToken);
            }
        }

        private static async Task resetLocation(ISession session, CancellationToken cancellationToken)
        {
            if (GlobalSettings.lstPokeStopLocations.Count < 1)
            {
                await session.Navigation.Move(new GeoCoordinate(GlobalSettings.Default.DefaultLatitude, GlobalSettings.Default.DefaultLongitude),
session.LogicSettings.WalkingSpeedInKilometerPerHour,
async () =>
{
    // Catch normal map Pokemon
    await CatchNearbyPokemonsTask.Execute(session, cancellationToken);
    //Catch Incense Pokemon
    await CatchIncensePokemonsTask.Execute(session, cancellationToken);
    return true;
}, cancellationToken);
                return;
            }

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("EN-US");

            if (GlobalSettings.irLastPokeStopIndex >= GlobalSettings.lstPokeStopLocations.Count)
            {
                GlobalSettings.irLastPokeStopIndex = 0;
            }

            Logger.Write("Re setting global location no Poke Stop index " + GlobalSettings.irLastPokeStopIndex + " : "
                + GlobalSettings.lstPokeStopLocations[GlobalSettings.irLastPokeStopIndex], LogLevel.Self, ConsoleColor.Yellow);

            double dblLat = Convert.ToDouble(GlobalSettings.lstPokeStopLocations[GlobalSettings.irLastPokeStopIndex].Split(':')[0]);
            double dblLng = Convert.ToDouble(GlobalSettings.lstPokeStopLocations[GlobalSettings.irLastPokeStopIndex].Split(':')[1]);
            GlobalSettings.irLastPokeStopIndex++;

            await session.Navigation.Move(new GeoCoordinate(dblLat, dblLng),
      session.LogicSettings.WalkingSpeedInKilometerPerHour,
      async () =>
      {
          return true;
      }, cancellationToken);
        }
    }
}
