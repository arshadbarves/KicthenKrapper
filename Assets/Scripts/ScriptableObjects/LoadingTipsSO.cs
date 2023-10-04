using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
    [CreateAssetMenu(fileName = "LoadingTips", menuName = "KitchenKrapper/LoadingTips", order = 11)]
    public class LoadingTipsSO : ScriptableObject
    {
        [SerializeField] private List<string> tipsList;

        public string GetRandomTip()
        {
            return tipsList[Random.Range(0, tipsList.Count)];
        }
    }
}