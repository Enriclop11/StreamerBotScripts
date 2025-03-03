using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;

public class CPHInline {
    string source = "media";

    public bool Execute() {
        bool playing = CPH.GetGlobalVar<bool>("playingMedia");
        if (playing) return false;

        CPH.SetGlobalVar("playingMedia", true);
        List<string> playerQueue = CPH.GetGlobalVar<List<string>>("playerQueue") ?? new List<string>();

        if (!CPH.ObsIsConnected()) CPH.ObsConnect();
        if (playerQueue.Count == 0) return false;

        while (playerQueue.Count > 0) {
            string file = playerQueue[0];
            playerQueue.RemoveAt(0);
            CPH.SetGlobalVar("playerQueue", playerQueue);
            PlayMedia(file);
            playerQueue = CPH.GetGlobalVar<List<string>>("playerQueue");
        }

        CPH.SetGlobalVar("playingMedia", false);
        return true;
    }

    public void PlayMedia(string file) {
        string currentScene = CPH.ObsGetCurrentScene();
        CPH.ObsSetSourceVisibility(currentScene, source, true);
        CPH.ObsSetMediaSourceFile(currentScene, source, "");
        CPH.ObsSetSourceVisibility(currentScene, source, false);
        
        Thread.Sleep(1000);
        CPH.ObsSetMediaSourceFile(currentScene, source, file);
        CPH.ObsSetSourceVisibility(currentScene, source, true);
        CPH.ObsMediaPlay(currentScene, source);

        CPH.ObsSetSourceVisibility(currentScene, source, true);

		//show the video in the current scene CPH.ObsSetMediaSourceFile(scene, source, file);
		CPH.ObsSetMediaSourceFile(currentScene, source, file);

		//get SceneItem raw
		string sceneItem = CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\"" + currentScene + "\",\"sourceName\":\"" + source + "\",\"searchOffset\":null}", 0);

		//transform the sceneItem to a random postition CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":null,\"sceneItemId\":0,\"sceneItemTransform\":null}", 0);
		Random rnd = new Random();
        int positionX = rnd.Next(0, 1620);
		int positionY = rnd.Next(0, 880);
		CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\"" + currentScene + "\",\"sceneItemId\":" + sceneItem + ",\"sceneItemTransform\":{\"positionX\":" + positionX + ",\"positionY\":" + positionY + "}}", 0);

		//play the video
		CPH.ObsMediaPlay(currentScene, source);

        // Get media duration
        double durationMs = GetMediaDuration(file);
        if (durationMs > 0) Thread.Sleep((int)durationMs);
    }

    private double GetMediaDuration(string file) {
        try {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri(file));
            Thread.Sleep(1000); // Wait for metadata to load
            return player.NaturalDuration.HasTimeSpan ? player.NaturalDuration.TimeSpan.TotalMilliseconds : 0;
        } catch (Exception ex) {
            return 0;
        }
    }
}
