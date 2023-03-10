using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UniversalMobileController;

public class GameInput : MonoBehaviour
{
    [SerializeField] private FloatingJoyStick floatingJoyStick;

    public static GameInput Instance
    { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;

    public PlayerInputActions playerInputActions;

    private InputType inputType;

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

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable(); // Enable the player input actions

        playerInputActions.Player.Interact.performed += Interact_performed; // Subscribe to the Interact action
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed; // Subscribe to the InteractAlternate action
    }

    private void Start()
    {
        inputType = GameDataSource.Instance.GetInputType();
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= Interact_performed; // Unsubscribe from the Interact action
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed; // Unsubscribe from the InteractAlternate action

        playerInputActions.Dispose(); // Dispose of the player input actions
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
        Vector2 inputVector = Vector2.zero;

        switch (inputType)
        {
            case InputType.Keyboard:
                inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>(); // Get the movement input
                break;
            case InputType.Mobile:
                inputVector = new Vector2(floatingJoyStick.GetHorizontalValue(), floatingJoyStick.GetVerticalValue()); // Get the movement input
                break;
        }
        inputVector = inputVector.normalized; // Normalize the input vector
        return inputVector;
    }
}
