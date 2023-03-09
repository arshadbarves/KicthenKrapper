using UnityEngine;
using TMPro;
namespace UniversalMobileController
{
    public class UniversalMobileController_Manager : MonoBehaviour
    {
        public static bool editMode = false;
        public GameObject resizeSlider;
        public TextMeshProUGUI editButtonText;
        public void PressEditMode()
        {
            if (editMode)
            {
                editButtonText.text = "Edit";
                editMode = false;
                resizeSlider.SetActive(false);
            }
            else
            {
                resizeSlider.SetActive(true);
                editButtonText.text = "Exit";
                editMode = true;
            }
        }
    }
}
