using System;
using Managers;
using TMPro;
using UnityEngine;

namespace KitchenKrapper
{
    public class WalletUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI walletText;

        private void Start()
        {
            LevelManager.Instance.OnWalletAmountChanged += Wallet_OnWalletChanged;
            UpdateVisual();
        }

        private void Wallet_OnWalletChanged(object sender, int e)
        {
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            walletText.text = LevelManager.Instance.GetWalletAmount().ToString();
        }
    }
}