using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingleUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    public void SetIcon(KitchenObjectSO ingredient)
    {
        iconImage.sprite = ingredient.sprite;
    }
}
