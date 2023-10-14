using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public TMP_InputField codeInput;
    public TMP_InputField nameInput;

    private void Start()
    {
        string code = PlayerPrefs.GetString("lobbyCode", "");
        if (code != "")
            SceneUtility.instance.ChangeScene("Lobby");
    }

    public void onJoinPressed()
    {
        string code = codeInput.text.Trim();
        string name = nameInput.text.Trim();
        Debug.Log(code.Length);
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

        DBService context = new DBService();

        // Check if lobby is valid
        CampusProcesses campusProccesses = new CampusProcesses(context);
        if (!campusProccesses.isLobbyValid(code))
        {
            MsgUtility.instance.DisplayMsg("Cannot find a lobby with that code.", MsgType.Error);
            return;
        }

        // Create player
        PlayerProcesses playerProcesses = new PlayerProcesses(context);
        if (!playerProcesses.createPlayer(name, code))
        {
            MsgUtility.instance.DisplayMsg("Failed to create player, try again later.", MsgType.Error);
            return;
        }

        context.dispose();

        // Save lobby context to player prefs
        PlayerPrefs.SetString("lobbyCode", code);

        // Go to next scene
        SceneUtility.instance.ChangeScene("Lobby");
    }
}
