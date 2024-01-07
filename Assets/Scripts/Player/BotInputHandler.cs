using Player.Interfaces;
using UnityEngine;

namespace Player
{
    public class BotInputHandler : MonoBehaviour, IInputHandler
    {
        public Vector3 GetMovementInput()
        {
            throw new System.NotImplementedException();
        }

        public bool ShouldJump()
        {
            return false;
        }
    }
}
