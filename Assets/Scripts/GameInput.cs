using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UniversalMobileController;
using Utils.Enums;

namespace KitchenKrapper
{
    public class GameInput : MonoBehaviour
    {
        [SerializeField] private FloatingJoyStick floatingJoyStick;

        public static GameInput Instance
        { get; private set; }

        public event EventHandler OnInteractAction;
        public event EventHandler OnInteractAlternateAction;

        private PlayerInputActions _playerInputActions;

        [SerializeField] private InputType inputType;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Player.Enable(); // Enable the player input actions

            _playerInputActions.Player.Interact.performed += Interact_performed; // Subscribe to the Interact action
            _playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed; // Subscribe to the InteractAlternate action
        }

        private void Start()
        {
            if (LevelManager.Instance.isDebugMode)
            {
                return;
            }
            inputType = GameManager.Instance.GetInputType();
        }

        private void OnDestroy()
        {
            _playerInputActions.Player.Interact.performed -= Interact_performed; // Unsubscribe from the Interact action
            _playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed; // Unsubscribe from the InteractAlternate action

            _playerInputActions.Dispose(); // Dispose of the player input actions
        }

        private void InteractAlternate_performed(InputAction.CallbackContext obj)
        {
            OnInteractAlternateAction?.Invoke(this, EventArgs.Empty); // Invoke the OnInteractAlternateAction event when the InteractAlternate action is performed
        }

        private void Interact_performed(InputAction.CallbackContext obj)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty); // Invoke the OnInteractAction event when the Interact action is performed
        }

        public Vector2 GetMovementInputNormalized()
        {
            var inputVector = Vector2.zero;

            switch (inputType)
            {
                case InputType.Keyboard:
                    inputVector = _playerInputActions.Player.Movement.ReadValue<Vector2>(); // Get the movement input
                    break;
                case InputType.Touch:
                    inputVector = new Vector2(floatingJoyStick.GetHorizontalValue(), floatingJoyStick.GetVerticalValue()); // Get the movement input
                    break;
                case InputType.Gamepad:
                    break;
                case InputType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            inputVector = inputVector.normalized; // Normalize the input vector
            return inputVector;
        }
    }
}