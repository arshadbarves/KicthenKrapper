using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RecipeRage.Utilities
{
    public static class InternetConnectionManager
    {
        private const string URL = "https://www.google.com";
        private static CancellationTokenSource _cancellationTokenSource;

        public static void StartCheckingInternetConnection(float intervalInSeconds, Action<bool> callback)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    bool isConnected = await CheckInternetConnectionAsync();
                    callback(isConnected);
                    await Task.Delay(TimeSpan.FromSeconds(intervalInSeconds), token);
                }
            }, token);
        }

        public static void StopCheckingInternetConnection()
        {
            _cancellationTokenSource?.Cancel();
        }

        private static async Task<bool> CheckInternetConnectionAsync()
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(URL);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Debugger.LogError(e.Message);
                return false;
            }
        }
    }
}
