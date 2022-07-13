#nullable enable
using MyPackages.StateMachine;
using Projects.Scripts.Presenters.Animal;
using UnityEngine;

namespace Projects.Scripts.Domains.Animal.States
{
    /// <summary>
    ///     登録されたビーコンまで移動する処理
    /// </summary>
    public class StateMoveBeacon : StateMachine<AnimalCore, AnimalStateMemory>.StateBase
    {
        private Transform? _nearBeacon;

        public override void OnStart()
        {
            // 近くのビーコン探す
            var distance = float.PositiveInfinity;
            foreach (var beacon in Memory.Beacons)
            {
                var dis = (Owner.transform.position - beacon.position).sqrMagnitude;
                if (dis > distance) continue;
                distance = dis;
                _nearBeacon = beacon;
            }
        }

        public override void OnUpdate()
        {
            var agent = Owner.Agent;
            agent.SetDestination(_nearBeacon!.position);
            if ((_nearBeacon.transform.position - Owner.transform.position).sqrMagnitude <=
                Mathf.Pow(agent.stoppingDistance + 1, 2))
                StateMachine.DispatchEvent((int)StateEvent.ArriveBeacon);
        }
    }
}