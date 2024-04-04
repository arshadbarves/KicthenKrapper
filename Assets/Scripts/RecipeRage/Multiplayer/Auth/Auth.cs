using System.Threading.Tasks;
#if EOS_MULTIPLAYER
using RecipeRage.Multiplayer.EOS;
#endif
#if NAKAMA_MULTIPLAYER
using RecipeRage.Multiplayer.NakamaServer;
#endif

namespace RecipeRage.Multiplayer.Auth
{
    public class Auth : MonoSingleton<Auth>
    {
        public async Task<bool> StartLogin(bool isGuest = false)
        {
            if (isGuest)
            {
#if EOS_MULTIPLAYER
                return await EOSManager.LoginWithDeviceId();
#endif
#if NAKAMA_MULTIPLAYER
                await NakamaManager.LoginWithDeviceId();
#endif
            }
            else
            {
#if EOS_MULTIPLAYER
                return await EOSManager.LoginWithOpenID();
#endif
#if NAKAMA_MULTIPLAYER
                await NakamaManager.LoginWithFacebook();
#endif
            }

            return false;
        }

        public async Task StartLogout()
        {
#if EOS_MULTIPLAYER
            await EOSManager.Logout();
#endif
#if NAKAMA_MULTIPLAYER

            await NakamaManager.Logout();
#endif
        }
    }
}