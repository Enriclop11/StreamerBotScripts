using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

public class CPHInline
{
  public bool Execute()
  {
    //Get spotifyToken global var and the expiration time

    string spotifyToken = CPH.GetGlobalVar<string>("spotifyToken");
    DateTime expirationTime = DateTime.Parse(CPH.GetGlobalVar<string>("spotifyExpirationTime"));

    //Check if the token is expired
    if (DateTime.Now > expirationTime)
    {
      CPH.RegisterCustomTrigger("spotifyTokenExpired", "spotifyTokenExpired", new string[] { "spotify" });
      CPH.TriggerCodeEvent("spotifyTokenExpired");
    };

    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + spotifyToken);

    HttpResponseMessage response = client.GetAsync("https://api.spotify.com/v1/me/player/currently-playing").Result;

    if (response.IsSuccessStatusCode)
    {
      string responseBody = response.Content.ReadAsStringAsync().Result;
      dynamic json = JsonConvert.DeserializeObject(responseBody);

      string songName = json.item.name;
      string artistName = json.item.artists[0].name;
      string albumName = json.item.album.name;
      string albumImage = json.item.album.images[0].url;

      //CPH.SendMessage("Song: " + songName + " Artist: " + artistName);

      //Set the variables in the arguments
      CPH.SetArgument("songName", songName);
      CPH.SetArgument("artistName", artistName);
      CPH.SetArgument("albumName", albumName);
      CPH.SetArgument("albumImage", albumImage);
    }
    else
    {
      CPH.LogError("The request to the spotify API failed");
      return false;
    }
    return true;
  }
}
