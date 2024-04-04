using UnityEngine;

namespace RecipeRage.Multiplayer.NakamaServer
{
    [CreateAssetMenu(menuName = "Nakama/Connection data")]
    public class NakamaConnectionData : ScriptableObject
    {
        [SerializeField] private string scheme = "http";
        [SerializeField] private string host = "localhost";
        [SerializeField] private int port = 7350;
        [SerializeField] private string serverKey = "defaultkey";
        
        public string Scheme => scheme;
        public string Host => host;
        public int Port => port;
        public string ServerKey => serverKey;
    }
}