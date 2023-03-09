using UnityEngine;

[CreateAssetMenu(fileName = "New Skin", menuName = "ScriptableObjects/Skin")]
public class SkinSO : ScriptableObject
{
    [SerializeField] private string skinID = UniqueIDGenerator.GetUniqueID();
    [SerializeField] private Sprite skinSprite;
    [SerializeField] private AnimatorOverrideController animatorOverrideController;
    [SerializeField] private ParticleSystem particleEffect;
    [SerializeField] private string skinName;
    [SerializeField] private string skinDescription;
    [SerializeField] private int skinCost;
    [SerializeField] private GameObject skinPrefab;

    // Getters
    public string SkinID => skinID;
    public Sprite SkinSprite => skinSprite;
    public AnimatorOverrideController AnimatorOverrideController => animatorOverrideController;
    public ParticleSystem ParticleEffect => particleEffect;
    public string SkinName => skinName;
    public string SkinDescription => skinDescription;
    public int SkinCost => skinCost;
    public GameObject SkinPrefab => skinPrefab;
}
