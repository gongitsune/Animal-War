using System;
using MyPackages.StateMachine;
using Projects.Scripts.Domains;
using UniRx;
using UnityEngine;

namespace Projects.Scripts.Presenters.Beacon
{
    public partial class Beacon : MonoBehaviour
    {
        [SerializeField] private Memory memory = new();
        [SerializeField] private AnimalKind occupying = AnimalKind.None;

        private readonly ReactiveProperty<bool> _isOccupied = new();
        private ImtStateMachine<Beacon, StateEvent> _stateMachine;

        /// <summary>
        ///     現在占拠している動物のID
        /// </summary>
        public AnimalKind AnimalId
        {
            get => occupying;
            private set => occupying = value;
        }

        public IReadOnlyReactiveProperty<bool> IsOccupied => _isOccupied;

        private void Awake()
        {
            _stateMachine = new ImtStateMachine<Beacon, StateEvent>(this);
            _stateMachine.AddTransition<IdleState, OccupationState>(StateEvent.StartOccupation);
            _stateMachine.AddTransition<OccupationState, IdleState>(StateEvent.ExitOccupation);
            _stateMachine.SetStartState<IdleState>();
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        private enum StateEvent
        {
            StartOccupation,
            ExitOccupation
        }

        private abstract class MyState : ImtStateMachine<Beacon, StateEvent>.State
        {
        }

        [Serializable]
        private class Memory
        {
            public AnimalKind enteredAnimal;
            public float occupationSpan = 5f;
        }
    }
}