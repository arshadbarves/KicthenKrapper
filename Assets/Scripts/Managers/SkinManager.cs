using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
    public class SkinManager : MonoBehaviour
    {
        // Singleton instance
        public static SkinManager Instance { get; private set; }

        // Dictionary of all the skins
        private Dictionary<string, SkinSO> skins = new Dictionary<string, SkinSO>();

        // The currently selected skin
        private SkinSO selectedSkin;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            // Load all the skins
            LoadAllSkins();
        }

        private void Start()
        {
            // Load the selected skin from the player's inventory
            selectedSkin = PlayerInventory.Instance.GetSelectedSkin();
        }

        private void LoadAllSkins()
        {
            // Load all the skins from the "Skins" folder and populate the dictionary
            SkinSO[] allSkins = Resources.LoadAll<SkinSO>("Skins");
            foreach (SkinSO skin in allSkins)
            {
                skins.Add(skin.SkinID, skin);
            }
        }

        public IReadOnlyDictionary<string, SkinSO> GetSkins()
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
            if (CanUnlockSkin(skin))
            {
                // Remove the required coins
                PlayerInventory.Instance.SpendCoins(skin.SkinCost);

                // Unlock the skin in the player's inventory
                PlayerInventory.Instance.UnlockSkin(skin);

                return true;
            }

            return false;
        }

        public IReadOnlyDictionary<string, SkinSO> GetUnlockedSkins()
        {
            return PlayerInventory.Instance.GetUnlockedSkins();
        }

        public bool HasSkin(SkinSO skin)
        {
            return PlayerInventory.Instance.HasSkin(skin);
        }

        public IReadOnlyDictionary<string, SkinSO> GetLockedSkins()
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

        private bool CanUnlockSkin(SkinSO skin)
        {
            // Check if the player has enough coins and doesn't already own the skin
            return PlayerInventory.Instance.HasEnoughCoins(skin.SkinCost) && !PlayerInventory.Instance.HasSkin(skin);
        }
    }
}
