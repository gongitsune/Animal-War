using UniRx;
using UnityEngine;

namespace Projects.Scripts.Domains.Player
{
    public class PlayerDomain : MonoBehaviour
    {
        public readonly ReactiveProperty<bool> IsMove = new(false);
        public readonly Subject<PlayerActionState> OnAction = new();
    }
}