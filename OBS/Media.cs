using System;
public class CPHInline
{
	public bool Execute()
	{
		string source = "media";

		string mediaPath = CPH.GetGlobalVar<string>("mediaPath");

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

		//get a random video
		Random rnd = new Random();
		string file = files[rnd.Next(files.Length)];

		//show the source
		CPH.ObsSetSourceVisibility(currentScene, source, true);

		//show the video in the current scene CPH.ObsSetMediaSourceFile(scene, source, file);
		CPH.ObsSetMediaSourceFile(currentScene, source, file);

		//get SceneItem raw
		string sceneItem = CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\"" + currentScene + "\",\"sourceName\":\"" + source + "\",\"searchOffset\":null}", 0);

		//transform the sceneItem to a random postition CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":null,\"sceneItemId\":0,\"sceneItemTransform\":null}", 0);
		int positionX = rnd.Next(0, 1620);
		int positionY = rnd.Next(0, 880);
		CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\"" + currentScene + "\",\"sceneItemId\":" + sceneItem + ",\"sceneItemTransform\":{\"positionX\":" + positionX + ",\"positionY\":" + positionY + "}}", 0);

		//play the video
		CPH.ObsMediaPlay(currentScene, source);

		//get the duration of the video and wait for it to finish
		/*
		using (ShellObject shellObject = ShellObject.FromParsingName(file))
		{
			IPropertyStore propertyStore = shellObject.Properties;
			IShellProperty property = propertyStore.GetProperty(SystemProperties.System.Media.Duration);
			ulong duration = (ulong)property.ValueAsObject;
			System.Threading.Thread.Sleep((int)duration);

			//hide the video
			CPH.ObsSetSourceVisibility(currentScene, "media", false);
		}
		*/

		return true;
	}
}
