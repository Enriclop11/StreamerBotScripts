using System;

public class CPHInline
{
	public bool Execute()
	{
		//CPH.RegisterCustomTrigger(triggerName, eventName, categories);
		CPH.RegisterCustomTrigger("userTalk", "userTalk", new string[] { "user" });
		//Trigger the userTalk event when the user is pakuu27 CPH.TriggerCodeEvent("userTalk");
		string userName = CPH.TryGetArg("userName", out string user) ? user : "User";

		Console.WriteLine($"{userName} is talking to you!");
		if(userName == "pakuu27"){
			CPH.TriggerCodeEvent("userTalk");
		}

		if (userName == "enriclop11")
		{
			CPH.TriggerCodeEvent("userTalk");
		}
		return true;
	}
}
