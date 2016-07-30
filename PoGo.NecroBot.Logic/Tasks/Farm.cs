#region using directives

using System.Threading;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;

#endregion

namespace PoGo.NecroBot.Logic.Service
{
    public interface IFarm
    {
        void Run(CancellationToken cancellationToken);
    }

    public class Farm : IFarm
    {
        private readonly ISession _session;

        public Farm(ISession session)
        {
            _session = session;
        }

        public void Run(CancellationToken cancellationToken)
        {
            if (_session.LogicSettings.TransferDuplicatePokemon)
            {
<<<<<<< HEAD
                TransferDuplicatePokemonTask.Execute(_session, cancellationToken).Wait();
            }

            if (_session.LogicSettings.EvolveAllPokemonAboveIv || _session.LogicSettings.EvolveAllPokemonWithEnoughCandy)
            {
                EvolvePokemonTask.Execute(_session, cancellationToken).Wait();
=======
                EvolvePokemonTask.Execute(_session, cancellationToken).Wait(cancellationToken);
            }
            if (_session.LogicSettings.AutomaticallyLevelUpPokemon)
            {
                LevelUpPokemonTask.Execute(_session, cancellationToken).Wait(cancellationToken);
            }
            if (_session.LogicSettings.TransferDuplicatePokemon)
            {
                TransferDuplicatePokemonTask.Execute(_session, cancellationToken).Wait(cancellationToken);
>>>>>>> refs/remotes/upstream/master
            }

            if (_session.LogicSettings.RenamePokemon)
            {
                RenamePokemonTask.Execute(_session, cancellationToken).Wait(cancellationToken);
            }

            RecycleItemsTask.Execute(_session, cancellationToken).Wait(cancellationToken);

            if (_session.LogicSettings.UseEggIncubators)
            {
                UseIncubatorsTask.Execute(_session, cancellationToken).Wait(cancellationToken);
            }

            if (_session.LogicSettings.UseGpxPathing)
            {
                FarmPokestopsGpxTask.Execute(_session, cancellationToken).Wait(cancellationToken);
            }
            else
            {
<<<<<<< HEAD
                FarmPokestopsTask.ExeCuteMyFarm(_session, cancellationToken).Wait();
=======
                FarmPokestopsTask.Execute(_session, cancellationToken).Wait(cancellationToken);
>>>>>>> refs/remotes/upstream/master
            }
        }
    }
}