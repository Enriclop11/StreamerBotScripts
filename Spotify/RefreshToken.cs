using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

public class CPHInline {
    public bool Execute() {

        string spotifyRefreshToken = CPH.GetGlobalVar<string>("spotifyRefreshToken");
        string spotifyId = CPH.GetGlobalVar<string>("spotifyId");

        string scope = "user-read-private user-read-email user-read-playback-state user-modify-playback-state user-read-currently-playing";
        /*
          const getRefreshToken = async () => {

   // refresh token that has been previously stored
   const refreshToken = localStorage.getItem('refresh_token');
   const url = "https://accounts.spotify.com/api/token";

    const payload = {
      method: 'POST',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded'
      },
      body: new URLSearchParams({
        grant_type: 'refresh_token',
        refresh_token: refreshToken,
        client_id: clientId
      }),
    }
    const body = await fetch(url, payload);
    const response await body.json();

    localStorage.setItem('access_token', response.accessToken);
    localStorage.setItem('refresh_token', response.refreshToken);
  }
        */

        HttpClient client = new HttpClient();

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
            string refreshToken = json.refresh_token;

            // Set in global var
            CPH.SetGlobalVar("spotifyToken", token);
            CPH.SetGlobalVar("spotifyRefreshToken", refreshToken);

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