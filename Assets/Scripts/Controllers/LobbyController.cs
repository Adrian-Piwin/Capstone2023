using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    public float childHeight;
    public GameObject scoreChildPrefab;
    public RectTransform contentParent;

    private float currentHeight;

    // Start is called before the first frame update
    void Start()
    {
        // Get context of code
        string code = PlayerPrefs.GetString("lobbyCode", "");

        // Get players from database
        DBService context = new DBService();

        PlayerProcesses playerProcesses = new PlayerProcesses(context);
        List<Player> players = playerProcesses.getPlayers(code);
        foreach (Player player in players)
        {
            AddToLobby(player);
        }

        MsgUtility.instance.DisplayMsg("Please wait for the game to start.", MsgType.Info);
    }

    private void AddToLobby(Player player)
    {
        GameObject scoreChild = Instantiate(scoreChildPrefab, contentParent);
        var rt = scoreChild.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rt.rect.width);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, currentHeight, rt.rect.height);
        currentHeight += childHeight;

        // Name
        scoreChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.username;
        // Score
        scoreChild.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.id.ToString();
    }
}
