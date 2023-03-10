using System;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    public event EventHandler<WalletEventArgs> OnWalletAmountChanged;

    public class WalletEventArgs : EventArgs
    {
        public int walletAmount;
    }

    public static WalletManager Instance { get; private set; }

    private int walletAmount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeDelivered += DeliveryManager_OnRecipeDelivered;
        DeliveryManager.Instance.OnRecipeDeliveryFailed += DeliveryManager_OnRecipeDeliveryFailed;
        DeliveryManager.Instance.OnRecipeExpired += DeliveryManager_OnRecipeExpired;
    }

    private void DeliveryManager_OnRecipeExpired(object sender, EventArgs e)
    {
        RecipeSO recipeSO = sender as RecipeSO;
        int negativeCoinAmount = GameManager.Instance.GetNegativeCoinAmount();
        RemoveFromWallet(negativeCoinAmount);
    }

    private void DeliveryManager_OnRecipeDeliveryFailed(object sender, EventArgs e)
    {
        RecipeSO recipeSO = sender as RecipeSO;
        int negativeCoinAmount = GameManager.Instance.GetNegativeCoinAmount();
        RemoveFromWallet(negativeCoinAmount);
    }

    private void DeliveryManager_OnRecipeDelivered(object sender, EventArgs e)
    {
        RecipeSO recipeSO = sender as RecipeSO;
        int rewardAmount = GameManager.Instance.GetRewardAmount();
        AddToWallet(rewardAmount);
    }

    public void AddToWallet(int amount)
    {
        walletAmount += amount;
        OnWalletAmountChanged?.Invoke(this, new WalletEventArgs { walletAmount = walletAmount });
    }

    public void RemoveFromWallet(int amount)
    {
        if (walletAmount - amount >= 0)
        {
            walletAmount -= amount;
            OnWalletAmountChanged?.Invoke(this, new WalletEventArgs { walletAmount = walletAmount });
        }
    }

    public int GetWalletAmount()
    {
        return walletAmount;
    }
}
