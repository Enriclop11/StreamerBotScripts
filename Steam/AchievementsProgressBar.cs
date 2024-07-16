using System;
using System.Collections.Generic;

public class CPHInline {
    public bool Execute() {
        // get filesPath global var
        string filesPath = CPH.GetGlobalVar<string>("filesPath");

        //In the directory get the directory steam, if it does not exist create it
        string steamPath = filesPath + "\\steam";
        if (!System.IO.Directory.Exists(steamPath))
        {
            System.IO.Directory.CreateDirectory(steamPath);
        }

        //Get the file achievements.html
        string achievementsPath = steamPath + "\\achievementsBar.html";

        if (!System.IO.File.Exists(achievementsPath))
        {
            System.IO.File.Create(achievementsPath).Close();
        }

        //Get the variable currentGame and achievements
        string currentGame = CPH.GetGlobalVar<string>("currentGame");
        string achievements = CPH.GetGlobalVar<string>("steamAchievements" + currentGame);

        //If the user has no achievements, set it to 0
        if (achievements == null)
        {
            achievements = "0";
        }

        //If achievements are 0 set a empty html
        if (achievements == "0")
        {
            string htmlEmpty = "<html><head><meta http-equiv='refresh' content='10'></head></body></html>";
            System.IO.File.WriteAllText(achievementsPath, htmlEmpty);
            return true;
        }

        // achivements are unlocked/total 
        string unlocked = achievements.Split('/')[0];
        string total = achievements.Split('/')[1];

        // If the user has achievements, create a html with a progress bar with the percentage of achievements
        double percent = (Convert.ToDouble(unlocked) / Convert.ToDouble(total)) * 100;
        //trim percentage to 2 decimal places
        percent = Math.Round(percent, 2);
        int pixelsPercent = (int)percent * 4;

        var map = new Dictionary<string, string>
        {
            { "{{percent}}", percent.ToString() },
            { "{{pixelsPercent}}", pixelsPercent.ToString() },
            { "{{unlocked}}", unlocked },
            { "{{total}}", total }
        };

        //If a file achivementsBarCustom.html exists, use it and change the variables with its values
        string achievementsBarCustomPath = steamPath + "\\achievementsBarCustom.html";

        if (System.IO.File.Exists(achievementsBarCustomPath))
        {
            string htmlCustom = System.IO.File.ReadAllText(achievementsBarCustomPath);
            foreach (var entry in map)
            {
                htmlCustom = htmlCustom.Replace(entry.Key, entry.Value);
            }
            System.IO.File.WriteAllText(achievementsPath, htmlCustom);
        } else {
            // Default html progress bar 
            string htmlDefault = "<html><head><meta http-equiv='refresh' content='10'></head><body><div style='width: 400px; background-color: white; height: 30px; border-radius: 15px; position: relative;'><div style='width: {{pixelsPercent}}px; background-color: #4CAF50; color: black; text-align: center; height: 30px; border-radius: 15px;'><span style='position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); font-family: 'Arial', sans-serif; font-size: 14px; color: black;'>{{percent}}%</span></div></div></body></html>";

            foreach (var entry in map)
            {
                htmlDefault = htmlDefault.Replace(entry.Key, entry.Value);
            }

            System.IO.File.WriteAllText(achievementsPath, htmlDefault);
        }

        return true;
    }
}