using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

public class CPHInline {
    public bool Execute() {

        string spotifyRefreshToken = CPH.GetGlobalVar<string>("spotifyRefreshToken");
        string spotifyId = CPH.GetGlobalVar<string>("spotifyId");
        string spotifySecret = CPH.GetGlobalVar<string>("spotifyKey");

        string scope = "user-read-private user-read-email user-read-playback-state user-modify-playback-state user-read-currently-playing";

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(spotifyId + ":" + spotifySecret)));


        var values = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", spotifyRefreshToken },
            { "client_id", spotifyId }
        };

        var content = new FormUrlEncodedContent(values);

        HttpResponseMessage response = client.PostAsync("https://accounts.spotify.com/api/token", content).Result;

        if (response.IsSuccessStatusCode)
        {
            string responseBody = response.Content.ReadAsStringAsync().Result;
            dynamic json = JsonConvert.DeserializeObject(responseBody);

            string token = json.access_token;
            string expiresIn = json.expires_in;

            DateTime expirationTime = DateTime.Now.AddSeconds(Convert.ToDouble(expiresIn));

            // Set in global var
            CPH.SetGlobalVar("spotifyToken", token);
            CPH.SetGlobalVar("spotifyExpirationTime", expirationTime.ToString());

            CPH.LogDebug("Token: " + token);
            
        }
        else
        {
            CPH.SendMessage("Token refresh failed" + response.StatusCode);
            CPH.LogError("The request to the spotify API failed");
            return false;
        }

        return true;
    }
}