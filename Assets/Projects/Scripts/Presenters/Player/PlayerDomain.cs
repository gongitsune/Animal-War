using Projects.Scripts.Domains.Player;
using UniRx;
using UnityEngine;

namespace Projects.Scripts.Presenters.Player
{
    public class PlayerDomain : MonoBehaviour
    {
        public readonly ReactiveProperty<bool> IsMove = new(false);
        public readonly Subject<PlayerActionState> OnAction = new();
    }
}