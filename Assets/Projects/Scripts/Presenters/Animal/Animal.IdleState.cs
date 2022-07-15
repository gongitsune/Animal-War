using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Projects.Scripts.Presenters.Animal
{
    public partial class Animal
    {
        private class IdleState : MyState
        {
            private CompositeDisposable _disposable;

            protected internal override void Enter()
            {
                _disposable = new CompositeDisposable();

                Debug.Log("Enter Idle state");
                Context.memory.targetBeacon.IsOccupied.Subscribe(v =>
                {
                    if (v) StateMachine.SendEvent(StateEvent.EndOccupation);
                }).AddTo(_disposable);
                Context.memory.targetBeacon.OnTriggerExitAsObservable().Subscribe(col =>
                {
                    if (col.gameObject == Context.gameObject) StateMachine.SendEvent(StateEvent.EndOccupation);
                }).AddTo(_disposable);
            }

            protected internal override void Exit()
            {
                _disposable.Dispose();
            }
        }
    }
}