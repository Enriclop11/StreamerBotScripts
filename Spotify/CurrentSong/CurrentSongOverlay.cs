using System;
using System.Collections.Generic;

public class CPHInline {
    public bool Execute() {
        //Get arguments from song
        string songName = CPH.TryGetArg<string>("songName", out string name) ? name : "Song";
        string artistName = CPH.TryGetArg<string>("artistName", out string artist) ? artist : "Artist";
        string albumName = CPH.TryGetArg<string>("albumName", out string album) ? album : "Album";
        string albumImage = CPH.TryGetArg<string>("albumImage", out string image) ? image : "Image";

        var variables = new Dictionary<string, string> {
            { "{{songName}}", songName },
            { "{{artistName}}", artistName },
            { "{{albumName}}", albumName },
            { "{{albumImage}}", albumImage }
        };

        //Overlay html with the song information
        //Get the filesPath global var
        string filesPath = CPH.GetGlobalVar<string>("filesPath");

        //In the directory get the directory spotify, if it does not exist create it
        string spotifyPath = filesPath + "\\spotify";
        if (!System.IO.Directory.Exists(spotifyPath))
        {
            System.IO.Directory.CreateDirectory(spotifyPath);
        }

        //Path SongOverlay
        string songOverlayPath = spotifyPath + "\\currentSong.html";

        string html;

        if (songName == "Song")
        {
            html = "<html><head><meta http-equiv='refresh' content='10'></head></body></html>";
            System.IO.File.WriteAllText(songOverlayPath, html);
            return true;
        }

        //Get the file currentSongCustom.html
        string currentSongPathCustom = spotifyPath + "\\currentSongCustom.html";
        if (!System.IO.File.Exists(currentSongPathCustom))
        {
            //If the file does not exist, create it with the default html
            html = @"
            <html>
            <head>
                <meta http-equiv='refresh' content='10'>
                <style>
                    body {
                        background-color: transparent;
                        background-size: 30px 30px;
                        background-position: 0 0, 15px 15px;
                    }
                    .container {
                        display: flex;
                        align-items: center;
                        background-color: white;
                        padding: 10px;
                        border-radius: 10px;
                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                        width: 60%;
                        margin: auto; 
                    }
                    img {
                        width: 100px;
                        height: 100px;
                        border-radius: 50%;
                        margin-right: 15px;
                    }
                    .text-container {
                        display: flex;
                        flex-direction: column;
                    }
                    h1, h2, h3 {
                        margin: 0;
                        padding: 2px;
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <img src='{{albumImage}}'>
                    <div class='text-container'>
                        <h1>{{songName}}</h1>
                        <h2>{{artistName}}</h2>
                        <h3>{{albumName}}</h3>
                    </div>
                </div>
            </body>
            </html>
            ";
        } else {
            html = System.IO.File.ReadAllText(currentSongPathCustom);
        }

        //Replace the variables in the html
        foreach (var variable in variables)
        {
            html = html.Replace(variable.Key, variable.Value);
        }

        //Write the html in the file
        System.IO.File.WriteAllText(songOverlayPath, html);

        return true;
    }
}