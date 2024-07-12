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

        /*
        var client_id = 'CLIENT_ID';
var client_secret = 'CLIENT_SECRET';

var authOptions = {
  url: 'https://accounts.spotify.com/api/token',
  headers: {
    'Authorization': 'Basic ' + (new Buffer.from(client_id + ':' + client_secret).toString('base64'))
  },
  form: {
    grant_type: 'client_credentials'
  },
  json: true
};

request.post(authOptions, function(error, response, body) {
  if (!error && response.statusCode === 200) {
    var token = body.access_token;
  }
});
    }
        */
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(spotifyId + ":" + spotifySecret)));

        var values = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" }
        };

        var content = new FormUrlEncodedContent(values);

        HttpResponseMessage response = client.PostAsync("https://accounts.spotify.com/api/token", content).Result;

        if (response.IsSuccessStatusCode)
        {
            string responseBody = response.Content.ReadAsStringAsync().Result;
            dynamic json = JsonConvert.DeserializeObject(responseBody);

            string token = json.access_token;

            // Set in argument
            CPH.SetArgument("spotifyToken", token);

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