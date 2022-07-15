using System;
using MyPackages.StateMachine;
using Projects.Scripts.Domains;
using UnityEngine;
using UnityEngine.AI;

namespace Projects.Scripts.Presenters.Animal
{
    [RequireComponent(typeof(NavMeshAgent))]
    public partial class Animal : MonoBehaviour
    {
        [SerializeField] private AnimalKind kind;
        [SerializeField] private Memory memory = new();

        private NavMeshAgent _agent;

        private ImtStateMachine<Animal, StateEvent> _stateMachine;
        public AnimalKind Kind => kind;

        private void Awake()
        {
            TryGetComponent(out _agent);

            _stateMachine = new ImtStateMachine<Animal, StateEvent>(this);
            _stateMachine.AddTransition<MoveBeaconState, IdleState>(StateEvent.ArriveBeacon);
            _stateMachine.AddTransition<IdleState, MoveBeaconState>(StateEvent.EndOccupation);
            _stateMachine.AddTransition<MoveBeaconState, AttackState>(StateEvent.StartAttack);
            _stateMachine.AddTransition<IdleState, AttackState>(StateEvent.StartAttack);
            _stateMachine.SetStartState<MoveBeaconState>();
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        private enum StateEvent
        {
            ArriveBeacon,
            EndOccupation,
            StartAttack
        }

        private abstract class MyState : ImtStateMachine<Animal, StateEvent>.State
        {
        }

        [Serializable]
        private class Memory
        {
            public Beacon.Beacon targetBeacon;
        }
    }
}