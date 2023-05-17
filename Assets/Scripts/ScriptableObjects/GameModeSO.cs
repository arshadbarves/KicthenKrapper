using UnityEngine;

[CreateAssetMenu(fileName = "GameModeSO", menuName = "ScriptableObjects/GameModeSO", order = 1)]
public class GameModeSO : ScriptableObject
{
    [SerializeField] private int negativeCoinAmount;
    [SerializeField] private int fastDeliveryTipAmount;
    [SerializeField] private int rewardAmount;
    [SerializeField] private float recipeDeliveryTime;
    [SerializeField] private float gamePlayingTimerMax;

    public int NegativeCoinAmount => negativeCoinAmount;
    public int FastDeliveryTipAmount => fastDeliveryTipAmount;
    public int RewardAmount => rewardAmount;
    public float RecipeDeliveryTime => recipeDeliveryTime;
    public float GamePlayingTimerMax => gamePlayingTimerMax;
}