using System;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Projects.Scripts.Presenters.Animal
{
    public class AnimalAnimator : MonoBehaviour
    {
        public enum ActionAnim
        {
            Attack01,
            Attack02,
            GetHit,
            Defense
        }

        private static readonly int AnimIdSpeed = Animator.StringToHash("Speed");
        private static readonly int AnimIdAttack01 = Animator.StringToHash("Attack01");
        private static readonly int AnimIdAttack02 = Animator.StringToHash("Attack02");
        private static readonly int AnimIdGetHit = Animator.StringToHash("GetHit");
        private static readonly int AnimIdDefense = Animator.StringToHash("Defense");
        private Animator _animator;

        public float Speed
        {
            get => _animator.GetFloat(AnimIdSpeed);
            set => _animator.SetFloat(AnimIdSpeed, math.clamp(value, 0, 2));
        }

        private void Awake()
        {
            TryGetComponent(out _animator);
        }

        public void DispatchAction(ActionAnim anim)
        {
            switch (anim)
            {
                case ActionAnim.Attack01:
                    SetTrigger(AnimIdAttack01);
                    break;
                case ActionAnim.Attack02:
                    SetTrigger(AnimIdAttack02);
                    break;
                case ActionAnim.GetHit:
                    SetTrigger(AnimIdGetHit);
                    break;
                case ActionAnim.Defense:
                    SetTrigger(AnimIdDefense);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(anim), anim, null);
            }
        }

        private async void SetTrigger(int id)
        {
            _animator.SetBool(id, true);
            await UniTask.Delay(TimeSpan.FromTicks(1));
            _animator.SetBool(id, false);
        }
    }
}