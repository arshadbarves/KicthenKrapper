using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance { get; private set; }
    [SerializeField] private GameObject mouseIndicator, cellIndicator, stationIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject gridVisuals;
    [SerializeField] private Player player;
    [SerializeField] private Material validMaterial, invalidMaterial;

    private GridData gridData = new GridData();
    private Renderer previewRenderer;
    private List<GameObject> placedObjects = new List<GameObject>();
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

        bool placementValidity = CheckPlacementValidity(gridPos);
        foreach (MeshFilter meshFilter in previewRenderer.GetComponentsInChildren<MeshFilter>())
        {
            meshFilter.gameObject.GetComponent<Renderer>().material = placementValidity ? validMaterial : invalidMaterial;
        }
        cellIndicator.transform.position = grid.CellToWorld(gridPos);
    }

    public void StartPlacingStation(Player player, BaseStation prefab)
    {
        if (cellIndicator != null)
            Destroy(cellIndicator);
        cellIndicator = Instantiate(prefab.gameObject, Vector3.zero, Quaternion.identity);
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        cellIndicator.GetComponentInChildren<Collider>().enabled = false;
        isPlacingStation = true;
        this.player = player;
        gridVisuals.SetActive(true);
        cellIndicator.SetActive(true);
    }

    public bool PlaceStationObject(BaseStation prefab)
    {
        Vector3 playerOffsetPos = player.GetPlayerPostionOffset();
        Vector3Int gridPos = grid.WorldToCell(playerOffsetPos);
        if (!CheckPlacementValidity(gridPos))
            // TODO: play wrong sound
            return false;
        prefab.DropStationParent(grid.CellToWorld(gridPos));
        placedObjects.Add(cellIndicator);
        gridData.AddObjectAt(gridPos);
        StopPlacingStation();
        return true;
    }

    public void StopPlacingStation()
    {
        isPlacingStation = false;
        gridVisuals.SetActive(false);
        cellIndicator.SetActive(false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        return gridData.CanPlaceObjectAt(gridPosition);
    }
}
