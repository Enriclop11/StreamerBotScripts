using System;
using System.Collections.Generic;

public class CPHInline {
	public bool Execute() {
        //Get global vars
        string source = "media";

		string filesPath = CPH.GetGlobalVar<string>("filesPath");
		string mediaPath = filesPath + "\\mediaByName";

        if (filesPath == null)
        {
            Console.WriteLine("filesPath is null");
        }

        if (CPH.ObsIsConnected() == false)
        {
            CPH.ObsConnect();
        }

    	string currentScene = CPH.ObsGetCurrentScene();

		string[] files = System.IO.Directory.GetFiles(mediaPath, "*.mp4");

        string rewardName = CPH.TryGetArg("rewardName", out string reward) ? reward : "Reward";

        string file = null;

        foreach (string f in files)
        {
            if (f.Contains(rewardName))
            { 
                string fName = System.IO.Path.GetFileName(f);
                if (fName == rewardName + ".mp4")
                {
                    file = f;
                    break;
                }
            }
        }

        if (file == null)
        {
            Console.WriteLine("Video not found");
            return false;
        }

        List<string> playerQueue = CPH.GetGlobalVar<List<string>>("playerQueue");
        if (playerQueue == null)
        {
            playerQueue = new List<string>();
        }
        playerQueue.Add(file);
        CPH.SetGlobalVar("playerQueue", playerQueue);

		return true;
    }
}