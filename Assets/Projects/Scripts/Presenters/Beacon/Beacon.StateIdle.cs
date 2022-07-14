using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace Projects.Scripts.Presenters.Beacon
{
    public partial class Beacon
    {
        private class IdleState: MyState
        {
            
        }
        
        private class OccupationState: MyState
        {
            private float _elapsedTime;
            // private NavMeshAgent 
            private readonly CompositeDisposable _disposable = new();

            protected internal override void Enter()
            {
                Context.OnTriggerEnterAsObservable().Subscribe(col =>
                {
                    
                });
                Context.OnTriggerExitAsObservable().Subscribe(col =>
                {
                    if (col.CompareTag("Animal"))
                        StateMachine.SendEvent(StateEvent.ExitOccupation);
                }).AddTo(_disposable);
            }

            protected internal override void Update()
            {
                _elapsedTime += Time.deltaTime;
                
                if (!(_elapsedTime > 5)) return;
                // Context._occupiedAnimal.Value = 
                StateMachine.SendEvent(StateEvent.ExitOccupation);
            }

            protected internal override void Exit()
            {
                _disposable.Dispose();
            }
        }
    }
}