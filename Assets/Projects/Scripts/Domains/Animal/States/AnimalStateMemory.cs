using System.Collections.Generic;
using MyPackages.StateMachine;
using UnityEngine;

namespace Projects.Scripts.Domains.Animal.States
{
    public class AnimalStateMemory : Memory
    {
        [SerializeField] private Transform[] beacons;

        public IEnumerable<Transform> Beacons => beacons;
    }
}