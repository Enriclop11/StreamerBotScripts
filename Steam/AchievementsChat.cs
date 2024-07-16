using System;

public class CPHInline
{
    public bool Execute()
    {
        // Get the current game in global var and then with it get the achievements from the global var
        string currentGame = CPH.GetGlobalVar<string>("currentGame");
        string achievements = CPH.GetGlobalVar<string>("steamAchievements" + currentGame);

        // If the user has no achievements, set it to 0
        if (achievements == null)
        {
            achievements = "0";
        }

        if (achievements == "0")
        {
            CPH.SendMessage("You have no achievements in " + currentGame);
        }
        else
        {
            // achivements are unlocked/total 
            string unlocked = achievements.Split('/')[0];
            string total = achievements.Split('/')[1];
            // Calculate the percentage of achievements
            double percent = (Convert.ToDouble(unlocked) / Convert.ToDouble(total)) * 100;
            //trim percentage to 2 decimal places
            percent = Math.Round(percent, 2);
            CPH.SendMessage("Achievements unlocked in " + currentGame +  ": " + unlocked + "/" + total + " (" + percent + "%)");
        }

        return true;
    }
}