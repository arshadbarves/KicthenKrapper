using UnityEngine;

namespace Player.Interfaces
{
    public interface IInputHandler
    {
        public Vector3 GetMovementInput();
        public bool ShouldJump();
    }
}
