using System;
using KitchenKrapper;
using Player.Interfaces;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(IInputHandler)), RequireComponent(typeof(CharacterController))]
    public class BasePlayer : NetworkBehaviour, IKitchenObjectParent
    {
        private const float InteractDistance = 2f;
        private CharacterController _characterController;
        private Vector3 _playerVelocity;
        private bool _groundedPlayer;
        private IInputHandler _inputHandler;
        private Vector3 _lastMovementDirection = Vector3.zero;
        protected BaseStation SelectedCounter;
        private KitchenObject _kitchenObject;
        protected BaseStation GrabbedStationObject;
        [SerializeField] private float playerSpeed = 2.0f;
        [SerializeField] private float rotationSpeed = 20.0f;
        [SerializeField] private float jumpHeight = 1.0f;
        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private LayerMask countersLayerMask;
        [SerializeField] private LayerMask collisionsLayerMask;
        [SerializeField] private Vector3 stationHoldOffset = Vector3.zero;
        [SerializeField] private Transform kitchenObjectHoldPoint;
        [SerializeField] protected TextMeshProUGUI displayNameText;
        public static event Action OnPlayerSpawned;
        public static event Action OnAnyPickupObject;
        public event Action OnPickedUpObject;
        public event Action<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

        public class OnSelectedCounterChangedEventArgs : EventArgs
        {
            public BaseStation SelectedCounter;
        }

        protected virtual void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _inputHandler = GetComponent<IInputHandler>();
        }

        protected virtual void Update()
        {
            HandleMovementInput();
            HandleInteractInput();
            ApplyGravity();
        }

        private void HandleInteractInput()
        {
            Vector2 inputVector = _inputHandler.GetMovementInput();
            var movementDirection = new Vector3(inputVector.x, 0f, inputVector.y);
            if (movementDirection != Vector3.zero)
            {
                _lastMovementDirection = movementDirection;
            }

            var raycastOrigin = transform.position + Vector3.up * 0.5f;
            if (HasStationObject())
            {
                raycastOrigin += stationHoldOffset;
            }

            if (Physics.Raycast(raycastOrigin, _lastMovementDirection, out RaycastHit raycastHit, InteractDistance,
                    countersLayerMask))
            {
                if (raycastHit.transform.TryGetComponent(out BaseStation baseCounter))
                {
                    if (baseCounter != SelectedCounter)
                    {
                        SetSelectedCounter(baseCounter);
                    }
                }
                else
                {
                    SetSelectedCounter(null);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }

        private void SetSelectedCounter(BaseStation selectedCounter)
        {
            SelectedCounter = selectedCounter;
            OnSelectedCounterChanged?.Invoke(new OnSelectedCounterChangedEventArgs
            {
                SelectedCounter = SelectedCounter
            });
        }

        private void HandleMovementInput()
        {
            _groundedPlayer = _characterController.isGrounded;
            if (_groundedPlayer && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0f;
            }

            var move = _inputHandler.GetMovementInput();
            Debug.DrawRay(transform.position, move, Color.red);
            RotatePlayer(move);
            if (CanMove(move))
            {
                MovePlayer(move);
            }

            JumpPlayer();
        }

        private void MovePlayer(Vector3 move)
        {
            _characterController.Move(move * (Time.deltaTime * playerSpeed));
        }

        private bool CanMove(Vector3 move)
        {
            var moveDistance = Time.deltaTime * playerSpeed;
            var characterTransform = _characterController.transform;
            return !Physics.BoxCast(characterTransform.position, _characterController.bounds.extents, move.normalized,
                characterTransform.rotation, moveDistance, collisionsLayerMask);
        }

        private void RotatePlayer(Vector3 move)
        {
            if (move != Vector3.zero)
            {
                transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * rotationSpeed);
            }
        }

        private void JumpPlayer()
        {
            if (_inputHandler.ShouldJump() && _groundedPlayer)
            {
                _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
        }

        private void ApplyGravity()
        {
            _playerVelocity.y += gravityValue * Time.deltaTime;
            _characterController.Move(_playerVelocity * Time.deltaTime);
        }

        public bool IsMoving()
        {
            return _inputHandler.GetMovementInput() != Vector3.zero;
        }

        public bool IsGrounded()
        {
            return _groundedPlayer;
        }

        public bool IsJumping()
        {
            return _playerVelocity.y > 0;
        }

        public bool IsFalling()
        {
            return _playerVelocity.y < 0;
        }

        private bool HasStationObject()
        {
            return SelectedCounter != null;
        }

        public Transform GetKitchenObjectFollowTransform()
        {
            return kitchenObjectHoldPoint;
        }

        public KitchenObject GetKitchenObject()
        {
            return _kitchenObject;
        }

        public void SetKitchenObject(KitchenObject kitchenObject)
        {
            _kitchenObject = kitchenObject;
            if (kitchenObject == null) return;
            OnPickedUpObject?.Invoke();
            OnAnyPickupObject?.Invoke();
        }

        public void RemoveKitchenObject()
        {
            _kitchenObject = null;
        }

        public bool HasKitchenObject()
        {
            return _kitchenObject != null;
        }

        public NetworkObject GetNetworkObject()
        {
            return NetworkObject;
        }

        public Vector3 GetPlayerPositionOffset()
        {
            return transform.position + stationHoldOffset;
        }

        protected static void PlayerSpawned()
        {
            OnPlayerSpawned?.Invoke();
        }
    }
}