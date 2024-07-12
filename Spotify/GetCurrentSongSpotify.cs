using System;
using System.Net.Http;
using Newtonsoft.Json;

public class CPHInline
{
	public bool Execute()
	{
      //Get spotifyToken argument
      
      string spotifyToken = CPH.TryGetArg("spotifyToken", out string token) ? token : "Token";

      CPH.SendMessage("Token: " + spotifyToken);

      //Get the current song
      /*
      curl --request GET \
  --url https://api.spotify.com/v1/me/player/currently-playing \
  --header 'Authorization: Bearer 1POdFZRZbvb...qqillRxMr2z'
      */

      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Add("Authorization", "Bearer " + spotifyToken);

      HttpResponseMessage response = client.GetAsync("https://api.spotify.com/v1/me/player/currently-playing").Result;

      // the response is this
      /*
      {
  "device": {
    "id": "string",
    "is_active": false,
    "is_private_session": false,
    "is_restricted": false,
    "name": "Kitchen speaker",
    "type": "computer",
    "volume_percent": 59,
    "supports_volume": false
  },
  "repeat_state": "string",
  "shuffle_state": false,
  "context": {
    "type": "string",
    "href": "string",
    "external_urls": {
      "spotify": "string"
    },
    "uri": "string"
  },
  "timestamp": 0,
  "progress_ms": 0,
  "is_playing": false,
  "item": {
    "album": {
      "album_type": "compilation",
      "total_tracks": 9,
      "available_markets": [
        "CA",
        "BR",
        "IT"
      ],
      "external_urls": {
        "spotify": "string"
      },
      "href": "string",
      "id": "2up3OPMp9Tb4dAKM2erWXQ",
      "images": [
        {
          "url": "https://i.scdn.co/image/ab67616d00001e02ff9ca10b55ce82ae553c8228",
          "height": 300,
          "width": 300
        }
      ],
      "name": "string",
      "release_date": "1981-12",
      "release_date_precision": "year",
      "restrictions": {
        "reason": "market"
      },
      "type": "album",
      "uri": "spotify:album:2up3OPMp9Tb4dAKM2erWXQ",
      "artists": [
        {
          "external_urls": {
            "spotify": "string"
          },
          "href": "string",
          "id": "string",
          "name": "string",
          "type": "artist",
          "uri": "string"
        }
      ]
    },
    "artists": [
      {
        "external_urls": {
          "spotify": "string"
        },
        "followers": {
          "href": "string",
          "total": 0
        },
        "genres": [
          "Prog rock",
          "Grunge"
        ],
        "href": "string",
        "id": "string",
        "images": [
          {
            "url": "https://i.scdn.co/image/ab67616d00001e02ff9ca10b55ce82ae553c8228",
            "height": 300,
            "width": 300
          }
        ],
        "name": "string",
        "popularity": 0,
        "type": "artist",
        "uri": "string"
      }
    ],
    "available_markets": [
      "string"
    ],
    "disc_number": 0,
    "duration_ms": 0,
    "explicit": false,
    "external_ids": {
      "isrc": "string",
      "ean": "string",
      "upc": "string"
    },
    "external_urls": {
      "spotify": "string"
    },
    "href": "string",
    "id": "string",
    "is_playable": false,
    "linked_from": {},
    "restrictions": {
      "reason": "string"
    },
    "name": "string",
    "popularity": 0,
    "preview_url": "string",
    "track_number": 0,
    "type": "track",
    "uri": "string",
    "is_local": false
  },
  "currently_playing_type": "string",
  "actions": {
    "interrupting_playback": false,
    "pausing": false,
    "resuming": false,
    "seeking": false,
    "skipping_next": false,
    "skipping_prev": false,
    "toggling_repeat_context": false,
    "toggling_shuffle": false,
    "toggling_repeat_track": false,
    "transferring_playback": false
  }
}
      */

      if (response.IsSuccessStatusCode)
      {
        string responseBody = response.Content.ReadAsStringAsync().Result;
        dynamic json = JsonConvert.DeserializeObject(responseBody);

        string songName = json.item.name;
        string artistName = json.item.artists[0].name;
        string albumName = json.item.album.name;
        string albumImage = json.item.album.images[0].url;

        CPH.SendMessage("Song: " + songName + " Artist: " + artistName + " Album: " + albumName + " Album Image: " + albumImage);
      }
      else
      {
        CPH.SendMessage("The request to the spotify API failed");
        CPH.LogError("The request to the spotify API failed");
        return false;
      }
		return true;
	}
}
