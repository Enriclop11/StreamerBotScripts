using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        string spotifyToken = CPH.GetGlobalVar<string>("spotifyToken");
        DateTime expirationTime = CPH.GetGlobalVar<DateTime>("spotifyTokenExpiration");

        //Get arg rawInput0
        string songName = CPH.TryGetArg("rawInput", out string uri) ? uri : "spotify:track:4iV5W9uYEdYUVa79Axb7Rh";

        if (DateTime.Now > expirationTime)
        {
            CPH.RegisterCustomTrigger("spotifyTokenExpired", "spotifyTokenExpired", new string[] { "spotify" });
            CPH.TriggerCodeEvent("spotifyTokenExpired");
        };

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + spotifyToken);

        //search for the song and get the uri
        HttpResponseMessage response = client.GetAsync("https://api.spotify.com/v1/search?q=" + songName + "&type=track").Result;

        string songUri = "";
        if (response.IsSuccessStatusCode)
        {
            string responseBody = response.Content.ReadAsStringAsync().Result;
            dynamic json = JsonConvert.DeserializeObject(responseBody);

            songUri = json.tracks.items[0].uri;
        }
        else
        {
            CPH.LogError("The request to the spotify API failed");
            CPH.SendMessage("Song doesn't exist");
            return false;
        }

        //add the song to the queue
        response = client.PostAsync("https://api.spotify.com/v1/me/player/queue?uri=" + songUri, null).Result;

        if (response.IsSuccessStatusCode)
        {
            CPH.LogDebug("Song added to queue");
            CPH.SendMessage("Song added to queue");
        }
        else
        {
            CPH.LogError("The request to the spotify API failed");
            CPH.SendMessage("Song doesn't exist");
            return false;
        }

        return true;
    }
}