using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalsLeaderboardController : MonoBehaviour 
{
    public TextMeshProUGUI firstPlaceUI;

    public float childHeight;
    public GameObject playerChildPrefab;
    public RectTransform contentParent;

    private int playerID;
    private bool timerEnabled = true;
    private float timePassed = 0f;
    private float currentHeight;
    private List<GameObject> playerChildren = new List<GameObject>();
    private List<Player> players;

    private DBService dbContext;
    private PlayerProcesses playerProcesses;

    // Start is called before the first frame update
    void Start()
    {
        // Init
        players = new List<Player>();

        // Get context of code
        string code = PlayerPrefs.GetString("lobbyCode", "");
        string lobbyID = PlayerPrefs.GetInt("campusID", -1).ToString();
        playerID = PlayerPrefs.GetInt("playerID", -1);

        // Get players from database
        dbContext = new DBService();

        playerProcesses = new PlayerProcesses(dbContext, code);
        loadPlayers();

        MsgUtility.instance.DisplayMsg("Congrats! You completed the quest!", MsgType.Success);
    }

    void Update()
    {
        if (!timerEnabled)
            return;

        // Update players every 5 seconds
        timePassed += Time.deltaTime;
        if (timePassed > 5f)
        {
            loadPlayers();
            timePassed = 0f;
        }
    }

    private void loadPlayers()
    {
        currentHeight = 0;
        foreach (GameObject child in playerChildren)
        {
            Destroy(child);
        }
        playerChildren.Clear();
        players.Clear();

        // Order them by their place
        players = playerProcesses.getPlayers().OrderByDescending(p => p.status).OrderByDescending(p => p.timeToComplete).ToList();
        foreach (Player player in players)
        {
            AddToLeaderboard(player);
        }
    }

    private void AddToLeaderboard(Player player)
    {
        GameObject playerChild = Instantiate(playerChildPrefab, contentParent);
        var rt = playerChild.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rt.rect.width);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, currentHeight, rt.rect.height);
        currentHeight += childHeight;
        contentParent.GetComponent<RectTransform>().sizeDelta = new Vector2(contentParent.GetComponent<RectTransform>().sizeDelta.x, currentHeight);

        // Get the place of this plater
        int place = GetPlayersPlace(player);

        // Name
        playerChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.username;
        // Score
        playerChild.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"#{place}";

        // Show who got first place
        if (place == 1)
        {
            firstPlaceUI.text = $"{player.username}!!";
        }

        
        
        if (player.didCompleteQuest)
        {
            // show timer on those who completed
            playerChild.transform.GetChild(1).gameObject.SetActive(true);
            playerChild.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.timeToComplete.ToString();

            if (player.id == playerID)
            {
                // highlight owned player
                playerChild.transform.GetChild(3).gameObject.SetActive(true);
            }
            else 
            {
                // show who else completed quest
                playerChild.transform.GetChild(4).gameObject.SetActive(true);
            }
        }

        playerChildren.Add(playerChild);
    }

    private int GetPlayersPlace(Player player)
    {
        return players.FindIndex(obj => obj.id == player.id) + 1;
    }
}