using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public TMP_InputField codeInput;
    public TMP_InputField nameInput;

    private DBService dbContext;

    private void Start()
    {
        //string code = PlayerPrefs.GetString("lobbyCode", "");
        //if (code != "")
        //    SceneManager.LoadScene("Lobby");

        dbContext = new DBService();
    }

    public void onJoinPressed()
    {
        string code = codeInput.text.Trim();
        string name = nameInput.text.Trim();

        if (code.Length < 5)
        {
            MsgUtility.instance.DisplayMsg("Please enter a valid code", MsgType.Error);
            return;
        }

        if (name.Length < 3)
        {
            MsgUtility.instance.DisplayMsg("Your name must be at least 3 characters long.", MsgType.Error);
            return;
        }

        // Check if lobby is valid
        CampusProcesses campusProccesses = new CampusProcesses(dbContext, code);
        if (!campusProccesses.isLobbyValid())
        {
            MsgUtility.instance.DisplayMsg("Cannot find a lobby with that code.", MsgType.Error);
            return;
        }

        // Create player
        PlayerProcesses playerProcesses = new PlayerProcesses(dbContext, code);
        Player player = playerProcesses.createPlayer(name);

        if (player == null)
        {
            MsgUtility.instance.DisplayMsg("Failed to create player, try again later.", MsgType.Error);
            return;
        }

        // Save lobby dbContext to player prefs
        PlayerPrefs.SetString("lobbyCode", code);
        PlayerPrefs.SetInt("playerID", player.id);

        // Go to next scene
        dbContext.dispose();
        SceneUtility.instance.ChangeScene("Lobby");
    }
}
