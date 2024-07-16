using System;

public class CPHInline {
    public bool Execute() {
        //Get arguments from song
        string songName = CPH.TryGetArg<string>("songName", out string name) ? name : "Song";
        string artistName = CPH.TryGetArg<string>("artistName", out string artist) ? artist : "Artist";

        if (songName == "Song" || artistName == "Artist") {
            CPH.SendMessage("No song is currently playing");
            return true;
        }

        CPH.SendMessage("Song: " + songName + " Artist: " + artistName);
        return true;
    }
}