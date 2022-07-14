using UnityEngine;

namespace Projects.Scripts.Presenters.Animal
{
    // [RequireComponent(typeof(AnimalStateMemory), typeof(NavMeshAgent))]
    public class AnimalCore : MonoBehaviour
    {
        // [SerializeField] [ReadOnly] private string currentState;
        // [NonSerialized] public NavMeshAgent Agent;
        // public StateMachine<AnimalCore, AnimalStateMemory> StateMachine { get; private set; }
        //
        // private void Awake()
        // {
        //     TryGetComponent(out Agent);
        //     TryGetComponent(out AnimalStateMemory stateMemory);
        //
        //     StateMachine = new StateMachine<AnimalCore, AnimalStateMemory>(this, stateMemory)
        //         .AddTransition<StateIdle, StateMoveBeacon>((int)StateEvent.IdleFinish)
        //         .AddTransition<StateMoveBeacon, StateIdle>((int)StateEvent.ArriveBeacon)
        //         .OnStart<StateMoveBeacon>();
        //
        //     StateMachine.CurrentState.Subscribe(state => currentState = state.GetType().ToString().Split(".").Last());
        // }
        //
        // private void Update()
        // {
        //     StateMachine.OnUpdate();
        // }
    }
}