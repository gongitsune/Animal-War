using UnityEngine;
using UnityEngine.AI;

namespace UniBT.Examples.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Transform player;

        private NavMeshAgent _navMeshAgent;
        private Rigidbody _rigid;
        public float Hate { get; private set; }

        public bool Attacking { get; private set; }

        private void Awake()
        {
            TryGetComponent(out _rigid);
            TryGetComponent(out _navMeshAgent);
        }

        private void Update()
        {
            var distance = Vector3.Distance(transform.position, player.position);
            Hate = distance switch
            {
                < 8 => 20,
                < 12 => 8,
                _ => 0
            };
        }

        private void OnCollisionStay(Collision other)
        {
            // TODO other.collider.name cause GC.Alloc by Object.GetName
            if (Attacking && other.collider.name == "Ground" && Mathf.Abs(_rigid.velocity.y) < 0.1) CancelAttack();
        }

        public void Attack(float force)
        {
            Attacking = true;
            _navMeshAgent.enabled = false;
            _rigid.isKinematic = false;
            _rigid.AddForce(Vector3.up * force, ForceMode.Impulse);
        }

        public void CancelAttack()
        {
            _navMeshAgent.enabled = true;
            _rigid.isKinematic = true;
            Attacking = false;
        }
    }
}