using System;

public class CPHInline
{
	public bool Execute()
	{
		//CPH.RegisterCustomTrigger(triggerName, eventName, categories);
		CPH.RegisterCustomTrigger("userTalk", "userTalk", new string[] { "user" });
		//Trigger the userTalk event when the user is pakuu27 CPH.TriggerCodeEvent("userTalk");
		string userName = CPH.TryGetArg("userName", out string user) ? user : "User";

		//get list of users that can talk, global var string with the users separated by commas
		string users = CPH.GetGlobalVar<string>("usersTalkTrigger");
		string[] usersArray = users.Split(',');
		if (Array.Exists(usersArray, element => element == userName))
		{
			CPH.TriggerCodeEvent("userTalk");
		}
		return true;
	}
}
