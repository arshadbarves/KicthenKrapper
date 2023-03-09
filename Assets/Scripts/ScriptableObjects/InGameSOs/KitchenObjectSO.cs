using UnityEngine;

[CreateAssetMenu(fileName = "New Kitchen Object", menuName = "ScriptableObjects/Kitchen Object")]
public class KitchenObjectSO : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}
