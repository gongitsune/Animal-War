using UniRx;
using UnityEngine;

namespace Projects.Scripts.Domains.Animal
{
    public class AnimalDomain : MonoBehaviour
    {
        public readonly ReactiveProperty<bool> IsMove = new();
    }
}