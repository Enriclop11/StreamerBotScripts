using System;
using System.Net.Http;
using Newtonsoft.Json;


public class CPHInline {
    public bool Execute () {
        HttpClient client = new HttpClient();
        string steamKey = CPH.GetGlobalVar<string>("steamKey");
        string steamUserId = CPH.GetGlobalVar<string>("steamID");

        string steamGameID = CPH.GetGlobalVar<string>("steamGameID");

        
        //string url = "https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v0001/?appid=227300&key=" + steamKey + "&steamid=" + steamUserId; Get info from all achievements
        /*
        Returns on global achievements overview of a specific game in percentages.

Example: http://api.steampowered.com/ISteamUserStats/GetGlobalAchievementPercentagesForApp/v0002/?gameid=440&format=xml */
        //We need to get the next achievement who is not unlocked by the user and more percentage of players have unlocked it
        string url = "https://api.steampowered.com/ISteamUserStats/GetGlobalAchievementPercentagesForApp/v0002/?gameid=" + steamGameID + "&format=json";
        HttpResponseMessage response = client.GetAsync(url).Result;

        /*
        {achivmentpercentages: {achivements : [{name: "name", percent: 0.0}, {name: "name", percent: 0.0}]}}
        */

        //do a List Map of the response, "name" and "percent" are the keys
        dynamic data = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
        dynamic achievements = data.achievementpercentages.achievements;
        List<Dictionary<string, double>> achievementsPercent = new List<Map<string, double>>();
        foreach (var achievement in achievements) {
            Dictionary<string, double> map = new Dictionary<string, double>();
            map.Add(achievement.name, achievement.percent);
            achievementsPercent.Add(map);
        }

        //sort the list by percent descending
        achievementsPercent.Sort((a, b) => b["percent"].CompareTo(a["percent"]));

        //Get the list of achievements of the user Example URL: http://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v0001/?appid=440&key=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX&steamid=76561197972495328
        url = "http://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v0001/?appid=" + steamGameID + "&key=" + steamKey + "&steamid=" + steamUserId;
        response = client.GetAsync(url).Result;

        /*
        Result data

A list of achievements.

    apiname
        The API name of the achievement
    achieved
        Whether or not the achievement has been completed.
    unlocktime
        Date when the achievement was unlocked.
        */
        dynamic userAchievements = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).playerstats.achievements;
        List<Dictionary<string, bool>> userAchievementsMap = new List<Map<string, string>>();
        foreach (var achievement in userAchievements) {
            Dictionary<string, bool> map = new Dictionary<string, bool>();
            map.Add(achievement.apiname, achievement.achieved);
            userAchievementsMap.Add(map);
        }

        var nextAchievement;

        //Do a for loop of the achivementsPercentages and check if the user has unlocked it in descending order
        foreach (var achievement in achievementsPercent) {
            //Check if the value is not unlocked
            if (!userAchievementsMap[achievement.key]) {
                nextAchievement = achievement;
                return true;
            }
        }

        //Check for the details of the next achievement in https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v0001/?appid=227300&key=C1309E0A7026A5EED4DEE924EBDCBE63&steamid=76561198269683029
        url = "https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v0001/?appid=" + steamGameID + "&key=" + steamKey + "&steamid=" + steamUserId;
        response = client.GetAsync(url).Result;

        /*
        {game: {gameName: "name", availableGameStats: {achievements: [{name: "name", displayName: "name", description: "description", icon: "url"}]}}}
        in the array of achievements we can get the next achievement
        */

        dynamic gameData = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
        dynamic achievements = gameData.game.availableGameStats.achievements;
        //achviements[nexAchievement]
        dynamic nextAchievementData = achievements[nextAchievement.key];

        var name = nextAchievementData.displayName;
        var description = nextAchievementData.description;
        var icon = nextAchievementData.icon;

        //Set in Arguments the next achievement
        CPH.SetArgument("nextAchievement", name);
        CPH.SetArgument("nextAchievementDescription", description);
        CPH.SetArgument("nextAchievementIcon", icon);
    }
}