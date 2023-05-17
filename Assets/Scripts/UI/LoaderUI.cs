using UnityEngine;

public class LoaderUI : MonoBehaviour, IBaseUI
{
    public void Hide()
    {
        // Set this panel to inactive.
        gameObject.SetActive(false);
    }

    public void Show()
    {
        // Set this panel to active.
        gameObject.SetActive(true);
    }
}
