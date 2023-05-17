using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class OpenIDManager : MonoBehaviour
{
    protected static OpenIDManager s_instance = null;
    public static OpenIDManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                return new GameObject("OpenIDManager").AddComponent<OpenIDManager>();
            }
            else
            {
                return s_instance;
            }
        }
    }

    [SerializeField]
    private string authorizationEndpoint = "http://192.168.1.96:8000/o/authorize/";
    [SerializeField] private string tokenEndpoint = "http://192.168.1.96:8000/o/token/";
    [SerializeField] private string clientId = "eHRiDENK6teWTMKIkYlq9tvSnUoj221OWHIPNr1e";
    [SerializeField] private string clientSecret = "HVgtykSgpnlyaDpysbBLxG97GvxTN6Y1ywgAwRLVATkHQYMm5PdMMk31dfTZu9DZQJEnOjCaMcJvf72wc8AUr2isI3XpVHgQMp7gE6tZp5PhxBs6j2PweCxThHanlILY";
    [SerializeField] private string redirectUri = "http://127.0.0.1:54545/callback/";
    [SerializeField] private string scope = "openid email profile";
    [SerializeField] private string responseType = "code";
    [SerializeField] private string codeChallengeMethod = "S256";
    [SerializeField] private string codeVerifier;
    [SerializeField] private string codeChallenge;
    [SerializeField] private string authorizationCode;
    [SerializeField] private string accessToken;
    [SerializeField] private string refreshToken;
    [SerializeField] private string idToken;

    public class TokenReader
    {
        public string access_token;
        public string refresh_token;
        public string scope;
        public string token_type;
        public string id_token;
        public float expires_in;
    }

    private TokenReader cachedToken = null;
    private DateTime tokenExpiration;
    private event Action<TokenReader> onTokenReceived;


    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void RequestOAuth2Token(Action<TokenReader> callback)
    {
        onTokenReceived += callback;

        if (cachedToken != null && DateTime.Now < tokenExpiration)
        {
            onTokenReceived?.Invoke(cachedToken);
            cachedToken = null; // Consume the token so it can't be used again.
            return;
        }
        else
        {
            cachedToken = null;
        }

        // Generate a random code verifier
        codeVerifier = GenerateCodeVerifier(64);

        // Calculate the code challenge
        codeChallenge = CalculateCodeChallenge(codeVerifier);

        GetTokenAsync().ContinueWith(GetTokenAsyncComplete);
    }

    public static string GenerateCodeVerifier(int length)
    {
        const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~";
        StringBuilder sb = new StringBuilder();
        System.Random random = new System.Random();
        for (int i = 0; i < length; i++)
        {
            sb.Append(allowedChars[random.Next(allowedChars.Length)]);
        }
        return sb.ToString();
    }

    public static string CalculateCodeChallenge(string codeVerifier)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(codeVerifier);
        byte[] hashBytes;
        using (SHA256 sha256 = SHA256.Create())
        {
            hashBytes = sha256.ComputeHash(bytes);
        }
        string codeChallenge = Base64UrlEncode(hashBytes);
        return codeChallenge;
    }

    public static string Base64UrlEncode(byte[] data)
    {
        string base64 = Convert.ToBase64String(data);
        string base64Url = base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        return base64Url;
    }

    private void GetTokenAsyncComplete(Task<TokenReader> tokenTask)
    {
        TokenReader tokenContent = null;
        if (tokenTask.IsCompleted)
        {
            tokenContent = tokenTask.Result;
        }

        if (tokenContent != null)
        {
            cachedToken = tokenContent;
            tokenExpiration = DateTime.Now.AddSeconds(tokenContent.expires_in);
        }
        else
        {
            cachedToken = null;
        }

        onTokenReceived?.Invoke(cachedToken);
        onTokenReceived = null;
    }
    private async Task<string> GetAuthCode()
    {
        // create a local listen server to receive redirect callback from the authorization server
        var listener = new HttpListener();
        listener.Prefixes.Add(redirectUri);
        listener.Start();

        // wait for the callback
        HttpListenerContext context = await listener.GetContextAsync();
        using HttpListenerResponse response = context.Response;

        // get the authorization code from the callback
        string code = context.Request.QueryString.Get("code");

        // response.StatusCode = 200;
        // response.StatusDescription = "OK";
        // response.Close();

        // Read the contents of the embedded resource HTML file into a string variable
        print(Application.dataPath);
        string resourceName = Application.dataPath + "/Scripts/HTML/redirect.html";
        if (string.IsNullOrEmpty(code))
        {
            resourceName = Application.dataPath + "/Scripts/HTML/authorization_failed.html";
        }
        else
        {
            resourceName = Application.dataPath + "/Scripts/HTML/authorization_success.html";
        }

        string html = "";

        using (StreamReader reader = new StreamReader(resourceName))
        {
            html = reader.ReadToEnd();
        }

        // Replace the code placeholder with the actual authorization code
        // html = html.Replace("{{code}}", code);

        // Convert the string to a byte array and send it back to the browser
        byte[] buffer = Encoding.UTF8.GetBytes(html);
        response.ContentLength64 = buffer.Length;

        using Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        listener.Stop();
        html += "<script>closeWindow();</script>";

        return code;
    }

    private async Task<TokenReader> GetTokenAsync()
    {
        Application.OpenURL(authorizationEndpoint + "?client_id=" + clientId + "&redirect_uri=" + redirectUri + "&scope=" + scope + "&response_type=" + responseType + "&code_challenge_method=" + codeChallengeMethod + "&code_challenge=" + codeChallenge);

        string authorizationCode = await Task.Run(GetAuthCode);

        if (authorizationCode == null)
        {
            return null;
        }

        TokenReader tokenContent = null;
        try
        {
            HttpClient httpClient = new HttpClient();

            // Create a dictionary to hold the request parameters
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            requestParams.Add("client_id", clientId);
            requestParams.Add("client_secret", clientSecret);
            requestParams.Add("code", authorizationCode);
            requestParams.Add("code_verifier", codeVerifier);
            requestParams.Add("redirect_uri", redirectUri);
            requestParams.Add("grant_type", "authorization_code");

            // Convert the dictionary to FormUrlEncodedContent
            FormUrlEncodedContent content = new FormUrlEncodedContent(requestParams);

            // Send the POST request with FormUrlEncodedContent as the content
            HttpResponseMessage response = await httpClient.PostAsync(tokenEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();
                Debug.Log("Response: " + responseJson);
                tokenContent = JsonUtility.FromJson<TokenReader>(responseJson);

                if (string.IsNullOrEmpty(tokenContent.access_token))
                {
                    tokenContent = null;
                }
            }
            else
            {
                Debug.LogError("Error: " + response.StatusCode + " " + response.ReasonPhrase);
            }

            httpClient.Dispose();
        }
        catch (HttpRequestException ex)
        {
            Debug.LogError("HTTP request failed: " + ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error: " + ex.Message);
        }

        return tokenContent;
    }

    public void GetAccessToken(string refreshToken, Action<TokenReader> callback)
    {
        onTokenReceived += callback;
        GetAsyncAccessTokenWithRefreshToken(refreshToken).ContinueWith(GetTokenAsyncComplete);
    }

    private async Task<TokenReader> GetAsyncAccessTokenWithRefreshToken(string refreshToken)
    {
        TokenReader tokenContent = null;
        try
        {
            HttpClient httpClient = new HttpClient();

            // Create a dictionary to hold the request parameters
            Dictionary<string, string> requestParams = new Dictionary<string, string>();
            requestParams.Add("client_id", clientId);
            requestParams.Add("client_secret", clientSecret);
            requestParams.Add("refresh_token", refreshToken);
            requestParams.Add("grant_type", "refresh_token");

            // Convert the dictionary to FormUrlEncodedContent
            FormUrlEncodedContent content = new FormUrlEncodedContent(requestParams);

            // Send the POST request with FormUrlEncodedContent as the content
            HttpResponseMessage response = await httpClient.PostAsync(tokenEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();
                Debug.Log("Response: " + responseJson);
                tokenContent = JsonUtility.FromJson<TokenReader>(responseJson);

                if (string.IsNullOrEmpty(tokenContent.access_token))
                {
                    tokenContent = null;
                }
            }
            else
            {
                Debug.LogError("Error: " + response.StatusCode + " " + response.ReasonPhrase);
            }

            httpClient.Dispose();
        }
        catch (HttpRequestException ex)
        {
            Debug.LogError("HTTP request failed: " + ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error: " + ex.Message);
        }

        return tokenContent;
    }
}
