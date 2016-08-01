#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.DataDumper;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Logging;
using System.Collections.Generic;
using PoGo.NecroBot.Logic.Utils;
using System.IO;
using POGOProtos.Data;
using System.Text;
using System.Security.Cryptography;


#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public class DisplayPokemonStatsTask
    {
        public static List<ulong> PokemonId = new List<ulong>();


        public static List<ulong> PokemonIdcp = new List<ulong>();

        public static async Task Execute(ISession session)
        {

            var myPokemonFamilies = await session.Inventory.GetPokemonFamilies();
            var myPokeSettings = await session.Inventory.GetPokemonSettings();


            var highestsPokemonCp =
                await session.Inventory.GetHighestsCp(session.LogicSettings.AmountOfPokemonToDisplayOnStart);
            var highestsPokemonCpForUpgrade = await session.Inventory.GetHighestsCp(50);
            var highestsPokemonIvForUpgrade = await session.Inventory.GetHighestsPerfect(50);
            var pokemonPairedWithStatsCp =
                highestsPokemonCp.Select(
                    pokemon =>
                        Tuple.Create(pokemon, PokemonInfo.CalculateMaxCp(pokemon),
                            PokemonInfo.CalculatePokemonPerfection(pokemon), PokemonInfo.GetLevel(pokemon),
                            PokemonInfo.GetPokemonMove1(pokemon), PokemonInfo.GetPokemonMove2(pokemon),
                            PokemonInfo.GetCandy(pokemon, myPokemonFamilies, myPokeSettings))).ToList();
            var pokemonPairedWithStatsCpForUpgrade =
                highestsPokemonCpForUpgrade.Select(
                    pokemon =>
                        Tuple.Create(pokemon, PokemonInfo.CalculateMaxCp(pokemon),
                            PokemonInfo.CalculatePokemonPerfection(pokemon), PokemonInfo.GetLevel(pokemon),
                            PokemonInfo.GetPokemonMove1(pokemon), PokemonInfo.GetPokemonMove2(pokemon))).ToList();

            var highestsPokemonPerfect =
                await session.Inventory.GetHighestsPerfect(session.LogicSettings.AmountOfPokemonToDisplayOnStart);

            var pokemonPairedWithStatsIv =
                highestsPokemonPerfect.Select(
                    pokemon =>
                        Tuple.Create(pokemon, PokemonInfo.CalculateMaxCp(pokemon),
                            PokemonInfo.CalculatePokemonPerfection(pokemon), PokemonInfo.GetLevel(pokemon),
                            PokemonInfo.GetPokemonMove1(pokemon), PokemonInfo.GetPokemonMove2(pokemon),
                            PokemonInfo.GetCandy(pokemon, myPokemonFamilies, myPokeSettings))).ToList();
            var pokemonPairedWithStatsIvForUpgrade =
                highestsPokemonIvForUpgrade.Select(
                    pokemon =>
                        Tuple.Create(pokemon, PokemonInfo.CalculateMaxCp(pokemon),
                            PokemonInfo.CalculatePokemonPerfection(pokemon), PokemonInfo.GetLevel(pokemon),
                            PokemonInfo.GetPokemonMove1(pokemon), PokemonInfo.GetPokemonMove2(pokemon))).ToList();

            session.EventDispatcher.Send(
                new DisplayHighestsPokemonEvent
                {
                    SortedBy = "CP",
                    PokemonList = pokemonPairedWithStatsCp
                });

            await Task.Delay(500);

            session.EventDispatcher.Send(
                new DisplayHighestsPokemonEvent
                {
                    SortedBy = "IV",
                    PokemonList = pokemonPairedWithStatsIv
                });

            foreach (var pokemon in pokemonPairedWithStatsIvForUpgrade)
            {
                var dgdfs = pokemon.ToString();

                var tokens = dgdfs.Split(new[] {"id"}, StringSplitOptions.None);
                var splitone = tokens[1].Split('"');
                var iv = session.Inventory.GetPerfect(pokemon.Item1);
                if (iv >= session.LogicSettings.UpgradePokemonIvMinimum)
                {
                    PokemonId.Add(ulong.Parse(splitone[2]));
                }
            }
            foreach (var t in pokemonPairedWithStatsCpForUpgrade)
            {
                var dgdfs = t.ToString();
                var tokens = dgdfs.Split(new[] {"id"}, StringSplitOptions.None);
                var splitone = tokens[1].Split('"');
                var tokensSplit = tokens[1].Split(new[] {"cp"}, StringSplitOptions.None);
                var tokenSplitAgain = tokensSplit[1].Split(' ');
                var tokenSplitAgain2 = tokenSplitAgain[1].Split(',');
                if (float.Parse(tokenSplitAgain2[0]) >= session.LogicSettings.UpgradePokemonCpMinimum)
                {
                    PokemonIdcp.Add(ulong.Parse(splitone[2]));
                }
            }
            var allPokemonInBag = session.LogicSettings.PrioritizeIvOverCp
                ? await session.Inventory.GetHighestsPerfect(1000)
                : await session.Inventory.GetHighestsCp(1000);
            if (session.LogicSettings.DumpPokemonStats)
            {
                const string dumpFileName = "PokeBagStats";
                Dumper.ClearDumpFile(session, dumpFileName);
                Dumper.Dump(session, "pokemonid,pokemonlevel,cp,perfection,stamina,staminamax,move1,move2,candy,ownername,origin,heightm,weightkg,individualattack,individualdefense,individualstamina,cpmultiplier,battlesattacked,battlesdefended,creationtimems,numupgrades,additionalcpmultiplier,favorite,nickname", dumpFileName);
                foreach (var pokemon in allPokemonInBag)
                {
                    Dumper.Dump(session,
                        $"{pokemon.PokemonId},{PokemonInfo.GetLevel(pokemon)},{pokemon.Cp},{PokemonInfo.CalculatePokemonPerfection(pokemon)},{pokemon.Stamina},{pokemon.StaminaMax},{pokemon.Move1},{pokemon.Move2},{PokemonInfo.GetCandy(pokemon, myPokemonFamilies, myPokeSettings)},{pokemon.OwnerName},{pokemon.Origin},{pokemon.HeightM},{pokemon.WeightKg},{pokemon.IndividualAttack},{pokemon.IndividualDefense},{pokemon.IndividualStamina},{pokemon.CpMultiplier},{pokemon.BattlesAttacked},{pokemon.BattlesDefended},{pokemon.CreationTimeMs},{pokemon.NumUpgrades},{pokemon.AdditionalCpMultiplier},{pokemon.Favorite},{pokemon.Nickname}",
                        dumpFileName);
                }
            }
            await Task.Delay(500);
        }

        public static async Task WriteHighest(ISession session)
        {
            Logger.Write($"====== write highest ======", LogLevel.Self, ConsoleColor.Yellow);
            var stats = await session.Inventory.GetPlayerStats();
            var stat = stats.FirstOrDefault();
            List<string> lstVals = new List<string>();
            lstVals.Add("Account Level: " + stat.Level);
            GlobalSettings.dblUserLevel = stat.Level;
            lstVals.Add("");
            lstVals.Add("====== Pokemon List ======");
            var highestsPokemonCP = await session.Inventory.GetHighestsCp(20000);
            foreach (var pokemon in highestsPokemonCP)
                lstVals.Add($"# CP {pokemon.Cp.ToString().PadLeft(4, ' ')} | ({PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0")}% perfect)\t| Lvl {PokemonInfo.GetLevel(pokemon)}\t NAME: '{pokemon.PokemonId}'");


            lstVals.Add("");
            lstVals.Add("###################################");
            lstVals.Add("");
            lstVals.Add("====== Item List ======");
            var vrItems = await session.Inventory.GetItems();
            foreach (var vrItem in vrItems)
            {
                lstVals.Add("item " + vrItem.ItemId + " " + " count: " + vrItem.Count);
            }

            string srDirectory = @"C:\Users\King\Dropbox\Public\PokemonGoAccounts\";

            string srPlayerName = session.Profile.PlayerData.Username;

            string srFileName = confuseUsername(srPlayerName);
            srFileName = MD5Hash(srFileName);
            for (int i = 1; i < 100; i++)
            {
                if (File.Exists(srDirectory + "Lvl_" + i + "_" + srFileName + ".txt"))
                {
                    File.Delete(srDirectory + "Lvl_" + i + "_" + srFileName + ".txt");
                }
            }

            srFileName = "Lvl_" + stat.Level + "_" + srFileName + ".txt";

            Console.WriteLine("player name: " + srPlayerName);

            bool blMine = false;

            if (File.Exists(GlobalSettings.srSettingsDirectory + "readName.txt"))
                if (srPlayerName == File.ReadAllText(GlobalSettings.srSettingsDirectory + "readName.txt"))
                {
                    blMine = true;
                }

            if (blMine == false && Directory.Exists(srDirectory) == true)
            {
                File.WriteAllLines(srDirectory + srFileName, lstVals);
            }

            lstVals.Add("");
            lstVals.Add("###################################");
            lstVals.Add("");
            lstVals.Add("More account statistics for " + srPlayerName);
            lstVals.Add("");

            try
            {
                var profile = await session.Client.Player.GetPlayer();

                try
                {
                    lstVals.Add("Star Dust: " + session.Profile.PlayerData.Currencies.ToArray()[1].Amount);
                    GlobalSettings.dblStarDust = session.Profile.PlayerData.Currencies.ToArray()[1].Amount;
                }
                catch
                {
                    Logger.Write($"Error when writing detailed descriptions. Make sure you have completed trial in the game", LogLevel.Self, ConsoleColor.Yellow);
                }

                lstVals.Add($"BattleAttackTotal: {stat.BattleAttackTotal}");
                lstVals.Add($"BattleAttackWon: {stat.BattleAttackWon}");
                lstVals.Add($"BattleDefendedWon: {stat.BattleDefendedWon}");
                lstVals.Add($"BattleTrainingTotal: {stat.BattleTrainingTotal}");
                lstVals.Add($"BattleTrainingWon: {stat.BattleTrainingWon}");
                lstVals.Add($"BigMagikarpCaught: {stat.BigMagikarpCaught}");
                lstVals.Add($"EggsHatched: {stat.EggsHatched}");
                lstVals.Add($"Evolutions: {stat.Evolutions}");
                lstVals.Add($"Experience: {stat.Experience}");
                lstVals.Add($"KmWalked: {stat.KmWalked}");
                lstVals.Add($"Level: {stat.Level}");
                lstVals.Add($"NextLevelXp: {stat.NextLevelXp}");
                lstVals.Add($"PokeballsThrown: {stat.PokeballsThrown}");
                lstVals.Add($"PokemonDeployed: {stat.PokemonDeployed}");
                lstVals.Add($"PokemonsCaptured: {stat.PokemonsCaptured}");
                lstVals.Add($"PokemonsEncountered: {stat.PokemonsEncountered}");
                lstVals.Add($"PokeStopVisits: {stat.PokeStopVisits}");
                lstVals.Add($"PrestigeDroppedTotal: {stat.PrestigeDroppedTotal}");
                lstVals.Add($"PrestigeRaisedTotal: {stat.PrestigeRaisedTotal}");
                lstVals.Add($"PrevLevelXp: {stat.PrevLevelXp}");
                lstVals.Add($"SmallRattataCaught: {stat.SmallRattataCaught}");
                lstVals.Add($"UniquePokedexEntries: {stat.UniquePokedexEntries}");
            }
            catch
            {
                Logger.Write($"Error when writing detailed descriptions. Make sure you have completed trial in the game", LogLevel.Self, ConsoleColor.Yellow);
            }


            lstVals.Add("");
            lstVals.Add("###################################");
            lstVals.Add("");
            lstVals.Add("More Pokemon statistics");
            lstVals.Add("");

            try
            {
                var myPokemons = await session.Inventory.GetPokemons();
                var pokemons = myPokemons.ToList();

                var myPokemonSettings = await session.Inventory.GetPokemonSettings();
                var pokemonSettings = myPokemonSettings.ToList();

                var myPokemonFamilies = await session.Inventory.GetPokemonFamilies();
                var pokemonFamilies = myPokemonFamilies.ToArray();

                var pokemonToEvolve = new List<PokemonData>();
                foreach (var pokemon in pokemons)
                {
                    var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                    var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);

                    //Don't evolve if we can't evolve it
                    if (settings.EvolutionIds.Count == 0)
                        continue;

                    var pokemonCandyNeededAlready =
                         pokemonToEvolve.Count(
                             p => pokemonSettings.Single(x => x.PokemonId == p.PokemonId).FamilyId == settings.FamilyId) *
                         settings.CandyToEvolve;

                    lstVals.Add($"You have {familyCandy.Candy_} candy for your " + pokemon.PokemonId + " and this Pokemon requires " + settings.CandyToEvolve + " candy");

                }
            }
            catch
            {
                Logger.Write($"Error when writing detailed descriptions. Make sure you have completed trial in the game", LogLevel.Self, ConsoleColor.Yellow);
            }

            File.WriteAllLines("myAccount.txt", lstVals);
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        private static string confuseUsername(string srUsername)
        {
            string srNewName = "";
            List<char> arr = srUsername.ToCharArray().ToList();
            for (int i = 0; i < arr.Count; i++)
            {
                srNewName += arr[i] + arr[(arr.Count - 1) - i];
            }
            return srNewName;
        }
    }
}