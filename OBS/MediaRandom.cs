using System;
using System.Collections.Generic;
using System.IO;

public class CPHInline
{
	public bool Execute()
	{
		string source = "media";

		string filesPath = CPH.GetGlobalVar<string>("filesPath");
		string mediaPath = filesPath + "\\media";

		if (mediaPath == null)
		{
			Console.WriteLine("mediaPath is null");
			return false;
		}

		if (CPH.ObsIsConnected() == false)
		{
			CPH.ObsConnect();
		}

		string currentScene = CPH.ObsGetCurrentScene();

		//get Media from the mediaPath, is a folder path to videos
		string[] files = System.IO.Directory.GetFiles(mediaPath, "*.mp4");


		//Check the history of the videos to not repeat the same video
		List<string> history = CPH.GetGlobalVar<List<string>>("history");
		if (history == null)
		{
			history = new List<string>();
			CPH.SetGlobalVar("history", history);
		}

		//get a random video
		Random rnd = new Random();
		string file = files[rnd.Next(files.Length)];

		//check if the video is in the history
		while (history.Contains(file))
		{
			file = files[rnd.Next(files.Length)];
		}

		//add the video to the history and exclude the oldest video
		history.Add(file);
		if (history.Count > 3)
		{
			history.RemoveAt(0);
		}

		//Save the history
		CPH.SetGlobalVar("history", history);

		//Add the video to the queue
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
