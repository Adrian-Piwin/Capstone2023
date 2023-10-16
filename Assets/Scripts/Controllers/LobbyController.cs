using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    public float childHeight;
    public GameObject playerChildPrefab;
    public RectTransform contentParent;

    private bool timerEnabled = true;
    private float timePassed = 0f;
    private float currentHeight;
    private List<GameObject> playerChildren = new List<GameObject>();

    private DBService dbContext;
    private PlayerProcesses playerProcesses;
    private CampusProcesses campusProcesses;

    // Start is called before the first frame update
    void Start()
    {
        // Get context of code
        string code = PlayerPrefs.GetString("lobbyCode", "");

        // Get players from database
        dbContext = new DBService();

        playerProcesses = new PlayerProcesses(dbContext, code);
        campusProcesses = new CampusProcesses(dbContext, code);
        loadPlayers();

        MsgUtility.instance.DisplayMsg("Please wait for the game to start.", MsgType.Info);
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
        if (campusProcesses.isGameStarted()) { 
            StartCoroutine(InitiateQuest());
            return;
        }

        currentHeight = 0;
        foreach (GameObject child in playerChildren)
        {
            Destroy(child);
        }
        playerChildren.Clear();

        List<Player> players = playerProcesses.getPlayers();
        foreach (Player player in players)
        {
            AddToLobby(player);
        }
    }

    private void AddToLobby(Player player)
    {
        GameObject playerChild = Instantiate(playerChildPrefab, contentParent);
        var rt = playerChild.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rt.rect.width);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, currentHeight, rt.rect.height);
        currentHeight += childHeight;

        // Name
        playerChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.username;
        // Score
        playerChild.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.id.ToString();

        playerChildren.Add(playerChild);
    }

    IEnumerator InitiateQuest()
    {
        timerEnabled = false;
        MsgUtility.instance.DisplayMsg("Get ready, your quest begins now!", MsgType.Info);
        yield return new WaitForSeconds(3);
        dbContext.dispose();
        SceneUtility.instance.ChangeScene("Quest");
    }
}
