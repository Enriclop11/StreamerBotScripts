using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        // Get spotifyKey and spotifyID
        string spotifySecret = CPH.GetGlobalVar<string>("spotifyKey");
        string spotifyId = CPH.GetGlobalVar<string>("spotifyId");

        // Get arg spotfyCode
        string spotifyCode = CPH.TryGetArg("spotifyCode", out string code) ? code : "Code";

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(spotifyId + ":" + spotifySecret)));

        var values = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", spotifyCode },
            { "redirect_uri", "http://localhost:8888/callback" }
        };

        var content = new FormUrlEncodedContent(values);

        HttpResponseMessage response = client.PostAsync("https://accounts.spotify.com/api/token", content).Result;

        if (response.IsSuccessStatusCode)
        {
            string responseBody = response.Content.ReadAsStringAsync().Result;
            dynamic json = JsonConvert.DeserializeObject(responseBody);

            string token = json.access_token;
            string refreshToken = json.refresh_token;
            string expiresIn = json.expires_in;

            //calculate the expiration time
            DateTime expirationTime = DateTime.Now.AddSeconds(Convert.ToDouble(expiresIn));

            // Set in global var
            CPH.SetGlobalVar("spotifyToken", token);
            CPH.SetGlobalVar("spotifyRefreshToken", refreshToken);
            CPH.SetGlobalVar("spotifyExpirationTime", expirationTime.ToString());

            CPH.LogDebug("Token: " + token);
        }
        else
        {
            CPH.LogError("The request to the spotify API failed");
            return false;
        }
        return true;
    }
}