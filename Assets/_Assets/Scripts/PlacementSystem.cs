using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator, cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject gridVisuals;
    [SerializeField] private Player player;

    private void Update()
    {
        // Vector3 mousePos = inputManager.GetSelectedMapPosition();
        Vector3 mousePos = player.GetPlayerPostionOffset();
        mousePos.y = 0;
        Vector3Int gridPos = grid.WorldToCell(mousePos);
        mouseIndicator.transform.position = mousePos;
        cellIndicator.transform.position = grid.CellToWorld(gridPos);   
    }

    public void StartPlacing(GameObject prefab)
    {
        gridVisuals.SetActive(true);
        cellIndicator.SetActive(true);
    }

    public void StopPlacing()
    {
        gridVisuals.SetActive(false);
        cellIndicator.SetActive(false);
    }
}
