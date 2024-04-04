using System.Threading.Tasks;
using Nakama;
using RecipeRage.Managers;

namespace RecipeRage.Multiplayer.NakamaServer
{
    public static class NakamaAuth
    {
        public static async Task LoginWithDeviceId()
        {
        }

        public static async Task<ISession> LoginWithFacebook()
        {
            // await Startup.Instance.NakamaConnection.ConnectionClient.AuthenticateFacebookAsync();
            return null;
        }
    }
}