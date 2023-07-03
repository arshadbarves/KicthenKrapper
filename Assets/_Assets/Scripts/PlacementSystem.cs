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
    private GridData gridData = new();
    private Renderer previewRenderer;
    private List<GameObject> placedObjects = new();
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
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;
        cellIndicator.transform.position = grid.CellToWorld(gridPos);
    }

    public void StartPlacingStation(Player player, BaseStation prefab)
    {
        Debug.Log("Start placing station");
        if (cellIndicator != null)
            Destroy(cellIndicator);
        cellIndicator = Instantiate(prefab.gameObject, Vector3.zero, Quaternion.identity);
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        // cellIndicator.GetComponentInChildren<MeshRenderer>().material = validMaterial;
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
            // play wrong sound
            return false;
        prefab.DropStationParent(grid.CellToWorld(gridPos));
        placedObjects.Add(cellIndicator);
        gridData.AddObjectAt(gridPos);
        StopPlacingStation();
        return true;
    }

    public void StopPlacingStation()
    {
        Debug.Log("Stop placing station");
        isPlacingStation = false;
        gridVisuals.SetActive(false);
        cellIndicator.SetActive(false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        return gridData.CanPlaceObejctAt(gridPosition);
    }
}
