using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickupObject;

    public static void ResetStaticData()
    {
        LocalInstance = null;
    }

    public event EventHandler OnPickedUpObject;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    [SerializeField] private float playerSpeed = 7f;
    [SerializeField] private float playerRotationSpeed = 10f;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spawnPoints;

    private bool isWalking = false;
    private Vector3 lastMovementDirection = Vector3.zero;
    private BaseCounter selectedCounter = null;
    private KitchenObject kitchenObject = null;

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        // transform.position = spawnPoints[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromPlayerId(OwnerClientId)];
        transform.position = GetRandomSpawnPoint();

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    // Spawn on random SpawnPoint which is not occupied by another player
    private Vector3 GetRandomSpawnPoint()
    {
        List<Vector3> freeSpawnPoints = new List<Vector3>(spawnPoints);

        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player != this)
            {
                freeSpawnPoints.Remove(player.transform.position);
            }
        }

        return freeSpawnPoints[UnityEngine.Random.Range(0, freeSpawnPoints.Count)];
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == OwnerClientId && IsServer && HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsPlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsPlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        if (!IsOwner) return; // Only the owner of the player can move it

        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementInputNormalized();

        Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (movDir != Vector3.zero)
        {
            lastMovementDirection = movDir;
        }

        if (Physics.Raycast(transform.position, lastMovementDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
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

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementInputNormalized();

        Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = playerSpeed * Time.deltaTime;

        bool canMove = !Physics.BoxCast(transform.position, new Vector3(playerRadius, playerHeight / 2f, playerRadius), movDir, Quaternion.identity, moveDistance, collisionsLayerMask);

        if (!canMove)
        {
            // If we can't move, we check if we can move in the direction of the input vector 
            // but only in the horizontal plane
            Vector3 movDirX = new Vector3(movDir.x, 0f, 0f).normalized;
            canMove = (movDir.x < -.5f || movDir.x > +.5f) && !Physics.BoxCast(transform.position, new Vector3(playerRadius, playerHeight / 2f, playerRadius), movDirX, Quaternion.identity, moveDistance, collisionsLayerMask);
            if (canMove)
            {
                // Can move only in the X axis
                movDir = movDirX;
            }
            else
            {
                // Can't move in the X axis, so we check if we can move in the Z axis
                Vector3 movDirZ = new Vector3(0f, 0f, movDir.z).normalized;
                canMove = (movDir.z < -.5f || movDir.z > +.5f) && !Physics.BoxCast(transform.position, new Vector3(playerRadius, playerHeight / 2f, playerRadius), movDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);

                if (canMove)
                {
                    // Can move only in the Z axis
                    movDir = movDirZ;
                }
                else
                {
                    // Can't move in the X or Z axis, so we can't move at all
                    canMove = false;
                }
            }

        }

        if (canMove)
        {
            transform.position += movDir * moveDistance;
        }

        isWalking = movDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, movDir, Time.deltaTime * playerRotationSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnPickedUpObject?.Invoke(this, EventArgs.Empty);
            OnAnyPickupObject?.Invoke(this, EventArgs.Empty);
        }
    }

    public void RemoveKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
