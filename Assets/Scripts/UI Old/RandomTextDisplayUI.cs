using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace KitchenKrapper
{
    public class RandomTextDisplayUI : MonoBehaviour
    {
        [SerializeField] private List<string> textList;   // List of strings to display
        [SerializeField] private TextMeshProUGUI textComponent;      // Text component to display the text on

        private void Start()
        {
            // Select a random string from the list and display it
            int randomIndex = Random.Range(0, textList.Count);
            textComponent.text = textList[randomIndex];
        }
    }
}