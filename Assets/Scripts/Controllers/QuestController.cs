using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    public RawImage mapImage;

    private PlayerProcesses playerProcesses;
    private CampusProcesses campusProcesses;

    // Start is called before the first frame update
    async void Start()
    {
        // Get context of code
        string code = PlayerPrefs.GetString("lobbyCode", "");

        DBService dbContext = new DBService();
        FirebaseService fbContext = new FirebaseService();

        playerProcesses = new PlayerProcesses(dbContext, code);
        campusProcesses = new CampusProcesses(fbContext, dbContext, code);

        loadImage(mapImage, await campusProcesses.getMap());
    }

    private void loadImage(RawImage dest, byte[] img)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(img);
        dest.texture = texture;
    }

}
