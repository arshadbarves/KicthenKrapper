using UnityEngine;

namespace KitchenKrapper
{
    public class TutorialSteps : MonoBehaviour
    {
        private int currentStepIndex;

        // Getter for the object position
        public Transform GetObjectPosition()
        {
            return this.transform;
        }
    }
}
