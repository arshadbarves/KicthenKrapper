using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    private const string DELIVERY_RESULT_ANIMATION_TRIGGER = "ShowDeliveryResult";

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeDelivered += DeliveryManager_OnRecipeDelivered;
        DeliveryManager.Instance.OnRecipeDeliveryFailed += DeliveryManager_OnRecipeDeliveryFailed;

        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnRecipeDeliveryFailed(object sender, EventArgs e)
    {
        Show();
        animator.SetTrigger(DELIVERY_RESULT_ANIMATION_TRIGGER);
        backgroundImage.color = failColor;
        iconImage.sprite = failSprite;
        resultText.text = "DELIVERY\nFAILED";
    }

    private void DeliveryManager_OnRecipeDelivered(object sender, EventArgs e)
    {
        Show();
        animator.SetTrigger(DELIVERY_RESULT_ANIMATION_TRIGGER);
        backgroundImage.color = successColor;
        iconImage.sprite = successSprite;
        resultText.text = "DELIVERY\nSUCCESS";
    }
}
