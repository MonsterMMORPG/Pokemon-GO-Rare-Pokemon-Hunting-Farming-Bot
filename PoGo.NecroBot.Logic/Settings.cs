#region using directives

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PoGo.NecroBot.Logic;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using PoGo.NecroBot.Logic.Logging;
using System.Linq;
#endregion

namespace PoGo.NecroBot.CLI
{
    internal class AuthSettings
    {
        public AuthType AuthType;


        [JsonIgnore]
        private string _filePath;

        public string GoogleRefreshToken;
        public string PtcUsername;
        public string PtcPassword;
        public string GoogleUsername;
        public string GooglePassword;

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
                    settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

                    JsonConvert.PopulateObject(input, this, settings);
                }
                else
                {
                    Save(_filePath);
                }
            }
            catch (Newtonsoft.Json.JsonReaderException exception)
            {
                if (exception.Message.Contains("Unexpected character") && exception.Message.Contains("PtcUsername"))
                    Logger.Write("JSON Exception: You need to properly configure your PtcUsername using quotations.", LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") && exception.Message.Contains("PtcPassword"))
                    Logger.Write("JSON Exception: You need to properly configure your PtcPassword using quotations.", LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") && exception.Message.Contains("GoogleUsername"))
                    Logger.Write("JSON Exception: You need to properly configure your GoogleUsername using quotations.", LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") && exception.Message.Contains("GooglePassword"))
                    Logger.Write("JSON Exception: You need to properly configure your GooglePassword using quotations.", LogLevel.Error);
                else
                    Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
            }
        }

        public void Save(string path)
        {
            var output = JsonConvert.SerializeObject(this, Formatting.Indented,
                new StringEnumConverter { CamelCaseText = true });

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
        public bool TransferConfigAndAuthOnUpdate = true;

        [JsonIgnore]
        internal AuthSettings Auth = new AuthSettings();
        [JsonIgnore]
        public string ProfilePath;
        [JsonIgnore]
        public string ProfileConfigPath;
        [JsonIgnore]
        public string GeneralConfigPath;

        public bool AutoUpdate = false;
        public double DefaultAltitude = 10;
        public double DefaultLatitude = 36.80204;
        public double DefaultLongitude = 34.63328;
        public int DelayBetweenPokemonCatch = 2000;
        public int DelayBetweenPlayerActions = 5000;
        public float EvolveAboveIvValue = 50;
        public bool EvolveAllPokemonAboveIv = false;
        public bool EvolveAllPokemonWithEnoughCandy = true;
        public int UseLuckyEggsMinPokemonAmount = 30;
        public bool UseLuckyEggsWhileEvolving = false;
        public bool UseEggIncubators = true;
        public bool DumpPokemonStats = false;
        public string GpxFile = "GPXPath.GPX";
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


        public List<KeyValuePair<ItemId, int>> ItemRecycleFilter = new List<KeyValuePair<ItemId, int>>
        {
            new KeyValuePair<ItemId, int>(ItemId.ItemUnknown, 0),
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

        public List<PokemonId> PokemonsNotToTransfer = new List<PokemonId>
        {
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
            //PokemonId.Vaporeon,
            //PokemonId.Jolteon,
            //PokemonId.Flareon,
            //PokemonId.Porygon,
            //PokemonId.Snorlax,
            //PokemonId.Articuno,
            //PokemonId.Zapdos,
            //PokemonId.Moltres,
            //PokemonId.Dragonite,
            //PokemonId.Mewtwo,
            //PokemonId.Mew
            //PokemonId.Golduck,
        };

        public List<PokemonId> PokemonsToEvolve = new List<PokemonId>
        {
            //12 candies
            PokemonId.Caterpie,
            PokemonId.Weedle,
            PokemonId.Pidgey,
            //25 candies
            PokemonId.Rattata,
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
            //PokemonId.Dratini
            //50 candies
            PokemonId.Spearow,
            PokemonId.Zubat,
            PokemonId.Doduo,
            PokemonId.Goldeen,
            PokemonId.Paras,
            PokemonId.Ekans,
            PokemonId.Staryu,
            PokemonId.Psyduck,
            PokemonId.Krabby,
            PokemonId.Venonat
        };

        public List<PokemonId> PokemonsToIgnore = new List<PokemonId>
        {
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
            {PokemonId.Pidgeotto, new TransferFilter(1500, 90, 1)},
            {PokemonId.Fearow, new TransferFilter(1500, 90, 2)},
            {PokemonId.Zubat, new TransferFilter(500, 90, 2)},
            {PokemonId.Golbat, new TransferFilter(1500, 90, 2)},
            {PokemonId.Pinsir, new TransferFilter(1500, 95, 2)},
            {PokemonId.Golduck, new TransferFilter(1350, 95, 2)},
            {PokemonId.Tentacruel, new TransferFilter(1350, 95, 2)},
            {PokemonId.Starmie, new TransferFilter(1350, 95, 2)},
            {PokemonId.Eevee, new TransferFilter(750, 92, 2)},
            {PokemonId.Gyarados, new TransferFilter(1200, 90, 5)},
            {PokemonId.Mew, new TransferFilter(0, 0, 10)}
        };

        public SnipeSettings PokemonToSnipe = new SnipeSettings
        {
            Locations = new List<Location>
            {
                new Location(38.55680748646112, -121.2383794784546), //Dratini Spot
                new Location(-33.85901900, 151.21309800), //Magikarp Spot
                new Location(47.5014969, -122.0959568), //Eevee Spot
                new Location(51.5025343,-0.2055027) //Charmender Spot

            },
            Pokemon = new List<PokemonId>()
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

        public static GlobalSettings Default => new GlobalSettings();

        public static string srSettingsDirectory = @"D:\74 pokemon go\settings\";

        public static List<string> lstPokeStopLocations = new List<string> { };
        public static int irLastPokeStopIndex = 0;

        public static GlobalSettings Load(string path)
        {
            GlobalSettings settings;
            var profilePath = Path.Combine(Directory.GetCurrentDirectory(), path);
            var profileConfigPath = Path.Combine(profilePath, "config");
            var configFile = Path.Combine(profileConfigPath, "config.json");

            bool blOverWriteSettings = false;
            if (File.Exists(srSettingsDirectory + "overwrite.txt"))
            {
                blOverWriteSettings = true;
            }

            if (File.Exists(srSettingsDirectory + "predefined_pokestop_locs.txt"))
            {
                lstPokeStopLocations = File.ReadAllLines(srSettingsDirectory + "predefined_pokestop_locs.txt").ToList();
            }

            if (File.Exists(configFile) && blOverWriteSettings == false)
            {
                try
                {
                    //if the file exists, load the settings
                    var input = File.ReadAllText(configFile);

                    var jsonSettings = new JsonSerializerSettings();
                    jsonSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                    jsonSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    jsonSettings.DefaultValueHandling = DefaultValueHandling.Populate;

                    settings = JsonConvert.DeserializeObject<GlobalSettings>(input, jsonSettings);
                }
                catch (Newtonsoft.Json.JsonReaderException exception)
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
                new StringEnumConverter { CamelCaseText = true });

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
            get
            {
                return _settings.Auth.AuthType;
            }

            set
            {
                _settings.Auth.AuthType = value;
            }
        }

        double ISettings.DefaultLatitude
        {
            get
            {
                return _settings.DefaultLatitude;
            }

            set
            {
                _settings.DefaultLatitude = value;
            }
        }

        double ISettings.DefaultLongitude
        {
            get
            {
                return _settings.DefaultLongitude;
            }

            set
            {
                _settings.DefaultLongitude = value;
            }
        }

        double ISettings.DefaultAltitude
        {
            get
            {
                return _settings.DefaultAltitude;
            }

            set
            {
                _settings.DefaultAltitude = value;
            }
        }

        string ISettings.PtcPassword
        {
            get
            {
                return _settings.Auth.PtcPassword;
            }

            set
            {
                _settings.Auth.PtcPassword = value;
            }
        }

        string ISettings.PtcUsername
        {
            get
            {
                return _settings.Auth.PtcUsername;
            }

            set
            {
                _settings.Auth.PtcUsername = value;
            }
        }

        string ISettings.GoogleUsername
        {
            get
            {
                return _settings.Auth.GoogleUsername;
            }

            set
            {
                _settings.Auth.GoogleUsername = value;
            }
        }
        string ISettings.GooglePassword
        {
            get
            {
                return _settings.Auth.GooglePassword;
            }

            set
            {
                _settings.Auth.GooglePassword = value;
            }
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
        public double WalkingSpeedInKilometerPerHour => _settings.WalkingSpeedInKilometerPerHour;
        public bool EvolveAllPokemonWithEnoughCandy => _settings.EvolveAllPokemonWithEnoughCandy;
        public bool KeepPokemonsThatCanEvolve => _settings.KeepPokemonsThatCanEvolve;
        public bool TransferDuplicatePokemon => _settings.TransferDuplicatePokemon;
        public bool UseEggIncubators => _settings.UseEggIncubators;
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
        public bool RenameAboveIv => _settings.RenameAboveIv;
        public string RenameTemplate => _settings.RenameTemplate;
        public int AmountOfPokemonToDisplayOnStart => _settings.AmountOfPokemonToDisplayOnStart;
        public bool DumpPokemonStats => _settings.DumpPokemonStats;
        public string TranslationLanguageCode => _settings.TranslationLanguageCode;
        public ICollection<KeyValuePair<ItemId, int>> ItemRecycleFilter => _settings.ItemRecycleFilter;
        public ICollection<PokemonId> PokemonsToEvolve => _settings.PokemonsToEvolve;
        public ICollection<PokemonId> PokemonsNotToTransfer => _settings.PokemonsNotToTransfer;
        public ICollection<PokemonId> PokemonsNotToCatch => _settings.PokemonsToIgnore;
        public Dictionary<PokemonId, TransferFilter> PokemonsTransferFilter => _settings.PokemonsTransferFilter;
        public bool StartupWelcomeDelay => _settings.StartupWelcomeDelay;
        public bool SnipeAtPokestops => _settings.SnipeAtPokestops;
        public int MinPokeballsToSnipe => _settings.MinPokeballsToSnipe;
        public SnipeSettings PokemonToSnipe => _settings.PokemonToSnipe;
        public string SnipeLocationServer => _settings.SnipeLocationServer;
        public int SnipeLocationServerPort => _settings.SnipeLocationServerPort;
        public bool UseSnipeLocationServer => _settings.UseSnipeLocationServer;
        public bool UseTransferIVForSnipe => _settings.UseTransferIVForSnipe;
        public bool SnipeIgnoreUnknownIV => _settings.SnipeIgnoreUnknownIV;
        public int MinDelayBetweenSnipes => _settings.MinDelayBetweenSnipes;
        public int TotalAmountOfPokebalsToKeep => _settings.TotalAmountOfPokebalsToKeep;
        public int TotalAmountOfPotionsToKeep => _settings.TotalAmountOfPotionsToKeep;
        public int TotalAmountOfRevivesToKeep => _settings.TotalAmountOfRevivesToKeep;
        public int irPowerUpPerfectionIV => _settings.irPowerUpPerfectionIV;
    }
}
