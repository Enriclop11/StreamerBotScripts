using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq; 

public class CPHInline
{
	public bool Execute()
	{
		//Get the args user and 
		string userName = CPH.TryGetArg("user", out string user) ? user : "User";
		string rawInput = CPH.TryGetArg("rawInput", out string input) ? input : "es";

		//Get traductorKey
		string traductorKey = CPH.GetGlobalVar<string>("traductorKey");


		//Get the lenguage code and the text to translate
        string lenguageCode = CPH.TryGetArg("lenguageCode", out string lenguage) ? lenguage : "ES";
		string textToTranslate = rawInput;

		//Get the traduction POST https://api-free.deepl.com/v2/translate, Authentication with Authorization header and DeepL-Auth-Key authentication scheme
		HttpClient client = new HttpClient();
		client.DefaultRequestHeaders.Add("Authorization", "DeepL-Auth-Key " + traductorKey);

		string data = JsonConvert.SerializeObject(new
		{
			text = new string[] { textToTranslate },
			target_lang = lenguageCode
		});
		CPH.SendMessage("data: " + data);
		HttpResponseMessage response = client.PostAsync("https://api-free.deepl.com/v2/translate", new StringContent(data, System.Text.Encoding.UTF8, "application/json")).Result;

		CPH.SendMessage("response: " + response.ToString());

		//response 
		if (response.IsSuccessStatusCode)
		{
			string responseBody = response.Content.ReadAsStringAsync().Result;
			dynamic json = JsonConvert.DeserializeObject(responseBody);

			string translatedText = json.translations[0].text;
			
			CPH.SendMessage("@" + userName + ": " + translatedText);
		}
		else
		{
			CPH.LogError("The request to the traductor API failed");
			return false;
		}

		return true;
	}
}
