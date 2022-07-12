using DG.Tweening;
using Projects.Scripts.Domains.Animal;
using UniRx;
using UnityEngine;

namespace Projects.Scripts.Presenters.Animal
{
    public class AnimalAnimator : MonoBehaviour
    {
        private static readonly int AnimIdSpeed = Animator.StringToHash("Speed");
        private Animator _animator;
        private AnimalDomain _domain;

        private void Awake()
        {
            TryGetComponent(out _animator);
            TryGetComponent(out _domain);

            _domain.IsMove.Subscribe(state =>
            {
                DOTween
                    .To(() => _animator.GetFloat(AnimIdSpeed), v => _animator.SetFloat(AnimIdSpeed, v),
                        state ? 1 : 0, 0.5f)
                    .SetEase(Ease.OutQuad)
                    .SetLink(gameObject);
            }).AddTo(this);
        }
    }
}