using System;
using Projects.Scripts.Domains;
using Projects.Scripts.Presenters.Common;
using UnityEngine;
using UnityEngine.AI;

namespace Projects.Scripts.Presenters.Animal
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
    public class Animal : AnimalBase
    {
        [SerializeField] private AnimalKind kind;

        private NavMeshAgent _agent;
        private Rigidbody _rigid;
        public AnimalBase Target { get; set; }
        public AnimalKind Kind => kind;

        private void Awake()
        {
            TryGetComponent(out _agent);
            TryGetComponent(out _rigid);
            
            _agent.SetDestination(FindObjectOfType<Beacon.Beacon>().transform.position);
        }

        private void FixedUpdate()
        {
            Debug.Log(_agent.hasPath);
        }
    }
}