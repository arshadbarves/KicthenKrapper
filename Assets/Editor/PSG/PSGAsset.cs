using UnityEngine;

[CreateAssetMenu(fileName = "PSGAsset", menuName = "PSG/PSGAsset", order = 1)]
public class PSGAsset : ScriptableObject
{
    public int id;
    public GameObject prefab;
    public Vector2 area;
    public PrefabTag prefabTag;
    public PrefabType prefabType;

    public Bounds bounds => prefab.GetComponent<MeshRenderer>().bounds;

    [Range(0, 1)]
    public float probability = 1;

    public bool cornerSupport = false;
    public bool isLimited = false;
    public int limit = 0;

    // Generate a unique id for the prefab on creation
    private void OnEnable()
    {
        id = GetInstanceID();
    }
}
