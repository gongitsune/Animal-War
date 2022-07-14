using MyPackages.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Projects.Scripts.Presenters.Animal
{
    [RequireComponent(typeof(NavMeshAgent))]
    public partial class Animal : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private ImtStateMachine<Animal, StateEvent> _stateMachine;

        // AgentTypeを動物のIDとして使用
        public int AnimalId => _agent.agentTypeID;

        private void Awake()
        {
            TryGetComponent(out _agent);

            _stateMachine = new ImtStateMachine<Animal, StateEvent>(this);
            _stateMachine.AddTransition<MoveBeaconState, IdleState>(StateEvent.ArriveBeacon);
            _stateMachine.AddTransition<IdleState, MoveBeaconState>(StateEvent.EndOccupation);
            _stateMachine.SetStartState<MoveBeaconState>();
        }

        private enum StateEvent
        {
            ArriveBeacon,
            EndOccupation
        }

        private abstract class MyState : ImtStateMachine<Animal, StateEvent>.State
        {
        }
    }
}