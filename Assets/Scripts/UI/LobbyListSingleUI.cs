using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices.Samples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class LobbyListSingleUI : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI OwnerNameTxt;
        [SerializeField] private TextMeshProUGUI MembersTxt;
        [SerializeField] private TextMeshProUGUI LobbyNameText;

        public Button JoinButton;

        // Metadata
        [HideInInspector]
        public string OwnerName = string.Empty;

        [HideInInspector]
        public int Members = 0;
        [HideInInspector]
        public int MaxMembers = 0;

        [HideInInspector]
        public string LobbyName = string.Empty;

        [HideInInspector]
        public Lobby LobbyRef;
        [HideInInspector]
        public LobbyDetails LobbyDetailsRef;

        private void Awake()
        {
            JoinButton.onClick.AddListener(OnLobbyClicked);
            gameObject.SetActive(false);
        }

        private void OnLobbyClicked()
        {
            EOSKitchenGameLobby.Instance.JoinLobby(LobbyRef, LobbyDetailsRef);
        }

        public void UpdateUI()
        {
            OwnerNameTxt.text = OwnerName;
            MembersTxt.text = string.Format("{0}/{1}", Members, MaxMembers);
            LobbyNameText.text = LobbyName;

            JoinButton.enabled = true;
            gameObject.SetActive(true);
        }
    }
}