#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ProceduralSceneGenerator : MonoBehaviour
{
    public static ProceduralSceneGenerator Instance;
    public bool showGrid = true;

    [Header("Generators")]
    public FloorGenerator floorGenerator;
    public CounterGenerator counterGenerator;
    public WallGenerator wallGenerator;
    public DecorativeGenerator decorativeGenerator;

    [Header("Level Size")]
    public Vector2Int levelSize;
    public Vector2Int playgroundSize;
    public Vector2Int gridSize;

    public float cellSize = 1;
    public List<Cell> cells;

    [Header("Generation")]
    public bool generateFloor = true;
    public bool generateCounters = true;
    public bool generateWalls = true;
    public bool generateDecorative = true;
    private void Awake()
    {
        Instance = this;
    }

    public void GenerateCells()
    {
        // Create a 2D array to represent the cells
        cells = new List<Cell>();

        // Calculate the middle point of the level size
        int middleX = levelSize.x / 2;
        int middleY = levelSize.y / 2;

        // Iterate through each cell position in the grid
        for (int x = 0; x < levelSize.x; x++)
        {
            for (int y = 0; y < levelSize.y; y++)
            {
                // Calculate the world position of the current cell
                Vector3 cellPosition = new Vector3((x - middleX) * cellSize, 0, (y - middleY) * cellSize);

                CellTag cellTag;
                bool isCorner;
                Quaternion rotation;

                // Check if the cell falls within the playable area
                if (Grinder(cellPosition, out cellTag, out isCorner, out rotation))
                {
                    cells.Add(new Cell(cellPosition, PrefabTag.Inside, cellTag, isCorner, rotation));
                }
                else
                {
                    cells.Add(new Cell(cellPosition, PrefabTag.Outside, cellTag, isCorner, Quaternion.identity));
                }
            }
        }
    }

    private bool Grinder(Vector3 cellPosition, out CellTag cellTag, out bool isCorner, out Quaternion rotation)
    {
        // Calculate the boundaries of the level size
        float minX = -levelSize.x / 2f * cellSize;
        float maxX = levelSize.x / 2f * cellSize;
        float minZ = -levelSize.y / 2f * cellSize;
        float maxZ = levelSize.y / 2f * cellSize;

        // Calculate the boundaries of the playable area
        float minXPlayground = -playgroundSize.x / 2f * cellSize;
        float maxXPlayground = playgroundSize.x / 2f * cellSize;
        float minZPlayground = -playgroundSize.y / 2f * cellSize;
        float maxZPlayground = playgroundSize.y / 2f * cellSize;

        // Check if the cell position falls within the boundaries of the playable area
        if (cellPosition.x >= minXPlayground && cellPosition.x <= maxXPlayground &&
            cellPosition.z >= minZPlayground && cellPosition.z <= maxZPlayground)
        {
            // Check if the cell position is suitable for a counter to be placed on it (i.e., edge of the playable area)
            bool isOnPlaygroundEdge = cellPosition.x == minXPlayground || cellPosition.x == maxXPlayground || cellPosition.z == minZPlayground || cellPosition.z == maxZPlayground;

            // Check if it's a corner cell
            bool isOnCorner = (cellPosition.x == minXPlayground && cellPosition.z == minZPlayground) ||
                              (cellPosition.x == minXPlayground && cellPosition.z == maxZPlayground) ||
                              (cellPosition.x == maxXPlayground && cellPosition.z == minZPlayground) ||
                              (cellPosition.x == maxXPlayground && cellPosition.z == maxZPlayground);

            if (isOnPlaygroundEdge || isOnCorner)
            {
                cellTag = CellTag.Counter;
                isCorner = isOnCorner;

                // Calculate rotation based on the cell's position
                if (cellPosition.x == minXPlayground)
                {
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                }
                else if (cellPosition.x == maxXPlayground)
                {
                    rotation = Quaternion.Euler(0f, 270f, 0f);
                }
                else if (cellPosition.z == minZPlayground)
                {
                    rotation = Quaternion.Euler(0f, 0f, 0f);
                }
                else // cellPosition.z == maxZPlayground
                {
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                }
            }
            else
            {
                cellTag = CellTag.None;
                isCorner = false;
                rotation = Quaternion.identity;
            }

            return true;
        }
        else
        {
            // Check if the cell position is suitable for a wall to be placed on it (i.e., edge of the level size)
            bool isOnLevelEdge = cellPosition.x == minX || cellPosition.x == maxX || cellPosition.z == minZ || cellPosition.z == maxZ;

            // Check if it's a corner cell
            bool isOnCorner = (cellPosition.x == minX && cellPosition.z == minZ) ||
                              (cellPosition.x == minX && cellPosition.z == maxZ) ||
                              (cellPosition.x == maxX && cellPosition.z == minZ) ||
                              (cellPosition.x == maxX && cellPosition.z == maxZ);

            if (isOnLevelEdge || isOnCorner)
            {
                cellTag = CellTag.Wall;
                isCorner = isOnCorner;

                // Calculate rotation based on the cell's position
                if (cellPosition.x == minX)
                {
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                }
                else if (cellPosition.x == maxX)
                {
                    rotation = Quaternion.Euler(0f, 270f, 0f);
                }
                else if (cellPosition.z == minZ)
                {
                    rotation = Quaternion.Euler(0f, 0f, 0f);
                }
                else // cellPosition.z == maxZ
                {
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                }
            }
            else
            {
                cellTag = CellTag.None;
                isCorner = false;
                rotation = Quaternion.identity;
            }

            return false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGrid)
        {
            return;
        }

        // Draw the Gizmos for level size and playground size with cell size included
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(levelSize.x * cellSize, 0, levelSize.y * cellSize));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(playgroundSize.x * cellSize, 0, playgroundSize.y * cellSize));

        // Draw the Gizmos for the grid
        if (cells != null)
        {
            foreach (var cell in cells)
            {
                // Draw the Gizmos for the cell position by the cell tag and prefab tag
                if (cell.prefabTag == PrefabTag.Inside)
                {
                    if (cell.cellTag == CellTag.Counter)
                    {
                        if (cell.isCorner)
                        {
                            Gizmos.color = Color.yellow;
                        }
                        else
                        {
                            Gizmos.color = Color.blue;
                        }
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                    }
                }
                else
                {
                    if (cell.cellTag == CellTag.Wall)
                    {
                        Gizmos.color = Color.black;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                }
                Gizmos.DrawWireCube(cell.position, new Vector3(cellSize, 0, cellSize));
            }
        }
    }
}

[CustomEditor(typeof(ProceduralSceneGenerator))]
public class ProceduralSceneGeneratorEditor : Editor
{
    ProceduralSceneGenerator sceneGenerator;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        sceneGenerator = (ProceduralSceneGenerator)target;
        if (GUILayout.Button("Generate Level"))
        {
            sceneGenerator.GenerateCells();
            if (sceneGenerator.generateWalls)
            {
                sceneGenerator.wallGenerator.Generate(sceneGenerator.cells, sceneGenerator.gridSize);
            }
            if (sceneGenerator.generateFloor)
            {
                sceneGenerator.floorGenerator.Generate(sceneGenerator.cells, sceneGenerator.gridSize);
            }
            if (sceneGenerator.generateCounters)
            {
                sceneGenerator.counterGenerator.Generate(sceneGenerator.cells, sceneGenerator.gridSize);
            }
            if (sceneGenerator.generateDecorative)
            {
                sceneGenerator.decorativeGenerator.Generate(sceneGenerator.cells, sceneGenerator.gridSize);
            }
        }
        if (GUILayout.Button("Clear Mesh"))
        {
            ClearAll();
        }
    }

    void ClearAll()
    {
        sceneGenerator.floorGenerator.ClearMeshes();
        sceneGenerator.counterGenerator.ClearMeshes();
        sceneGenerator.wallGenerator.ClearMeshes();
        sceneGenerator.decorativeGenerator.ClearMeshes();
    }
}

public class Cell
{
    public Vector3 position;
    public PrefabTag prefabTag;
    public CellTag cellTag;
    public bool isCorner;
    public Quaternion rotation;

    public PSGAsset prefab;

    public Cell(Vector3 position, PrefabTag prefabTag, CellTag cellTag, bool isCorner, Quaternion rotation = default(Quaternion))
    {
        this.position = position;
        this.prefabTag = prefabTag;
        this.cellTag = cellTag;
        this.isCorner = isCorner;
        this.rotation = rotation;
    }
}
#endif