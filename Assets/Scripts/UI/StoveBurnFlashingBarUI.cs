using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour
{
    private const string BURN_ANIMATION_TRIGGER = "IsFlashing";

    [SerializeField] private StoveCounter stoveCounter;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
        animator.SetBool(BURN_ANIMATION_TRIGGER, false);
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.ProgressChangedEventArgs e)
    {
        float burnShowProgress = 0.5f;
        bool shouldShow = stoveCounter.IsDone() && e.progressNormalized >= burnShowProgress;
        animator.SetBool(BURN_ANIMATION_TRIGGER, shouldShow);
    }
}
