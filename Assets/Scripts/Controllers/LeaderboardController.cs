using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LeaderboardController : MonoBehaviour 
{
    public TextMeshProUGUI playerPlaceUI;

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
        playerID = PlayerPrefs.GetInt("playerID", -1);

        // Get players from database
        dbContext = new DBService();

        playerProcesses = new PlayerProcesses(dbContext, code);
        loadPlayers();
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

        players = playerProcesses.getPlayers();
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
        playerChild.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"#{place}";

        if (player.id == playerID)
        {
            playerChild.transform.GetChild(2).gameObject.SetActive(true);
            playerPlaceUI.text = $"Your Place: {place}";
        }

        playerChildren.Add(playerChild);
    }

    private int GetPlayersPlace(Player player)
    {
        List<Player> orderedPlayers = players.OrderByDescending(p => p.status).ToList();
        return orderedPlayers.FindIndex(obj => obj.id == player.id) + 1;
    }
}