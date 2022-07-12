using UniRx;
using UnityEngine;

namespace Projects.Scripts.Presenters.Common
{
    public class GroundedChecker : MonoBehaviour
    {
        [SerializeField] private Transform rayPoint;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private float rayRadius;
        [SerializeField] private bool isShowGizmos;

        private readonly ReactiveProperty<bool> _isGrounded = new(false);
        public IReadOnlyReactiveProperty<bool> IsGrounded => _isGrounded;

        private void FixedUpdate()
        {
            _isGrounded.Value = Physics.CheckSphere(rayPoint.position, rayRadius, obstacleLayer);
        }

        private void OnDrawGizmos()
        {
            if (!isShowGizmos) return;
            if (Application.isPlaying)
                Gizmos.color = _isGrounded.Value ? Color.green : Color.red;
            else
                Gizmos.color = Physics.CheckSphere(rayPoint.position, rayRadius, obstacleLayer)
                    ? Color.green
                    : Color.red;
            Gizmos.DrawWireSphere(rayPoint.position, rayRadius);
        }
    }
}