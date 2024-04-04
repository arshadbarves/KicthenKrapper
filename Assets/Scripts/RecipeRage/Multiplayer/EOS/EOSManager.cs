using System.Threading.Tasks;

namespace RecipeRage.Multiplayer.EOS
{
    public static class EOSManager
    {
        public static async Task<bool> LoginWithDeviceId()
        {
            // await Startup.Instance.NakamaConnection.Connect();
            return false;
        }

        public static async Task<bool> LoginWithOpenID()
        {
            // await Startup.Instance.NakamaConnection.ConnectionClient.AuthenticateFacebookAsync();
            return false;
        }
        
        public static async Task<bool> Logout()
        {
            throw new System.NotImplementedException();
        }
    }
}