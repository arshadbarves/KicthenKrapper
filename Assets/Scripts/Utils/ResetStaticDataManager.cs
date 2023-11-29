using UnityEngine;


namespace KitchenKrapper
{
    public class ResetStaticDataManager : MonoBehaviour
    {

        private void Awake()
        {
            CuttingCounter.ResetStaticData();
            BaseStation.ResetStaticData();
            TrashCounter.ResetStaticData();
            Player.ResetStaticData();
        }
    }
}