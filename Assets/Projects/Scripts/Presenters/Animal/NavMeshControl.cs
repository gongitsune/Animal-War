using Projects.Scripts.Domains.Animal;
using UnityEngine;
using UnityEngine.AI;

namespace Projects.Scripts.Presenters.Animal
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshControl : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private NavMeshAgent _agent;
        private AnimalDomain _domain;

        private void Awake()
        {
            TryGetComponent(out _agent);
            TryGetComponent(out _domain);
            
            _agent.SetDestination(target.position);
        }

        private void FixedUpdate()
        {
            _domain.IsMove.Value = _agent.velocity.sqrMagnitude > 0.1f;
        }
    }
}