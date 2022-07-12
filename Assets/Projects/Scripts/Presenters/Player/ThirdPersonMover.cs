using Cinemachine;
using Projects.Scripts.Applications.Input;
using Projects.Scripts.Domains.Player;
using Projects.Scripts.Presenters.Common;
using UniRx;
using UnityEngine;

namespace Projects.Scripts.Presenters.Player
{
    /// <summary>
    ///     三人称視点に対応した移動コンポーネント
    /// </summary>
    [RequireComponent(typeof(GroundedChecker))]
    [RequireComponent(typeof(Rigidbody))]
    public class ThirdPersonMover : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera playerCamera;
        [SerializeField] private PlayerConfig playerConfig;


        private Animator _animator;
        private Vector3 _currentMoveVelocity = Vector3.zero;
        private PlayerDomain _domain;
        private GroundedChecker _groundedChecker;
        private PlayerInputUseCase _input;
        private Rigidbody _rigid;

        private void Awake()
        {
            TryGetComponent(out _rigid);
            TryGetComponent(out _groundedChecker);
            TryGetComponent(out _animator);
            TryGetComponent(out _domain);

            _input = new PlayerInputUseCase
            {
                Enabled = true
            };
            _input.OnJump.Subscribe(_ => Jump(_groundedChecker.IsGrounded.Value)).AddTo(this);
        }

        private void FixedUpdate()
        {
            MoveAndRotate(playerCamera.transform, _input.Move, _groundedChecker.IsGrounded.Value);
        }

        private void MoveAndRotate(Transform cam, Vector2 value, bool grounded)
        {
            // 入力がない場合は無視
            var preMoved = _domain.IsMove.Value;
            _domain.IsMove.Value = value.sqrMagnitude > 0.1f;
            if (preMoved && !_domain.IsMove.Value && grounded)
                _rigid.velocity = new Vector3(0, _rigid.velocity.y, 0);
            if (!_domain.IsMove.Value) return;

            // 水平面のベクトルを計算
            var camForward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1))
                .normalized;
            var moveForward = camForward * value.y + cam.right.normalized * value.x;

            // 回転
            _rigid.rotation = Quaternion.RotateTowards(_rigid.rotation, Quaternion.LookRotation(moveForward),
                playerConfig.RotateSpeed * Time.fixedDeltaTime);

            // アニメーターの判定
            if (_animator.applyRootMotion) return;

            // 移動
            var velocity = _rigid.velocity;
            if (grounded)
                _rigid.velocity = Vector3.SmoothDamp(velocity,
                    moveForward * (playerConfig.GroundSpeed * Time.fixedDeltaTime) +
                    new Vector3(0, velocity.y, 0), ref _currentMoveVelocity, 0.1f);
            else
                _rigid.velocity = Vector3.MoveTowards(velocity,
                    moveForward * (playerConfig.AirSpeed * Time.fixedDeltaTime) +
                    new Vector3(0, velocity.y, 0), playerConfig.AirSpeedDelta * Time.fixedDeltaTime);
        }

        private void Jump(bool grounded)
        {
            if (!grounded) return;
            _rigid.AddForce(new Vector3(0, playerConfig.JumpPower, 0), ForceMode.Impulse);
            _domain.OnAction.OnNext(PlayerActionState.Jump);
        }
    }
}