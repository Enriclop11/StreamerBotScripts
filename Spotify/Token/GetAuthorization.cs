using System;
using System.Net;
using System.Net.Http;

public class CPHInline {
    public bool Execute() {

        string spotifySecret = CPH.GetGlobalVar<string>("spotifyKey");
        string spotifyId = CPH.GetGlobalVar<string>("spotifyId");

        HttpClient client = new HttpClient();

        //make the get request in the explorer 

        string responseType = "code";
        string scope = "user-read-private user-read-email user-read-playback-state user-modify-playback-state user-read-currently-playing";
        string redirectUri = "http://localhost:8888/callback";

        //System.Diagnostics.Process.Start(target);   
        System.Diagnostics.Process.Start("https://accounts.spotify.com/authorize?response_type=" + responseType + "&client_id=" + spotifyId + "&scope=" + scope + "&redirect_uri=" + redirectUri); 

        // catch the response in the callback 
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8888/");
        listener.Start();
        HttpListenerContext context = listener.GetContext();
        HttpListenerRequest request = context.Request;

        string code = request.QueryString["code"];
        
        // Set in argument
        CPH.SetArgument("spotifyCode", code);
        return true;
    }
}