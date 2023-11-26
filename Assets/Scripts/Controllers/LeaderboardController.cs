using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour 
{
    public TextMeshProUGUI playerPlaceUI;
    public TextMeshProUGUI playerProgressUI;
    public Image progressBar;

    public float childHeight;
    public GameObject playerChildPrefab;
    public RectTransform contentParent;

    private int playerID;
    private bool timerEnabled = true;
    private float timePassed = 0f;
    private float currentHeight;
    private List<GameObject> playerChildren = new List<GameObject>();
    private List<Player> players;

    private float startingProgressBarScale;

    private DBService dbContext;
    private PlayerProcesses playerProcesses;
    private POIProcesses poiProcesses;

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

        // Handle progress bar
        startingProgressBarScale = progressBar.transform.localScale.x;
        poiProcesses = new POIProcesses(dbContext, lobbyID);
        UpdateProgressBar(players.FirstOrDefault(o => o.id == playerID));
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
        players = playerProcesses.getPlayers().OrderByDescending(p => p.status).ToList();
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
            playerPlaceUI.text = $" {GetOrdinalSuffix(place)}";
        }

        playerChildren.Add(playerChild);
    }

    private int GetPlayersPlace(Player player)
    {
        return players.FindIndex(obj => obj.id == player.id) + 1;
    }

    private void UpdateProgressBar(Player player)
    {
        int poiTotal = poiProcesses.getAllPOI().Count;
        float progress = player.status == 0 ? 0 : (float)(player.status - 1) / poiTotal;
        playerProgressUI.text = $"{poiTotal - player.status + 1} more to go!";
        progressBar.transform.localScale = new Vector3(startingProgressBarScale * progress, progressBar.transform.localScale.y, progressBar.transform.localScale.z);
    }

    private string GetOrdinalSuffix(int number)
    {
        if (number <= 0)
        {
            return number.ToString();
        }

        if (number % 100 >= 11 && number % 100 <= 13)
        {
            return number + "th";
        }

        switch (number % 10)
        {
            case 1: return number + "st";
            case 2: return number + "nd";
            case 3: return number + "rd";
            default: return number + "th";
        }
    }
}