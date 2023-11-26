using System;

public class Player
{
    public int id { get; set; }
    public string lobbyID { get; set; }
    public string username { get; set; }
    public int status { get; set; }
    public DateTime timer { get; set; }
    public DateTime timerEnd { get; set; }

    public TimeSpan timeToComplete 
    {
        get 
        {
            return timerEnd - timer;
        }
    }

    public bool didCompleteQuest 
    {
        get 
        {
            return timerEnd != DateTime.MinValue;
        }
    }
}
