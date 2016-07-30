﻿#region using directives

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;
using PokemonGo.RocketAPI;
using POGOProtos.Inventory.Item;
using PoGo.NecroBot.Logic.PoGoUtils;

#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public class EvolvePokemonTask
    {
        private static DateTime _lastLuckyEggTime;

        public static async Task Execute(ISession session, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var pokemonPowerUpTask = await session.Inventory.GetPokemonToPowerUp(session.LogicSettings.PokemonsToEvolve);
            var pokemonPowerUp = pokemonPowerUpTask.ToList();

            Logging.Logger.Write("Starting Power UP Pokemon...", Logging.LogLevel.Self, ConsoleColor.Yellow);

            foreach (var vrPokemon in pokemonPowerUp)
            {
                Logging.Logger.Write($"Upgrading Pokemon {vrPokemon.PokemonId} , Up Count: {vrPokemon.NumUpgrades} , IV: {PokemonInfo.CalculatePokemonPerfection(vrPokemon)} CP: {vrPokemon.Cp} , CP Multi: {vrPokemon.CpMultiplier} , CP Add Multi: {vrPokemon.AdditionalCpMultiplier}.....", Logging.LogLevel.Self, ConsoleColor.DarkYellow);

                while (true)
                {
                    var evolveResponse = await session.Client.Inventory.UpgradePokemon(vrPokemon.Id);
                    if (evolveResponse.Result != POGOProtos.Networking.Responses.UpgradePokemonResponse.Types.Result.Success)
                        break;
             
                }
            }

            var pokemonToEvolveTask = await session.Inventory.GetPokemonToEvolve(session.LogicSettings.PokemonsToEvolve);
            var pokemonToEvolve = pokemonToEvolveTask.ToList();

            if (pokemonToEvolve.Any())
            {
                var inventoryContent = await session.Inventory.GetItems();

                var luckyEggs = inventoryContent.Where(p => p.ItemId == ItemId.ItemLuckyEgg);
                var luckyEgg = luckyEggs.FirstOrDefault();

                //maybe there can be a warning message as an else condition of luckyEgg checks, like; 
                //"There is no Lucky Egg, so, your UseLuckyEggsMinPokemonAmount setting bypassed."
                if (session.LogicSettings.UseLuckyEggsWhileEvolving && luckyEgg != null && luckyEgg.Count > 0)
                {
                    if (pokemonToEvolve.Count >= session.LogicSettings.UseLuckyEggsMinPokemonAmount)
                    {
                        await UseLuckyEgg(session);
                    }
                    else
                    {
                        // Wait until we have enough pokemon
                        return;
                    }
                }

                foreach (var pokemon in pokemonToEvolve)
                {
                    // no cancellationToken.ThrowIfCancellationRequested here, otherwise the lucky egg would be wasted.
                    var evolveResponse = await session.Client.Inventory.EvolvePokemon(pokemon.Id);

                    session.EventDispatcher.Send(new PokemonEvolveEvent
                    {
                        Id = pokemon.PokemonId,
                        Exp = evolveResponse.ExperienceAwarded,
                        Result = evolveResponse.Result
                    });
                }
            }
        }

        public static async Task UseLuckyEgg(ISession session)
        {
            var inventoryContent = await session.Inventory.GetItems();

            var luckyEggs = inventoryContent.Where(p => p.ItemId == ItemId.ItemLuckyEgg);
            var luckyEgg = luckyEggs.FirstOrDefault();

            if (_lastLuckyEggTime.AddMinutes(30).Ticks > DateTime.Now.Ticks)
                return;

            _lastLuckyEggTime = DateTime.Now;
            await session.Client.Inventory.UseItemXpBoost();
            await session.Inventory.RefreshCachedInventory();
            session.EventDispatcher.Send(new UseLuckyEggEvent { Count = luckyEgg.Count });
            DelayingUtils.Delay(session.LogicSettings.DelayBetweenPokemonCatch, 2000);
        }
    }
}
