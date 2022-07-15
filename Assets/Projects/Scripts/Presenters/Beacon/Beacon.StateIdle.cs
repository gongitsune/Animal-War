using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Projects.Scripts.Presenters.Beacon
{
    public partial class Beacon
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private class IdleState : MyState
        {
            private CompositeDisposable _disposable;

            protected internal override void Enter()
            {
                _disposable = new CompositeDisposable();
                Context.OnTriggerEnterAsObservable().Subscribe(col =>
                {
                    if (!col.CompareTag("Animal")) return;

                    col.TryGetComponent(out Animal.Animal animal);
                    Context.memory.enteredAnimal = animal.Kind;
                    StateMachine.SendEvent(StateEvent.StartOccupation);
                }).AddTo(_disposable);
            }

            protected internal override void Exit()
            {
                _disposable.Dispose();
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class OccupationState : MyState
        {
            private CompositeDisposable _disposable;
            private float _elapsedTime;

            protected internal override void Enter()
            {
                _disposable = new CompositeDisposable();
                _elapsedTime = 0;

                Context.OnTriggerExitAsObservable().Subscribe(col =>
                {
                    if (col.CompareTag("Animal")) StateMachine.SendEvent(StateEvent.ExitOccupation);
                }).AddTo(_disposable);
            }

            protected internal override void Update()
            {
                _elapsedTime += Time.deltaTime;

                if (_elapsedTime < Context.memory.occupationSpan) return;
                Context._isOccupied.Value = true;
                Context.AnimalId = Context.memory.enteredAnimal;
                StateMachine.SendEvent(StateEvent.ExitOccupation);
            }

            protected internal override void Exit()
            {
                _disposable.Dispose();
            }
        }
    }
}