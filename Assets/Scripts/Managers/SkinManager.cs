using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    // Handle to the skin manager
    public static SkinManager Instance { get; private set; }

    // List of all the skins
    public Dictionary<string, SkinSO> skins = new Dictionary<string, SkinSO>();

    // The currently selected skin
    public SkinSO selectedSkin;

    private void Awake()
    {
        Instance = this;

        // Load all the skins
        SkinSO[] allSkins = Resources.LoadAll<SkinSO>("Skins"); // Load all the skins from the "Skins" folder
        foreach (SkinSO skin in allSkins)
        {
            skins.Add(skin.SkinID, skin);
        }
    }

    private void Start()
    {
        // Load the selected skin
        selectedSkin = PlayerInventory.Instance.GetSelectedSkin();
    }

    public Dictionary<string, SkinSO> GetSkins()
    {
        return skins;
    }

    public void SelectSkin(SkinSO skin)
    {
        selectedSkin = skin;
        PlayerInventory.Instance.SetSelectedSkin(skin);
    }

    public SkinSO GetSelectedSkin()
    {
        return selectedSkin;
    }

    public bool UnlockSkin(SkinSO skin)
    {
        // Check if the player has enough coins and already owns the skin
        if (PlayerInventory.Instance.HasEnoughCoins(skin.SkinCost) && !PlayerInventory.Instance.HasSkin(skin))
        {
            // Remove the coins
            PlayerInventory.Instance.SpendCoins(skin.SkinCost);

            // Unlock the skin
            PlayerInventory.Instance.UnlockSkin(skin);

            return true;
        }

        return false;
    }

    public Dictionary<string, SkinSO> GetUnlockedSkins()
    {
        return PlayerInventory.Instance.GetUnlockedSkins();
    }

    public bool HasSkin(SkinSO skin)
    {
        return PlayerInventory.Instance.HasSkin(skin);
    }

    public Dictionary<string, SkinSO> GetLockedSkins()
    {
        Dictionary<string, SkinSO> lockedSkins = new Dictionary<string, SkinSO>();

        foreach (KeyValuePair<string, SkinSO> skin in skins)
        {
            if (!PlayerInventory.Instance.HasSkin(skin.Value))
            {
                lockedSkins.Add(skin.Key, skin.Value);
            }
        }

        return lockedSkins;
    }
}
