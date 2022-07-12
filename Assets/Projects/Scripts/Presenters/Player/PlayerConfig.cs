using UnityEngine;

namespace Projects.Scripts.Presenters.Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/Player", order = 0)]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField] private float groundSpeed, airSpeed, airSpeedDelta, rotateSpeed;
        [SerializeField] private float jumpPower;
        public float GroundSpeed => groundSpeed;
        public float AirSpeed => airSpeed;
        public float AirSpeedDelta => airSpeedDelta;
        public float RotateSpeed => rotateSpeed;
        public float JumpPower => jumpPower;
    }
}