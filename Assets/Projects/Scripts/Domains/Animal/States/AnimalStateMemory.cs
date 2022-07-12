using MyPackages.StateMachine;
using UnityEngine;

namespace Projects.Scripts.Domains.Animal.States
{
    public class AnimalStateMemory: Memory
    {
        [SerializeField] private Transform[] beacons;

        public Transform[] Beacons => beacons;
    }
}