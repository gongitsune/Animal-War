using Projects.Scripts.Domains.Player;
using Projects.Scripts.Presenters.Common;
using UniRx;
using UnityEngine;

namespace Projects.Scripts.Presenters.Player
{
    public class PlayerCore : AnimalBase
    {
        public readonly ReactiveProperty<bool> IsMove = new(false);
        public readonly Subject<PlayerActionState> OnAction = new();
    }
}