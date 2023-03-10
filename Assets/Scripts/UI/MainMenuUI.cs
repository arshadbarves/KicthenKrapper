using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    // [SerializeField] private Button MapButton;
    // [SerializeField] private Button ShopButton;
    // [SerializeField] private GameObject MapPanel;
    // [SerializeField] private GameObject ShopPanel;

    private void Awake()
    {
        // playButton.onClick.AddListener(OnPlayButtonClicked);
        // MapButton.onClick.AddListener(OnNavigationButtonClicked(MapPanel));
        // ShopButton.onClick.AddListener(OnNavigationButtonClicked(ShopPanel));
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
