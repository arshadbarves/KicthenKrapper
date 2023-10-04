using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class PlateIconSingleUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        public void SetIcon(KitchenObjectSO ingredient)
        {
            iconImage.sprite = ingredient.sprite;
        }
    }
}