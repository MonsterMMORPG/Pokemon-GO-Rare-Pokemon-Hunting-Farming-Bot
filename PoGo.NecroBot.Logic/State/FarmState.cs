﻿#region using directives

using System.Threading;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Logging;
using System;

#endregion

namespace PoGo.NecroBot.Logic.State
{
    public class FarmState : IState
    {
        public async Task<IState> Execute(ISession session, CancellationToken cancellationToken)
        {
<<<<<<< HEAD
            if (session.LogicSettings.TransferDuplicatePokemon)
=======

            if (session.LogicSettings.EvolveAllPokemonAboveIv || session.LogicSettings.EvolveAllPokemonWithEnoughCandy)
>>>>>>> refs/remotes/upstream/master
            {
                await TransferDuplicatePokemonTask.Execute(session, cancellationToken);
            }

            if (session.LogicSettings.TransferDuplicatePokemon)
            {
                await DisplayPokemonStatsTask.WriteHighest(session);
            }

            if (session.LogicSettings.EvolveAllPokemonAboveIv || session.LogicSettings.EvolveAllPokemonWithEnoughCandy)
            {
                await EvolvePokemonTask.Execute(session, cancellationToken);
            }

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

            await GetPokeDexCount.Execute(session, cancellationToken);

            if (session.LogicSettings.RenamePokemon)
            {
                await RenamePokemonTask.Execute(session, cancellationToken);
            }

            if (session.LogicSettings.AutoFavoritePokemon)
            {
                await FavoritePokemonTask.Execute(session, cancellationToken);
            }

            await RecycleItemsTask.Execute(session, cancellationToken);

            if (session.LogicSettings.UseEggIncubators)
            {
                await UseIncubatorsTask.Execute(session, cancellationToken);
            }

            if (session.LogicSettings.UseGpxPathing)
            {
                await FarmPokestopsGpxTask.Execute(session, cancellationToken);
            }
            else
            {
                await FarmPokestopsTask.ExeCuteMyFarm(session, cancellationToken);
            }

            return this;
        }
    }
}