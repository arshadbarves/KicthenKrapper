using TMPro;
using UnityEngine;

public class WalletUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI walletText;

    private void Start()
    {
        WalletManager.Instance.OnWalletAmountChanged += Wallet_OnWalletChanged;
        UpdateVisual();
    }

    private void Wallet_OnWalletChanged(object sender, WalletManager.WalletEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        walletText.text = WalletManager.Instance.GetWalletAmount().ToString();
    }
}
