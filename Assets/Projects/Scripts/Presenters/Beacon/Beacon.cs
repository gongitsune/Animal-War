using MyPackages.StateMachine;
using UniRx;
using UnityEngine;

namespace Projects.Scripts.Presenters.Beacon
{
    public partial class Beacon : MonoBehaviour
    {
        private readonly ReactiveProperty<int> _occupiedAnimal = new(-1);
        private ImtStateMachine<Beacon, StateEvent> _stateMachine;

        /// <summary>
        ///     現在占拠している動物のID
        ///     <remarks>占拠していない場合は-1</remarks>
        /// </summary>
        public IReadOnlyReactiveProperty<int> OccupiedAnimal => _occupiedAnimal;

        private void Awake()
        {
            _stateMachine = new ImtStateMachine<Beacon, StateEvent>(this);
            _stateMachine.AddTransition<IdleState, OccupationState>(StateEvent.StartOccupation);
            _stateMachine.AddTransition<OccupationState, IdleState>(StateEvent.ExitOccupation);
        }

        private enum StateEvent
        {
            StartOccupation,
            ExitOccupation
        }

        private abstract class MyState : ImtStateMachine<Beacon, StateEvent>.State
        {
        }
    }
}