using UniRx;
using UniRx.Triggers;

namespace Projects.Scripts.Presenters.Animal
{
    public partial class Animal
    {
        private class MoveBeaconState : MyState
        {
            private readonly Beacon.Beacon[] _beacons;
            private CompositeDisposable _disposable;
            private Beacon.Beacon _targetBeacon;

            public MoveBeaconState()
            {
                _beacons = FindObjectsOfType<Beacon.Beacon>();
            }

            protected internal override void Enter()
            {
                _disposable = new CompositeDisposable();

                var dis = float.PositiveInfinity;
                foreach (var beacon in _beacons)
                {
                    var tmp = (beacon.transform.position - Context.transform.position).sqrMagnitude;
                    if (beacon.AnimalId == Context.kind || tmp > dis) continue;

                    dis = tmp;
                    _targetBeacon = beacon;
                }

                Context._agent.SetDestination(_targetBeacon.transform.position);

                Context.OnTriggerEnterAsObservable().Subscribe(col =>
                {
                    if (!col.CompareTag("Beacon")) return;

                    Context.memory.targetBeacon = _targetBeacon;
                    StateMachine.SendEvent(StateEvent.ArriveBeacon);
                }).AddTo(_disposable);
            }

            protected internal override void Exit()
            {
                _disposable.Dispose();
            }
        }
    }
}