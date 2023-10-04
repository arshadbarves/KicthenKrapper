using UnityEngine;

namespace KitchenKrapper
{
    public class StoveBurnWarningUI : MonoBehaviour
    {
        [SerializeField] private StoveCounter stoveCounter;

        private void Start()
        {
            stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
            Hide();
        }

        private void StoveCounter_OnProgressChanged(object sender, IHasProgress.ProgressChangedEventArgs e)
        {
            float burnShowProgress = 0.5f;
            bool shouldShow = stoveCounter.IsDone() && e.progressNormalized >= burnShowProgress;
            if (shouldShow)
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