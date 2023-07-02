using System;
using System.Collections.Generic;
using Epic.OnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples.Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickupObject;
    public static event EventHandler OnAnyStationGrabbed;
    public static event EventHandler OnAnyStationPlaced;

    public static void ResetStaticData()
    {
        LocalInstance = null;
    }

    public event EventHandler OnPickedUpObject;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseStation selectedCounter;
    }
    [SerializeField] private readonly float playerSpeed = 7f;
    [SerializeField] private readonly float playerRotationSpeed = 10f;
    [SerializeField] private readonly float playerRadius = 0.7f;
    [SerializeField] private readonly float playerHeight = 2f;
    [SerializeField] private readonly float interactDistance = 2f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spawnPoints;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private bool isTutorialPlayer = false;
    [SerializeField] private Vector3 stationHoldOffset = Vector3.zero;
    [SerializeField] private GameObject stationHoldPrefab;

    private bool isWalking = false;
    private Vector3 lastMovementDirection = Vector3.zero;
    private BaseStation selectedCounter = null;
    private KitchenObject kitchenObject = null;
    private BaseStation grabbedStationObject;

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

        if (!GameDataSource.PlayMultiplayer)
        {
            SpawnPlayer();
        }
        if (TutorialManager.Instance != null)
        {
            isTutorialPlayer = true;
            SpawnPlayer();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameInput.Instance.OnInteractAction -= GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction -= GameInput_OnInteractAlternateAction;
    }

    public static void SetNetworkHostId(ProductUserId userId)
    {
        var transportLayer = NetworkManager.Singleton?.GetComponent<EOSTransport>();
        if (transportLayer != null)
        {
            transportLayer.ServerUserIdToConnectTo = userId;
            print($"SetNetworkHostId: {transportLayer.ServerUserIdToConnectTo}");
        }
    }

    public static void DestroyNetworkManager()
    {
        if (NetworkManager.Singleton?.gameObject != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        SpawnPlayer();

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }

        if (IsOwner)
        {
            displayNameText.text = GameDataSource.Instance.GetPlayerData().PlayerName;
            displayNameText.color = Color.green;

            // Get the Virtual Camera and follow the player
            var virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                virtualCamera.Follow = transform;
                virtualCamera.LookAt = transform;
            }

        }
        else
        {
            displayNameText.text = EOSKitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerId(OwnerClientId).playerName.ToString();
            displayNameText.color = Color.red;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }

    public void SpawnPlayer()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        // transform.position = spawnPoints[EOSKitchenGameMultiplayer.Instance.GetPlayerDataIndexFromPlayerId(OwnerClientId)];
        transform.position = GetRandomSpawnPoint();

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
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
        if (!TutorialManager.Instance && !LevelManager.Instance.IsPlaying()) return;

        if (selectedCounter != null)
        {
            if (LevelManager.Instance.IsPlaying())
            {
                selectedCounter.InteractAlternate(this);
            }
            else
            {
                // Check if the player already has a station
                if(IsHoldingStationObject())
                {
                    // Place the station
                    PlaceStationObject();
                    OnAnyStationPlaced?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    // Grab the station
                    GrabStationObject(selectedCounter);
                    OnAnyStationGrabbed?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    private void PlaceStationObject()
    {
        if (IsHoldingStationObject())
        {
            grabbedStationObject.transform.SetParent(null);
            grabbedStationObject.GetComponent<Collider>().enabled = true;
            grabbedStationObject = null;
        }
    }

    private void GrabStationObject(BaseStation station)
    {
        if (!IsHoldingStationObject())
        {
            grabbedStationObject = station;
            grabbedStationObject.transform.SetParent(transform);
            grabbedStationObject.transform.localPosition = stationHoldOffset;
            grabbedStationObject.transform.localRotation = Quaternion.identity;
            grabbedStationObject.GetComponent<Collider>().enabled = false;
        }
    }

    private bool IsHoldingStationObject()
    {
        return grabbedStationObject != null;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (!TutorialManager.Instance && !LevelManager.Instance.IsPlaying()) return;

        selectedCounter?.Interact(this);
    }

    private void Update()
    {
        if (!isTutorialPlayer)
        {
            if (!IsOwner) return; // Only the owner of the player can move it
        }
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
            if (raycastHit.transform.TryGetComponent(out BaseStation baseCounter))
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
        print(inputVector);

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

    private void SetSelectedCounter(BaseStation selectedCounter)
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

    public void SetIsTutorialPlayer(bool isTutorialPlayer)
    {
        this.isTutorialPlayer = isTutorialPlayer;
    }

    public Vector3 GetPlayerPostionOffset()
    {
        return transform.position + stationHoldOffset;
    }
}
