using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
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
            SubscribeToDeliveryEvents();
            Hide();
        }

        private void SubscribeToDeliveryEvents()
        {
            DeliveryManager.Instance.OnRecipeDelivered += HandleRecipeDelivered;
            DeliveryManager.Instance.OnRecipeDeliveryFailed += HandleRecipeDeliveryFailed;
        }

        private void UnsubscribeFromDeliveryEvents()
        {
            DeliveryManager.Instance.OnRecipeDelivered -= HandleRecipeDelivered;
            DeliveryManager.Instance.OnRecipeDeliveryFailed -= HandleRecipeDeliveryFailed;
        }

        private void ShowDeliveryResult(Color backgroundColor, Sprite iconSprite, string resultMessage)
        {
            Show();
            animator.SetTrigger(DELIVERY_RESULT_ANIMATION_TRIGGER);
            backgroundImage.color = backgroundColor;
            iconImage.sprite = iconSprite;
            resultText.text = resultMessage;
        }

        private void HandleRecipeDeliveryFailed(object sender, EventArgs e)
        {
            ShowDeliveryResult(failColor, failSprite, "DELIVERY\nFAILED");
        }

        private void HandleRecipeDelivered(object sender, EventArgs e)
        {
            ShowDeliveryResult(successColor, successSprite, "DELIVERY\nSUCCESS");
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            UnsubscribeFromDeliveryEvents();
        }
    }
}