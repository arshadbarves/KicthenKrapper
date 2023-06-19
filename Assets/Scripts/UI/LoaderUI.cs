using UnityEngine;

public class LoaderUI : MonoBehaviour, IBaseUI
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
