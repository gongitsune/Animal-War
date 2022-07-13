using System;
using MyPackages.StateMachine;
using Projects.Scripts.Domains.Animal;
using Projects.Scripts.Domains.Animal.States;
using UnityEngine;
using UnityEngine.AI;

namespace Projects.Scripts.Presenters.Animal
{
    [RequireComponent(typeof(AnimalStateMemory), typeof(NavMeshAgent))]
    public class AnimalCore : MonoBehaviour
    {
        [NonSerialized] public NavMeshAgent Agent;
        public StateMachine<AnimalCore, AnimalStateMemory> StateMachine { get; private set; }

        private void Awake()
        {
            TryGetComponent(out Agent);
            TryGetComponent(out AnimalStateMemory stateMemory);

            StateMachine = new StateMachine<AnimalCore, AnimalStateMemory>(this, stateMemory);
            StateMachine
                .AddTransition<StateIdle, StateMoveBeacon>((int)StateEvent.IdleFinish)
                .AddTransition<StateMoveBeacon, StateIdle>((int)StateEvent.ArriveBeacon)
                .OnStart<StateMoveBeacon>();
        }

        private void Update()
        {
            StateMachine.OnUpdate();
        }
    }
}