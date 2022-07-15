using System;
using UniBT;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Action = UniBT.Action;

namespace Projects.Scripts.Presenters.Animal.Behaviour
{
    [Serializable]
    public class FollowAction: Action
    {
        [SerializeField] private float goalThreshold = 5;
        
        private NavMeshAgent _agent;
        private Animal _core;
        private Transform _transform;
        
        public override void Awake()
        {
            _transform = gameObject.transform;
            
            _transform.TryGetComponent(out _agent);
            _transform.TryGetComponent(out _core);
        }

        protected override Status OnUpdate()
        {
            if (!_agent.hasPath)
            {
                return Status.Failure;
            }

            var goalDis = (_transform.position - _core.Target.transform.position).sqrMagnitude;
            return goalDis < math.pow(goalThreshold, 2) ? Status.Success : Status.Running;
        }
    }
}