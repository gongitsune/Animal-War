using MyPackages.StateMachine;
using UnityEngine;

namespace Projects.Scripts.Presenters.Animal
{
    public partial class Animal
    {
        private class IdleState : MyState
        {
            protected internal override void Enter()
            {
                Debug.Log("Enter Idle State");
            }
        }
    }
}