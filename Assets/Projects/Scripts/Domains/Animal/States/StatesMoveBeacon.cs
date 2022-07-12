#nullable enable
using MyPackages.StateMachine;
using UnityEngine;

namespace Projects.Scripts.Domains.Animal.States
{
    /// <summary>
    ///     登録されたビーコンまで移動する処理
    /// </summary>
    public class StatesMoveBeacon : StateMachine<AnimalDomain, AnimalStateMemory>.StateBase
    {
        private Transform? _nearBeacon;
        
        public override void OnStart()
        {
            // 近くのビーコン探す
            const float distance = float.PositiveInfinity;
            var data = new { _nearBeacon, distance };
            foreach (var beacon in Memory.Beacons)
            {
                var dis = (Owner.transform.position - beacon.position).sqrMagnitude;
                // if (dis < distance) distance = dis;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}