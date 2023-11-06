using UnityEngine;

namespace KitchenKrapper
{
    public class DeliveryCounter : BaseStation
    {
        public static DeliveryCounter Instance { get; private set; }

        private void Awake()
        {
            SingletonCheck();
        }

        private void SingletonCheck()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public override void Interact(PlayerController player)
        {
            HandlePlayerInteraction(player);
        }

        private void HandlePlayerInteraction(PlayerController player)
        {
            if (player.HasKitchenObject())
            {
                HandleKitchenObject(player.GetKitchenObject());
            }
        }

        private void HandleKitchenObject(KitchenObject kitchenObject)
        {
            if (kitchenObject.TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
            {
                DeliverPlate(plateKitchenObject);
            }
        }

        private void DeliverPlate(PlateKitchenObject plateKitchenObject)
        {
            DeliveryManager.Instance.DeliveryRecipe(plateKitchenObject);
            KitchenObject.DestroyKitchenObject(plateKitchenObject);
            StepComplete();
        }
    }
}
