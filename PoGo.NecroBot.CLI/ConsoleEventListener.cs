#region using directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Responses;

#endregion

namespace PoGo.NecroBot.CLI
{
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    internal class ConsoleEventListener
    {
        private static void HandleEvent(ProfileEvent profileEvent, ISession session)
        {
            Logger.Write(session.Translation.GetTranslation(TranslationString.EventProfileLogin,
                profileEvent.Profile.PlayerData.Username ?? ""));
        }

        private static void HandleEvent(ErrorEvent errorEvent, ISession session)
        {
            Logger.Write(errorEvent.ToString(), LogLevel.Error, force: true);
        }

        private static void HandleEvent(NoticeEvent noticeEvent, ISession session)
        {
            Logger.Write(noticeEvent.ToString());
        }

        private static void HandleEvent(WarnEvent warnEvent, ISession session)
        {
            Logger.Write(warnEvent.ToString(), LogLevel.Warning);
            // If the event requires no input return.
            if (!warnEvent.RequireInput) return;
            // Otherwise require input.
            Logger.Write(session.Translation.GetTranslation(TranslationString.RequireInputText));
            Console.ReadKey();
        }

        private static void HandleEvent(UseLuckyEggEvent useLuckyEggEvent, ISession session)
        {
            Logger.Write(session.Translation.GetTranslation(TranslationString.EventUsedLuckyEgg, useLuckyEggEvent.Count),
                LogLevel.Egg);
        }

        private static void HandleEvent(PokemonEvolveEvent pokemonEvolveEvent, ISession session)
        {
            Logger.Write(pokemonEvolveEvent.Result == EvolvePokemonResponse.Types.Result.Success
                ? session.Translation.GetTranslation(TranslationString.EventPokemonEvolvedSuccess, pokemonEvolveEvent.Id, pokemonEvolveEvent.Exp)
                : session.Translation.GetTranslation(TranslationString.EventPokemonEvolvedFailed, pokemonEvolveEvent.Id, pokemonEvolveEvent.Result,
                    pokemonEvolveEvent.Id),
                LogLevel.Evolve);
        }

        private static void HandleEvent(TransferPokemonEvent transferPokemonEvent, ISession session)
        {
            Logger.Write(
                session.Translation.GetTranslation(TranslationString.EventPokemonTransferred, 
                transferPokemonEvent.Id, 
                transferPokemonEvent.Cp,
                transferPokemonEvent.Perfection.ToString("0.00"), 
                transferPokemonEvent.BestCp, 
                transferPokemonEvent.BestPerfection.ToString("0.00"), 
                transferPokemonEvent.FamilyCandies),
                LogLevel.Transfer);
        }

        private static void HandleEvent(ItemRecycledEvent itemRecycledEvent, ISession session)
        {
            Logger.Write(session.Translation.GetTranslation(TranslationString.EventItemRecycled, itemRecycledEvent.Count, itemRecycledEvent.Id),
                LogLevel.Recycling);
        }

        private static void HandleEvent(EggIncubatorStatusEvent eggIncubatorStatusEvent, ISession session)
        {
            Logger.Write(eggIncubatorStatusEvent.WasAddedNow
                ? session.Translation.GetTranslation(TranslationString.IncubatorPuttingEgg, eggIncubatorStatusEvent.KmRemaining)
                : session.Translation.GetTranslation(TranslationString.IncubatorStatusUpdate, eggIncubatorStatusEvent.KmRemaining),
                LogLevel.Egg);
        }

        private static void HandleEvent(EggHatchedEvent eggHatchedEvent, ISession session)
        {
            Logger.Write(session.Translation.GetTranslation(TranslationString.IncubatorEggHatched,
                eggHatchedEvent.PokemonId.ToString(), eggHatchedEvent.Level, eggHatchedEvent.Cp, eggHatchedEvent.MaxCp, eggHatchedEvent.Perfection),
                LogLevel.Egg);
        }

        private static void HandleEvent(FortUsedEvent fortUsedEvent, ISession session)
        {
            var itemString = fortUsedEvent.InventoryFull
                ? session.Translation.GetTranslation(TranslationString.InvFullPokestopLooting)
                : fortUsedEvent.Items;
            Logger.Write(
                session.Translation.GetTranslation(TranslationString.EventFortUsed, fortUsedEvent.Name, fortUsedEvent.Exp, fortUsedEvent.Gems,
                    itemString),
                LogLevel.Pokestop);
        }

        private static void HandleEvent(FortFailedEvent fortFailedEvent, ISession session)
        {
            Logger.Write(
                session.Translation.GetTranslation(TranslationString.EventFortFailed, fortFailedEvent.Name, fortFailedEvent.Try, fortFailedEvent.Max),
                LogLevel.Pokestop, ConsoleColor.DarkRed);
        }

        private static void HandleEvent(FortTargetEvent fortTargetEvent, ISession session)
        {
            Logger.Write(
                session.Translation.GetTranslation(TranslationString.EventFortTargeted, fortTargetEvent.Name,
                    Math.Round(fortTargetEvent.Distance)),
                LogLevel.Info, ConsoleColor.DarkRed);
        }

        private static void HandleEvent(PokemonCaptureEvent pokemonCaptureEvent, ISession session)
        {
            Func<ItemId, string> returnRealBallName = a =>
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (a)
                {
                    case ItemId.ItemPokeBall:
                        return session.Translation.GetTranslation(TranslationString.Pokeball);
                    case ItemId.ItemGreatBall:
                        return session.Translation.GetTranslation(TranslationString.GreatPokeball);
                    case ItemId.ItemUltraBall:
                        return session.Translation.GetTranslation(TranslationString.UltraPokeball);
                    case ItemId.ItemMasterBall:
                        return session.Translation.GetTranslation(TranslationString.MasterPokeball);
                    default:
                        return session.Translation.GetTranslation(TranslationString.CommonWordUnknown);
                }
            };

            var catchType = pokemonCaptureEvent.CatchType;

            string strStatus;
            switch (pokemonCaptureEvent.Status)
            {
                case CatchPokemonResponse.Types.CatchStatus.CatchError:
                    strStatus = session.Translation.GetTranslation(TranslationString.CatchStatusError);
                    break;
                case CatchPokemonResponse.Types.CatchStatus.CatchEscape:
                    strStatus = session.Translation.GetTranslation(TranslationString.CatchStatusEscape);
                    break;
                case CatchPokemonResponse.Types.CatchStatus.CatchFlee:
                    strStatus = session.Translation.GetTranslation(TranslationString.CatchStatusFlee);
                    break;
                case CatchPokemonResponse.Types.CatchStatus.CatchMissed:
                    strStatus = session.Translation.GetTranslation(TranslationString.CatchStatusMissed);
                    break;
                case CatchPokemonResponse.Types.CatchStatus.CatchSuccess:
                    strStatus = session.Translation.GetTranslation(TranslationString.CatchStatusSuccess);
                    break;
                default:
                    strStatus = pokemonCaptureEvent.Status.ToString();
                    break;
            }

            var catchStatus = pokemonCaptureEvent.Attempt > 1
                ? session.Translation.GetTranslation(TranslationString.CatchStatusAttempt, strStatus, pokemonCaptureEvent.Attempt)
                : session.Translation.GetTranslation(TranslationString.CatchStatus, strStatus);

            var familyCandies = pokemonCaptureEvent.FamilyCandies > 0
                ? session.Translation.GetTranslation(TranslationString.Candies, pokemonCaptureEvent.FamilyCandies)
                : "";

            Logger.Write(
                session.Translation.GetTranslation(TranslationString.EventPokemonCapture, catchStatus, catchType, pokemonCaptureEvent.Id,
                    pokemonCaptureEvent.Level, pokemonCaptureEvent.Cp, pokemonCaptureEvent.MaxCp, pokemonCaptureEvent.Perfection.ToString("0.00"), pokemonCaptureEvent.Probability,
                    pokemonCaptureEvent.Distance.ToString("F2"),
                    returnRealBallName(pokemonCaptureEvent.Pokeball), pokemonCaptureEvent.BallAmount, familyCandies), LogLevel.Caught);
        }

        private static void HandleEvent(NoPokeballEvent noPokeballEvent, ISession session)
        {
            Logger.Write(session.Translation.GetTranslation(TranslationString.EventNoPokeballs, noPokeballEvent.Id, noPokeballEvent.Cp),
                LogLevel.Caught);
        }

        private static void HandleEvent(UseBerryEvent useBerryEvent, ISession session)
        {
            string strBerry;
            switch (useBerryEvent.BerryType)
            {
                case ItemId.ItemRazzBerry:
                    strBerry = session.Translation.GetTranslation(TranslationString.ItemRazzBerry);
                    break;
                default:
                    strBerry = useBerryEvent.BerryType.ToString();
                    break;
            }

            Logger.Write(session.Translation.GetTranslation(TranslationString.EventUseBerry, strBerry, useBerryEvent.Count),
                LogLevel.Berry);
        }

        private static void HandleEvent(SnipeEvent snipeEvent, ISession session)
        {
            Logger.Write(snipeEvent.ToString(), LogLevel.Sniper);
        }

        private static void HandleEvent(SnipeScanEvent snipeScanEvent, ISession session)
        {
            Logger.Write(snipeScanEvent.PokemonId == PokemonId.Missingno
                ? session.Translation.GetTranslation(TranslationString.SnipeScan,
                    $"{snipeScanEvent.Bounds.Latitude},{snipeScanEvent.Bounds.Longitude}")
                : session.Translation.GetTranslation(TranslationString.SnipeScanEx, snipeScanEvent.PokemonId,
                    snipeScanEvent.Iv > 0 ? snipeScanEvent.Iv.ToString(CultureInfo.InvariantCulture) : "unknown",
                    $"{snipeScanEvent.Bounds.Latitude},{snipeScanEvent.Bounds.Longitude}"), LogLevel.Sniper);
        }

        private static void HandleEvent(DisplayHighestsPokemonEvent displayHighestsPokemonEvent, ISession session)
        {
            string strHeader;
            //PokemonData | CP | IV | Level | MOVE1 | MOVE2 | Candy
            switch (displayHighestsPokemonEvent.SortedBy)
            {
                case "Level":
                    strHeader = session.Translation.GetTranslation(TranslationString.DisplayHighestsLevelHeader);
                    break;
                case "IV":
                    strHeader = session.Translation.GetTranslation(TranslationString.DisplayHighestsPerfectHeader);
                    break;
                case "CP":
                    strHeader = session.Translation.GetTranslation(TranslationString.DisplayHighestsCpHeader);
                    break;
                case "MOVE1":
                    strHeader = session.Translation.GetTranslation(TranslationString.DisplayHighestMove1Header);
                    break;
                case "MOVE2":
                    strHeader = session.Translation.GetTranslation(TranslationString.DisplayHighestMove2Header);
                    break;
                case "Candy":
                    strHeader = session.Translation.GetTranslation(TranslationString.DisplayHighestCandy);
                    break;
                default:
                    strHeader = session.Translation.GetTranslation(TranslationString.DisplayHighestsHeader);
                    break;
            }
            var strPerfect = session.Translation.GetTranslation(TranslationString.CommonWordPerfect);
            var strName = session.Translation.GetTranslation(TranslationString.CommonWordName).ToUpper();

            Logger.Write($"====== {strHeader} ======", LogLevel.Info, ConsoleColor.Yellow);
            foreach (var pokemon in displayHighestsPokemonEvent.PokemonList)
                Logger.Write(
                    $"# CP {pokemon.Item1.Cp.ToString().PadLeft(4, ' ')}/{pokemon.Item2.ToString().PadLeft(4, ' ')} | ({pokemon.Item3.ToString("0.00")}% {strPerfect})\t| Lvl {pokemon.Item4.ToString("00")}\t {strName}: {pokemon.Item1.PokemonId.ToString().PadRight(10, ' ')}\t MOVE1: {pokemon.Item5.ToString().PadRight(20, ' ')} MOVE2: {pokemon.Item6.ToString().PadRight(20, ' ')} Candy: {pokemon.Item7}",
                    LogLevel.Info, ConsoleColor.Yellow);
        }

        private static void HandleEvent(EvolveCountEvent evolveCountEvent, ISession session )
        {
            Logger.Write(session.Translation.GetTranslation(TranslationString.PkmPotentialEvolveCount, evolveCountEvent.Evolves) + 
                ( session.LogicSettings.UseLuckyEggsWhileEvolving ? 
                    $" | {session.LogicSettings.UseLuckyEggsMinPokemonAmount} required for mass evolving" 
                    : "" ), LogLevel.Update, ConsoleColor.White );
        }

        private static void HandleEvent(UpdateEvent updateEvent, ISession session)
        {
            Logger.Write(updateEvent.ToString(), LogLevel.Update);
        }

        internal void Listen(IEvent evt, ISession session)
        {
            dynamic eve = evt;

            try
            {
                HandleEvent(eve, session);
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }
    }
}
