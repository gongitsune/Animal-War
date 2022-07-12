using System;
using DG.Tweening;
using Projects.Scripts.Domains.Player;
using Projects.Scripts.Presenters.Common;
using UniRx;
using UnityEngine;

namespace Projects.Scripts.Presenters.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private static readonly int AnimIdSpeed = Animator.StringToHash("Speed");
        private static readonly int AnimIdGrounded = Animator.StringToHash("Grounded");
        private static readonly int AnimIdJump = Animator.StringToHash("Jump");
        private Animator _animator;
        private PlayerDomain _domain;

        private void Awake()
        {
            TryGetComponent(out _animator);
            TryGetComponent(out _domain);
            TryGetComponent(out GroundedChecker groundedChecker);

            groundedChecker.IsGrounded
                .Subscribe(grounded =>
                {
                    _animator.SetBool(AnimIdGrounded, grounded);
                    if (grounded) _animator.SetBool(AnimIdJump, false);
                })
                .AddTo(this);
            _domain.IsMove
                .Subscribe(state =>
                {
                    DOTween.To(() => _animator.GetFloat(AnimIdSpeed), v => _animator.SetFloat(AnimIdSpeed, v),
                        state ? 1 : 0, 0.5f).SetEase(Ease.OutQuad).SetLink(gameObject);
                })
                .AddTo(this);
            _domain.OnAction
                .Subscribe(action =>
                {
                    switch (action)
                    {
                        case PlayerActionState.Jump:
                            _animator.SetBool(AnimIdJump, true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(action), action, null);
                    }
                })
                .AddTo(this);
        }
    }
}