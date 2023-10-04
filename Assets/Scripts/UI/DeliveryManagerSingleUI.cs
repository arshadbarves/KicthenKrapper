using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class DeliveryManagerSingleUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI recipeText;
        [SerializeField] private Transform iconContainer;
        [SerializeField] private Transform iconTemplate;
        [SerializeField] private Image recipeProgressImage;
        [SerializeField] private Gradient recipeProgressGradient;

        private float deliveryTime;
        private Recipe recipeInstance;

        private void Awake()
        {
            iconTemplate.gameObject.SetActive(false);
        }

        private void Update()
        {
            UpdateDeliveryProgress();
            UpdateProgressBarColor();
        }

        private void UpdateDeliveryProgress()
        {
            deliveryTime -= Time.deltaTime;
            float deliveryTimeRatio = deliveryTime / LevelManager.Instance.GetRecipeDeliveryTime();
            recipeProgressImage.fillAmount = deliveryTimeRatio;
        }

        private void UpdateProgressBarColor()
        {
            if (deliveryTime < LevelManager.Instance.GetRecipeDeliveryTime() * 0.5f)
            {
                recipeProgressImage.color = new Color(1, 0.92f, 0.016f, 0.78f);
            }
            if (deliveryTime < LevelManager.Instance.GetRecipeDeliveryTime() * 0.2f)
            {
                recipeProgressImage.color = new Color(1f, 0f, 0f, 0.78f);
            }
        }

        public void SetRecipe(Recipe recipe)
        {
            recipeInstance = recipe;
            recipeText.text = recipe.GetRecipeSO().recipeName;
            deliveryTime = LevelManager.Instance.GetRecipeDeliveryTime();
            recipeProgressImage.fillAmount = 1f;

            DestroyExistingIcons();

            foreach (KitchenObjectSO kitchenObjectSO in recipe.GetRecipeSO().kitchenObjectSOList)
            {
                CreateIcon(kitchenObjectSO);
            }
        }

        private void DestroyExistingIcons()
        {
            foreach (Transform child in iconContainer)
            {
                if (child == iconTemplate) continue;
                Destroy(child.gameObject);
            }
        }

        private void CreateIcon(KitchenObjectSO kitchenObjectSO)
        {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }

        public Recipe GetRecipe()
        {
            return recipeInstance;
        }
    }
}