using UnityEngine;

namespace KitchenKrapper
{
    public class LoaderUI : MonoBehaviour
    {
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}