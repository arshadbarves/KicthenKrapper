using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        deliveryTime -= Time.deltaTime;
        recipeProgressImage.fillAmount = deliveryTime / GameManager.Instance.GetRecipeDeliveryTime();
        // Change color of the progress bar using the delivery time with a threshold.
        if (deliveryTime < GameManager.Instance.GetRecipeDeliveryTime() * 0.5f)
        {
            recipeProgressImage.color = new Color(1, 0.92f, 0.016f, 0.78f);
        }
        if (deliveryTime < GameManager.Instance.GetRecipeDeliveryTime() * 0.2f)
        {
            recipeProgressImage.color = new Color(1f, 0f, 0f, 0.78f);
        }

    }
    public void SetRecipe(Recipe recipe)
    {
        recipeInstance = recipe;
        recipeText.text = recipe.GetRecipeSO().recipeName;
        deliveryTime = GameManager.Instance.GetRecipeDeliveryTime();
        recipeProgressImage.fillAmount = 1f;


        foreach (Transform child in iconContainer)
        {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipe.GetRecipeSO().kitchenObjectSOList)
        {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
    }

    public Recipe GetRecipe()
    {
        return recipeInstance;
    }
}
