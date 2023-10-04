using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private GameObject hasProgressGameObject;
        [SerializeField] private Image progressBarImage;

        private IHasProgress hasProgress;

        private void Start()
        {
            hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
            if (hasProgress == null)
            {
                Debug.LogError("Game Object" + hasProgressGameObject + " does not have IHasProgress component");
            }
            hasProgress.OnProgressChanged += HasProgressCounter_OnProgressChanged;
            progressBarImage.fillAmount = 0f;
            Hide();
        }

        private void HasProgressCounter_OnProgressChanged(object sender, IHasProgress.ProgressChangedEventArgs e)
        {
            progressBarImage.fillAmount = e.progressNormalized;
            if (e.progressNormalized > 0f && e.progressNormalized < 1f)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}