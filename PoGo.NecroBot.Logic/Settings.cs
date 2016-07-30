#region using directives

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PoGo.NecroBot.Logic.Logging;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
<<<<<<< HEAD
using PoGo.NecroBot.Logic.Logging;
using System.Linq;
=======

>>>>>>> refs/remotes/upstream/master
#endregion

namespace PoGo.NecroBot.Logic
{
    internal class AuthSettings
    {
        [JsonIgnore] private string _filePath;

        public AuthType AuthType;
        public string GooglePassword;

        public string GoogleRefreshToken;
        public string GoogleUsername;
        public string PtcPassword;
        public string PtcUsername;

        public void Load(string path)
        {
            try
            {
                _filePath = path;

                if (File.Exists(_filePath))
                {
                    //if the file exists, load the settings
                    var input = File.ReadAllText(_filePath);

                    var settings = new JsonSerializerSettings();
                    settings.Converters.Add(new StringEnumConverter {CamelCaseText = true});

                    JsonConvert.PopulateObject(input, this, settings);
                }
                else
                {
                    Save(_filePath);
                }
            }
<<<<<<< HEAD
            catch (Newtonsoft.Json.JsonReaderException exception)
=======
            catch (JsonReaderException exception)
>>>>>>> refs/remotes/upstream/master
            {
                if (exception.Message.Contains("Unexpected character") && exception.Message.Contains("PtcUsername"))
                    Logger.Write("JSON Exception: You need to properly configure your PtcUsername using quotations.",
                        LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") && exception.Message.Contains("PtcPassword"))
                    Logger.Write(
                        "JSON Exception: You need to properly configure your PtcPassword using quotations.",
                        LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") &&
                         exception.Message.Contains("GoogleUsername"))
                    Logger.Write(
                        "JSON Exception: You need to properly configure your GoogleUsername using quotations.",
                        LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") &&
                         exception.Message.Contains("GooglePassword"))
                    Logger.Write(
                        "JSON Exception: You need to properly configure your GooglePassword using quotations.",
                        LogLevel.Error);
                else
                    Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
            }
        }

        public void Save(string path)
        {
            var output = JsonConvert.SerializeObject(this, Formatting.Indented,
                new StringEnumConverter {CamelCaseText = true});

            var folder = Path.GetDirectoryName(path);
            if (folder != null && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(path, output);
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                Save(_filePath);
            }
        }
    }

    public class GlobalSettings
    {
        public int AmountOfPokemonToDisplayOnStart = 10;

        [JsonIgnore] internal AuthSettings Auth = new AuthSettings();

        public bool AutomaticallyLevelUpPokemon = false;

        public bool AutoUpdate = false;
        public double DefaultAltitude = 10;
<<<<<<< HEAD
        public double DefaultLatitude = 36.80204;
        public double DefaultLongitude = 34.63328;
        public int DelayBetweenPokemonCatch = 2000;
        public int DelayBetweenPlayerActions = 5000;
        public float EvolveAboveIvValue = 50;
=======
        public double DefaultLatitude = 40.785091;
        public double DefaultLongitude = -73.968285;
        public int DelayBetweenPlayerActions = 5000;
        public int DelayBetweenPokemonCatch = 2000;
        public bool DumpPokemonStats = false;
        public float EvolveAboveIvValue = 95;
>>>>>>> refs/remotes/upstream/master
        public bool EvolveAllPokemonAboveIv = false;
        public bool EvolveAllPokemonWithEnoughCandy = true;

        [JsonIgnore] public string GeneralConfigPath;

        public string GpxFile = "GPXPath.GPX";
<<<<<<< HEAD
        public bool UseGpxPathing = false;
        public double WalkingSpeedInKilometerPerHour = 130;
        public int MaxTravelDistanceInMeters = 10000;
        public int KeepMinCp = 5000;
        public int KeepMinDuplicatePokemon = 2;
        public float KeepMinIvPercentage = 95;
        public bool KeepPokemonsThatCanEvolve = false;
        public bool PrioritizeIvOverCp = true;
        public bool RenameAboveIv = false;
        public string RenameTemplate = "{1}_{0}";
        public bool TransferDuplicatePokemon = true;
        public string TranslationLanguageCode = "en";
        public bool UsePokemonToNotCatchFilter = false;
        public int WebSocketPort = 14251;
        public bool StartupWelcomeDelay = false;
        public bool SnipeAtPokestops = false;
        public int MinPokeballsToSnipe = 20;
        public string SnipeLocationServer = "localhost";
        public int SnipeLocationServerPort = 16969;
        public bool UseSnipeLocationServer = false;
        public bool UseTransferIVForSnipe = false;
        public bool SnipeIgnoreUnknownIV = false;
        public int MinDelayBetweenSnipes = 20000;
        public int TotalAmountOfPokebalsToKeep = 1500;
        public int TotalAmountOfPotionsToKeep = 1000;
        public int TotalAmountOfRevivesToKeep = 500;
        public int irPowerUpPerfectionIV = 90;
=======
>>>>>>> refs/remotes/upstream/master


        public List<KeyValuePair<ItemId, int>> ItemRecycleFilter = new List<KeyValuePair<ItemId, int>>
        {
            new KeyValuePair<ItemId, int>(ItemId.ItemUnknown, 0),
            new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, 200),
            new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, 4000),
            new KeyValuePair<ItemId, int>(ItemId.ItemMasterBall, 4000),
            new KeyValuePair<ItemId, int>(ItemId.ItemPotion, 0),
            new KeyValuePair<ItemId, int>(ItemId.ItemSuperPotion, 0),
            new KeyValuePair<ItemId, int>(ItemId.ItemHyperPotion, 50),
            new KeyValuePair<ItemId, int>(ItemId.ItemMaxPotion, 50),
            new KeyValuePair<ItemId, int>(ItemId.ItemRevive, 25),
            new KeyValuePair<ItemId, int>(ItemId.ItemMaxRevive, 25),
            new KeyValuePair<ItemId, int>(ItemId.ItemLuckyEgg, 200),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseOrdinary, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseSpicy, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseCool, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncenseFloral, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemTroyDisk, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemXAttack, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemXDefense, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemXMiracle, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemRazzBerry, 50),
            new KeyValuePair<ItemId, int>(ItemId.ItemBlukBerry, 10),
            new KeyValuePair<ItemId, int>(ItemId.ItemNanabBerry, 10),
            new KeyValuePair<ItemId, int>(ItemId.ItemWeparBerry, 30),
            new KeyValuePair<ItemId, int>(ItemId.ItemPinapBerry, 30),
            new KeyValuePair<ItemId, int>(ItemId.ItemSpecialCamera, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncubatorBasicUnlimited, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemIncubatorBasic, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemPokemonStorageUpgrade, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemItemStorageUpgrade, 100)
        };

        public int KeepMinCp = 1250;
        public int KeepMinDuplicatePokemon = 1;
        public float KeepMinIvPercentage = 90;
        public bool KeepPokemonsThatCanEvolve = false;
        public string LevelUpByCPorIv = "iv";
        public int MaxPokeballsPerPokemon = 6;
        public int MaxSpawnLocationOffset = 10;
        public int MaxTravelDistanceInMeters = 1000;
        public int MinDelayBetweenSnipes = 60000;
        public int MinPokeballsToSnipe = 20;
        public int MinPokeballsWhileSnipe = 0;

        public List<PokemonId> PokemonsNotToTransfer = new List<PokemonId>
        {
<<<<<<< HEAD
            //PokemonId.Aerodactyl,
            //PokemonId.Venusaur,
            //PokemonId.Charizard,
            //PokemonId.Blastoise,
            //PokemonId.Nidoqueen,
            //PokemonId.Nidoking,
            //PokemonId.Clefable,
            //PokemonId.Vileplume,
            //PokemonId.Arcanine,
            //PokemonId.Poliwrath,
            //PokemonId.Machamp,
            //PokemonId.Victreebel,
            //PokemonId.Golem,
            //PokemonId.Slowbro,
            //PokemonId.Farfetchd,
            //PokemonId.Muk,
            //PokemonId.Exeggutor,
            //PokemonId.Lickitung,
            //PokemonId.Chansey,
            //PokemonId.Kangaskhan,
            //PokemonId.MrMime,
            //PokemonId.Gyarados,
            //PokemonId.Lapras,
            //PokemonId.Ditto,
=======
            //criteria: from SS Tier to A Tier + Regional Exclusive
            PokemonId.Venusaur,
            PokemonId.Charizard,
            PokemonId.Blastoise,
            PokemonId.Nidoqueen,
            PokemonId.Nidoking,
            PokemonId.Clefable,
            PokemonId.Vileplume,
            //PokemonId.Golduck,
            PokemonId.Arcanine,
            PokemonId.Poliwrath,
            PokemonId.Machamp,
            PokemonId.Victreebel,
            PokemonId.Golem,
            PokemonId.Slowbro,
            //PokemonId.Farfetchd,
            PokemonId.Muk,
            PokemonId.Exeggutor,
            PokemonId.Lickitung,
            PokemonId.Chansey,
            //PokemonId.Kangaskhan,
            //PokemonId.MrMime,
            //PokemonId.Tauros,
            //PokemonId.Gyarados,
            //PokemonId.Lapras,
            PokemonId.Ditto,
>>>>>>> refs/remotes/upstream/master
            //PokemonId.Vaporeon,
            //PokemonId.Jolteon,
            //PokemonId.Flareon,
            //PokemonId.Porygon,
            //PokemonId.Snorlax,
<<<<<<< HEAD
            //PokemonId.Articuno,
            //PokemonId.Zapdos,
            //PokemonId.Moltres,
            //PokemonId.Dragonite,
            //PokemonId.Mewtwo,
            //PokemonId.Mew
            //PokemonId.Golduck,
=======
            PokemonId.Articuno,
            PokemonId.Zapdos,
            PokemonId.Moltres,
            //PokemonId.Dragonite,
            PokemonId.Mewtwo,
            PokemonId.Mew
>>>>>>> refs/remotes/upstream/master
        };

        public List<PokemonId> PokemonsToEvolve = new List<PokemonId>
        {
            /*NOTE: keep all the end-of-line commas exept for the last one or an exception will be thrown!
            criteria: 12 candies*/
            PokemonId.Caterpie,
            PokemonId.Weedle,
            PokemonId.Pidgey,
            /*criteria: 25 candies*/
            //PokemonId.Bulbasaur,
            //PokemonId.Charmander,
            //PokemonId.Squirtle,
            PokemonId.Rattata
            //PokemonId.NidoranFemale,
            //PokemonId.NidoranMale,
            //PokemonId.Oddish,
            //PokemonId.Poliwag,
            //PokemonId.Abra,
            //PokemonId.Machop,
            //PokemonId.Bellsprout,
            //PokemonId.Geodude,
            //PokemonId.Gastly,
            //PokemonId.Eevee,
            //PokemonId.Dratini,
            /*criteria: 50 candies commons*/
            //PokemonId.Spearow,
            //PokemonId.Ekans,
            //PokemonId.Zubat,
            //PokemonId.Paras,
            //PokemonId.Venonat,
            //PokemonId.Psyduck,
            //PokemonId.Slowpoke,
            //PokemonId.Doduo,
            //PokemonId.Drowzee,
            //PokemonId.Krabby,
            //PokemonId.Horsea,
            //PokemonId.Goldeen,
            //PokemonId.Staryu
        };

        public List<PokemonId> PokemonsToIgnore = new List<PokemonId>
        {
            //criteria: most common
            PokemonId.Caterpie,
            PokemonId.Weedle,
            PokemonId.Pidgey,
            PokemonId.Rattata,
            PokemonId.Spearow,
            PokemonId.Zubat,
            PokemonId.Doduo
        };

        public Dictionary<PokemonId, TransferFilter> PokemonsTransferFilter = new Dictionary<PokemonId, TransferFilter>
        {
            //criteria: based on NY Central Park and Tokyo variety + sniping optimization
            {PokemonId.Golduck, new TransferFilter(1800, 95, 1)},
            {PokemonId.Farfetchd, new TransferFilter(1250, 80, 1)},
            {PokemonId.Krabby, new TransferFilter(1250, 95, 1)},
            {PokemonId.Kangaskhan, new TransferFilter(1500, 60, 1)},
            {PokemonId.Horsea, new TransferFilter(1250, 95, 1)},
            {PokemonId.Staryu, new TransferFilter(1250, 95, 1)},
            {PokemonId.MrMime, new TransferFilter(1250, 40, 1)},
            {PokemonId.Scyther, new TransferFilter(1800, 80, 1)},
            {PokemonId.Jynx, new TransferFilter(1250, 95, 1)},
            {PokemonId.Electabuzz, new TransferFilter(1250, 80, 1)},
            {PokemonId.Magmar, new TransferFilter(1500, 80, 1)},
            {PokemonId.Pinsir, new TransferFilter(1800, 95, 1)},
            {PokemonId.Tauros, new TransferFilter(1250, 90, 1)},
            {PokemonId.Magikarp, new TransferFilter(1250, 95, 1)},
            {PokemonId.Gyarados, new TransferFilter(1250, 90, 1)},
            {PokemonId.Lapras, new TransferFilter(1800, 80, 1)},
            {PokemonId.Eevee, new TransferFilter(1250, 95, 1)},
            {PokemonId.Vaporeon, new TransferFilter(1500, 90, 1)},
            {PokemonId.Jolteon, new TransferFilter(1500, 90, 1)},
            {PokemonId.Flareon, new TransferFilter(1500, 90, 1)},
            {PokemonId.Porygon, new TransferFilter(1250, 60, 1)},
            {PokemonId.Snorlax, new TransferFilter(2600, 90, 1)},
            {PokemonId.Dragonite, new TransferFilter(2600, 90, 1)}
        };

        public SnipeSettings PokemonToSnipe = new SnipeSettings
        {
            Locations = new List<Location>
            {
                new Location(38.55680748646112, -121.2383794784546), //Dratini Spot
                new Location(-33.85901900, 151.21309800), //Magikarp Spot
                new Location(47.5014969, -122.0959568), //Eevee Spot
                new Location(51.5025343, -0.2055027) //Charmender Spot
            },
            Pokemon = new List<PokemonId>
            {
                PokemonId.Venusaur,
                PokemonId.Charizard,
                PokemonId.Blastoise,
                PokemonId.Beedrill,
                PokemonId.Raichu,
                PokemonId.Sandslash,
                PokemonId.Nidoking,
                PokemonId.Nidoqueen,
                PokemonId.Clefable,
                PokemonId.Ninetales,
                PokemonId.Golbat,
                PokemonId.Vileplume,
                PokemonId.Golduck,
                PokemonId.Primeape,
                PokemonId.Arcanine,
                PokemonId.Poliwrath,
                PokemonId.Alakazam,
                PokemonId.Machamp,
                PokemonId.Golem,
                PokemonId.Rapidash,
                PokemonId.Slowbro,
                PokemonId.Farfetchd,
                PokemonId.Muk,
                PokemonId.Cloyster,
                PokemonId.Gengar,
                PokemonId.Exeggutor,
                PokemonId.Marowak,
                PokemonId.Hitmonchan,
                PokemonId.Lickitung,
                PokemonId.Rhydon,
                PokemonId.Chansey,
                PokemonId.Kangaskhan,
                PokemonId.Starmie,
                PokemonId.MrMime,
                PokemonId.Scyther,
                PokemonId.Magmar,
                PokemonId.Electabuzz,
                PokemonId.Magmar,
                PokemonId.Jynx,
                PokemonId.Gyarados,
                PokemonId.Lapras,
                PokemonId.Ditto,
                PokemonId.Vaporeon,
                PokemonId.Jolteon,
                PokemonId.Flareon,
                PokemonId.Porygon,
                PokemonId.Kabutops,
                PokemonId.Aerodactyl,
                PokemonId.Snorlax,
                PokemonId.Articuno,
                PokemonId.Zapdos,
                PokemonId.Moltres,
                PokemonId.Dragonite,
                PokemonId.Mewtwo,
                PokemonId.Mew
            }
        };

        public List<PokemonId> PokemonToUseMasterball = new List<PokemonId>
        {
            PokemonId.Articuno,
            PokemonId.Zapdos,
            PokemonId.Moltres,
            PokemonId.Mew,
            PokemonId.Mewtwo
        };

        public bool PrioritizeIvOverCp = true;

        [JsonIgnore] public string ProfileConfigPath;

        [JsonIgnore] public string ProfilePath;

        public bool RenameOnlyAboveIv = true;
        public bool RenamePokemon = false;
        public string RenameTemplate = "{1}_{0}";
        public bool SnipeAtPokestops = false;
        public bool SnipeIgnoreUnknownIv = false;
        public string SnipeLocationServer = "localhost";
        public int SnipeLocationServerPort = 16969;
        public bool StartupWelcomeDelay = true;
        public int TotalAmountOfPokebalsToKeep = 120;
        public int TotalAmountOfPotionsToKeep = 80;
        public int TotalAmountOfRevivesToKeep = 60;
        public bool TransferConfigAndAuthOnUpdate = true;
        public bool TransferDuplicatePokemon = true;
        public string TranslationLanguageCode = "en";
        public float UpgradePokemonCpMinimum = 1000;
        public float UpgradePokemonIvMinimum = 95;
        public bool UseEggIncubators = true;
        public bool UseGpxPathing = false;
        public int UseGreatBallAboveCp = 750;
        public int UseLuckyEggsMinPokemonAmount = 30;
        public bool UseLuckyEggsWhileEvolving = false;
        public int UseMasterBallAboveCp = 1500;
        public bool UsePokemonToNotCatchFilter = false;
        public bool UseSnipeLocationServer = false;
        public bool UseTransferIvForSnipe = false;
        public int UseUltraBallAboveCp = 1000;
        public double WalkingSpeedInKilometerPerHour = 15.0;
        public int WebSocketPort = 14251;

        public static GlobalSettings Default => new GlobalSettings();

        public static string srSettingsDirectory = @"D:\74 pokemon go\settings\";
        public static string srPokemonGoMap_Directory = @"D:\74 pokemon go\PokemonGo-Map-master\";

        public static List<string> lstPokeStopLocations = new List<string> { };
        public static int irLastPokeStopIndex = 0;
        public static List<int> lstPriorityPokemon = new List<int> { 150, 149, 151, 146, 145, 143, 59, 131, 144, 103, 134, 130, 136, 89, 6, 80, 68, 3, 9, 71, 62, 45, 31, 34, 36, 55, 76, 126, 110, 112, 139, 73, 78, 38, 97, 121, 2, 3, 5, 6, 8, 9, 26, 31, 34, 36, 45, 65, 68, 71, 76, 83, 89, 97, 105, 107, 108, 110, 113, 115, 124, 128, 131, 132, 135, 137, 139, 141, 142, 144, 145, 146 };
        public static bool blEnableRareHunt = true;
        public static bool blCriticalBall = false;
        public static int irCritical_Ball_Lowest = 25;
        public static int irCritical_Ball_Upper = 50;
        public static double dblUserLevel = 0;
        public static double dblStarDust = 0;

        public static GlobalSettings Load(string path)
        {
            GlobalSettings settings;
            var profilePath = Path.Combine(Directory.GetCurrentDirectory(), path);
            var profileConfigPath = Path.Combine(profilePath, "config");
            var configFile = Path.Combine(profileConfigPath, "config.json");

            //personal stuff
            bool blOverWriteSettings = false;
            if (File.Exists(srSettingsDirectory + "overwrite.txt"))
            {
                blOverWriteSettings = true;
            }

            //personal stuff
            if (File.Exists(srSettingsDirectory + "predefined_pokestop_locs.txt"))
            {
                lstPokeStopLocations = File.ReadAllLines(srSettingsDirectory + "predefined_pokestop_locs.txt").ToList();
            }

            //personal stuff
            if (File.Exists(srSettingsDirectory + "auths.txt"))
            {
                foreach (var vrLine in File.ReadLines(srSettingsDirectory + "auths.txt"))
                {
                    List<string> lstParams = vrLine.Split(';').ToList();
                    string srAuth = "{{" + Environment.NewLine +
                          "  \"AuthType\": \"google\"," + Environment.NewLine +
                          "  \"GoogleRefreshToken\": null," + Environment.NewLine +
                          "  \"PtcUsername\": null," + Environment.NewLine +
                          "  \"PtcPassword\": null," + Environment.NewLine +
                          "  \"GoogleUsername\": \"{0}\"," + Environment.NewLine +
                          "  \"GooglePassword\": \"{1}\"" + Environment.NewLine +
                          "  }}";

                    string srTempAuth = string.Format(srAuth, lstParams[1], lstParams[2]);
                    string srPath = @"D:\74 pokemon go\hesap botlar\{0}\Config\auth.json";
                    File.WriteAllText( string.Format(srPath, lstParams[0]), srTempAuth);
                }
            }

            if (File.Exists(configFile) && blOverWriteSettings == false)
            {
                try
                {
                    //if the file exists, load the settings
                    var input = File.ReadAllText(configFile);

                    var jsonSettings = new JsonSerializerSettings();
                    jsonSettings.Converters.Add(new StringEnumConverter {CamelCaseText = true});
                    jsonSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    jsonSettings.DefaultValueHandling = DefaultValueHandling.Populate;

                    settings = JsonConvert.DeserializeObject<GlobalSettings>(input, jsonSettings);
                }
                catch (JsonReaderException exception)
                {
                    Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
                    return null;
                }
            }
            else
            {
                settings = new GlobalSettings();
            }

            if (settings.WebSocketPort == 0)
            {
                settings.WebSocketPort = 14251;
            }

            if (settings.PokemonToSnipe == null)
            {
                settings.PokemonToSnipe = Default.PokemonToSnipe;
            }

            if (settings.RenameTemplate == null)
            {
                settings.RenameTemplate = Default.RenameTemplate;
            }

            if (settings.SnipeLocationServer == null)
            {
                settings.SnipeLocationServer = Default.SnipeLocationServer;
            }

            settings.ProfilePath = profilePath;
            settings.ProfileConfigPath = profileConfigPath;
            settings.GeneralConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config");

            var firstRun = !File.Exists(configFile);

            settings.Save(configFile);
            settings.Auth.Load(Path.Combine(profileConfigPath, "auth.json"));

            if (firstRun && blOverWriteSettings == false)
            {
                return null;
            }

            return settings;
        }

        public void Save(string fullPath)
        {
            var output = JsonConvert.SerializeObject(this, Formatting.Indented,
                new StringEnumConverter {CamelCaseText = true});

            var folder = Path.GetDirectoryName(fullPath);
            if (folder != null && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(fullPath, output);
        }
    }

    public class ClientSettings : ISettings
    {
        // Never spawn at the same position.
        private readonly Random _rand = new Random();
        private readonly GlobalSettings _settings;

        public ClientSettings(GlobalSettings settings)
        {
            _settings = settings;
        }


        public string GoogleUsername => _settings.Auth.GoogleUsername;
        public string GooglePassword => _settings.Auth.GooglePassword;

        public string GoogleRefreshToken
        {
            get { return _settings.Auth.GoogleRefreshToken; }
            set
            {
                _settings.Auth.GoogleRefreshToken = value;
                _settings.Auth.Save();
            }
        }

        AuthType ISettings.AuthType
        {
            get { return _settings.Auth.AuthType; }

            set { _settings.Auth.AuthType = value; }
        }

        double ISettings.DefaultLatitude
        {
            get
            {
                return _settings.DefaultLatitude + _rand.NextDouble()*((double) _settings.MaxSpawnLocationOffset/111111);
            }

            set { _settings.DefaultLatitude = value; }
        }

        double ISettings.DefaultLongitude
        {
            get
            {
                return _settings.DefaultLongitude +
                       _rand.NextDouble()*
                       ((double) _settings.MaxSpawnLocationOffset/111111/Math.Cos(_settings.DefaultLatitude));
            }

            set { _settings.DefaultLongitude = value; }
        }

        double ISettings.DefaultAltitude
        {
            get { return _settings.DefaultAltitude; }

            set { _settings.DefaultAltitude = value; }
        }

        string ISettings.PtcPassword
        {
            get { return _settings.Auth.PtcPassword; }

            set { _settings.Auth.PtcPassword = value; }
        }

        string ISettings.PtcUsername
        {
            get { return _settings.Auth.PtcUsername; }

            set { _settings.Auth.PtcUsername = value; }
        }

        string ISettings.GoogleUsername
        {
            get { return _settings.Auth.GoogleUsername; }

            set { _settings.Auth.GoogleUsername = value; }
        }

        string ISettings.GooglePassword
        {
            get { return _settings.Auth.GooglePassword; }

            set { _settings.Auth.GooglePassword = value; }
        }
    }

    public class LogicSettings : ILogicSettings
    {
        private readonly GlobalSettings _settings;

        public LogicSettings(GlobalSettings settings)
        {
            _settings = settings;
        }

        public string ProfilePath => _settings.ProfilePath;
        public string ProfileConfigPath => _settings.ProfileConfigPath;
        public string GeneralConfigPath => _settings.GeneralConfigPath;
        public bool AutoUpdate => _settings.AutoUpdate;
        public bool TransferConfigAndAuthOnUpdate => _settings.TransferConfigAndAuthOnUpdate;
        public float KeepMinIvPercentage => _settings.KeepMinIvPercentage;
        public int KeepMinCp => _settings.KeepMinCp;
        public bool AutomaticallyLevelUpPokemon => _settings.AutomaticallyLevelUpPokemon;
        public string LevelUpByCPorIv => _settings.LevelUpByCPorIv;
        public float UpgradePokemonIvMinimum => _settings.UpgradePokemonIvMinimum;
        public float UpgradePokemonCpMinimum => _settings.UpgradePokemonCpMinimum;
        public double WalkingSpeedInKilometerPerHour => _settings.WalkingSpeedInKilometerPerHour;
        public bool EvolveAllPokemonWithEnoughCandy => _settings.EvolveAllPokemonWithEnoughCandy;
        public bool KeepPokemonsThatCanEvolve => _settings.KeepPokemonsThatCanEvolve;
        public bool TransferDuplicatePokemon => _settings.TransferDuplicatePokemon;
        public bool UseEggIncubators => _settings.UseEggIncubators;
        public int UseGreatBallAboveCp => _settings.UseGreatBallAboveCp;
        public int UseUltraBallAboveCp => _settings.UseUltraBallAboveCp;
        public int UseMasterBallAboveCp => _settings.UseMasterBallAboveCp;
        public int DelayBetweenPokemonCatch => _settings.DelayBetweenPokemonCatch;
        public int DelayBetweenPlayerActions => _settings.DelayBetweenPlayerActions;
        public bool UsePokemonToNotCatchFilter => _settings.UsePokemonToNotCatchFilter;
        public int KeepMinDuplicatePokemon => _settings.KeepMinDuplicatePokemon;
        public bool PrioritizeIvOverCp => _settings.PrioritizeIvOverCp;
        public int MaxTravelDistanceInMeters => _settings.MaxTravelDistanceInMeters;
        public string GpxFile => _settings.GpxFile;
        public bool UseGpxPathing => _settings.UseGpxPathing;
        public bool UseLuckyEggsWhileEvolving => _settings.UseLuckyEggsWhileEvolving;
        public int UseLuckyEggsMinPokemonAmount => _settings.UseLuckyEggsMinPokemonAmount;
        public bool EvolveAllPokemonAboveIv => _settings.EvolveAllPokemonAboveIv;
        public float EvolveAboveIvValue => _settings.EvolveAboveIvValue;
        public bool RenamePokemon => _settings.RenamePokemon;
        public bool RenameOnlyAboveIv => _settings.RenameOnlyAboveIv;
        public string RenameTemplate => _settings.RenameTemplate;
        public int AmountOfPokemonToDisplayOnStart => _settings.AmountOfPokemonToDisplayOnStart;
        public bool DumpPokemonStats => _settings.DumpPokemonStats;
        public string TranslationLanguageCode => _settings.TranslationLanguageCode;
        public ICollection<KeyValuePair<ItemId, int>> ItemRecycleFilter => _settings.ItemRecycleFilter;
        public ICollection<PokemonId> PokemonsToEvolve => _settings.PokemonsToEvolve;
        public ICollection<PokemonId> PokemonsNotToTransfer => _settings.PokemonsNotToTransfer;
        public ICollection<PokemonId> PokemonsNotToCatch => _settings.PokemonsToIgnore;
        public ICollection<PokemonId> PokemonToUseMasterball => _settings.PokemonToUseMasterball;
        public Dictionary<PokemonId, TransferFilter> PokemonsTransferFilter => _settings.PokemonsTransferFilter;
        public bool StartupWelcomeDelay => _settings.StartupWelcomeDelay;
        public bool SnipeAtPokestops => _settings.SnipeAtPokestops;
        public int MinPokeballsToSnipe => _settings.MinPokeballsToSnipe;
        public int MinPokeballsWhileSnipe => _settings.MinPokeballsWhileSnipe;
        public int MaxPokeballsPerPokemon => _settings.MaxPokeballsPerPokemon;

        public SnipeSettings PokemonToSnipe => _settings.PokemonToSnipe;
        public string SnipeLocationServer => _settings.SnipeLocationServer;
        public int SnipeLocationServerPort => _settings.SnipeLocationServerPort;
        public bool UseSnipeLocationServer => _settings.UseSnipeLocationServer;
        public bool UseTransferIvForSnipe => _settings.UseTransferIvForSnipe;
        public bool SnipeIgnoreUnknownIv => _settings.SnipeIgnoreUnknownIv;
        public int MinDelayBetweenSnipes => _settings.MinDelayBetweenSnipes;
        public int TotalAmountOfPokebalsToKeep => _settings.TotalAmountOfPokebalsToKeep;
        public int TotalAmountOfPotionsToKeep => _settings.TotalAmountOfPotionsToKeep;
        public int TotalAmountOfRevivesToKeep => _settings.TotalAmountOfRevivesToKeep;
        public int irPowerUpPerfectionIV => _settings.irPowerUpPerfectionIV;
    }
}