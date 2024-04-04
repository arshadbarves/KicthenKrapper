using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class SetPlayerNameUI : MonoBehaviour
    {
        [SerializeField] private Button m_setPlayerNameButton;
        [SerializeField] private TMP_InputField m_playerNameText;

        private void Awake()
        {
            m_setPlayerNameButton.onClick.AddListener(OnSetPlayerNameButtonClicked);
        }

        private void OnSetPlayerNameButtonClicked()
        {
            if (m_playerNameText.text.Length > 0)
            {
                GameManager.Instance.CreatePlayerData(m_playerNameText.text);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Player name cannot be empty");
            }
        }
    }
}