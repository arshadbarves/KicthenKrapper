using KitchenKrapper;
using Player.Interfaces;
using UnityEngine;

namespace Player
{
    public class PlayerInputHandler : MonoBehaviour, IInputHandler
    {
        public Vector3 GetMovementInput()
        {
            var inputVector = GameInput.Instance.GetMovementInputNormalized();
            return new Vector3(inputVector.x, 0f, inputVector.y);
        }

        public bool ShouldJump()
        {
            return false;
        }
    }
}
