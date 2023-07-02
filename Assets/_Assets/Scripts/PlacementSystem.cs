using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance { get; private set; }
    [SerializeField] private GameObject mouseIndicator, cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject gridVisuals;
    [SerializeField] private Player player;
    private bool isPlacingStation = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StopPlacingStation();
    }

    private void Update()
    {
        if (isPlacingStation)
        {
            UpdateStationPlacement();
        }
    }

    private void UpdateStationPlacement()
    {
        Vector3 playerOffsetPos = player.GetPlayerPostionOffset();
        Vector3Int gridPos = grid.WorldToCell(playerOffsetPos);
        mouseIndicator.transform.position = playerOffsetPos;
        cellIndicator.transform.position = grid.CellToWorld(gridPos);
    }

    public void StartPlacingStation(Player player)
    {
        Debug.Log("Start placing station");
        isPlacingStation = true;
        this.player = player;
        gridVisuals.SetActive(true);
        cellIndicator.SetActive(true);
    }

    public void PlaceStationObject(BaseStation prefab)
    {
        Vector3 playerOffsetPos = player.GetPlayerPostionOffset();
        Vector3Int gridPos = grid.WorldToCell(playerOffsetPos);
        prefab.RemoveStationParent(grid.CellToWorld(gridPos));
        StopPlacingStation();
    }

    public void StopPlacingStation()
    {
        Debug.Log("Stop placing station");
        isPlacingStation = false;
        gridVisuals.SetActive(false);
        cellIndicator.SetActive(false);
    }
}
