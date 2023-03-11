using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Toggle multiplayerToggle;
    [SerializeField] private TextMeshProUGUI multiplayerText;
    // [SerializeField] private Button MapButton;
    // [SerializeField] private Button ShopButton;
    // [SerializeField] private GameObject MapPanel;
    // [SerializeField] private GameObject ShopPanel;

    private void Awake()
    {
        // playButton.onClick.AddListener(OnPlayButtonClicked);
        multiplayerToggle.onValueChanged.AddListener(OnMultiplayerToggleValueChanged);
        // MapButton.onClick.AddListener(OnNavigationButtonClicked(MapPanel));
        // ShopButton.onClick.AddListener(OnNavigationButtonClicked(ShopPanel));
    }

    private void OnMultiplayerToggleValueChanged(bool isMultiplayer)
    {
        if (isMultiplayer)
        {
            multiplayerText.text = "Multiplayer";
        }
        else
        {
            multiplayerText.text = "Singleplayer";
        }

        KitchenGameMultiplayer.playMultiplayer = isMultiplayer;
    }

    private UnityAction OnNavigationButtonClicked(GameObject mapPanel)
    {
        return () => ShowPanel(mapPanel);
    }

    private void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void OnPlayButtonClicked()
    {
        SceneLoaderWrapper.Instance.LoadScene(SceneType.MultiPlayerMenu.ToString(), false);
    }
}
