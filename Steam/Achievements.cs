using System;
using System.Net.Http;
using Newtonsoft.Json;

public class CPHInline
{

    public HttpClient client;
    public string steamKey;
    public string steamUserId;

	public bool Execute()
	{
        try{
            client = new HttpClient();
            steamKey = CPH.GetGlobalVar<string>("steamKey");
            steamUserId = CPH.GetGlobalVar<string>("steamID");

            //get the current game streaming
            TwitchUserInfo user = CPH.TwitchGetBroadcaster();
            string userID = user.UserId;
            TwitchUserInfoEx broadcaster = CPH.TwitchGetExtendedUserInfoById(userID);
            string game = broadcaster.Game;

            CPH.SetGlobalVar("currentGame", game);

            //get the json list games of steam http://api.steampowered.com/ISteamApps/GetAppList/v0002/
            string url = "http://api.steampowered.com/ISteamApps/GetAppList/v0002/";
            
            HttpResponseMessage response = client.GetAsync(url).Result;

            string steamGameId = "";

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                dynamic apps = data.applist.apps;

                foreach (dynamic app in apps)
                {
                    if (app.name == game)
                    {
                        steamGameId = app.appid;
                        break;
                    }
                }
            }

            if (steamGameId == "")
            {
                //CPH.SendMessage("The game is not on steam");
                //Check the game on xbox 
            } else {

                CPH.SetArgument("steamGameId", steamGameId);

                //get the actual achievements of the game
                url = "http://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v0001/?appid=" + steamGameId + "&key=" + steamKey + "&steamid=" + steamUserId;
                response = client.GetAsync(url).Result;

                //do a % of the achievements
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    dynamic achievements = data.playerstats.achievements;

                    int total = 0;
                    int unlocked = 0;

                    foreach (dynamic achievement in achievements)
                    {
                        total++;
                        if (achievement.achieved == 1)
                        {
                            unlocked++;
                        }
                    }

                    if (total > 0)
                    {
                        string percent = unlocked + "/" + total;
                        CPH.SetGlobalVar("steamAchievements" + game, percent);

                    }
                } else {
                    //don't have achievements
                    string percent = "0";
                }
            }

            return true;
        }
        catch (Exception e)
        {
            CPH.LogError("Error: " + e.Message);
            return false;
        }
	}
}


