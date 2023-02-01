using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class LeaderboardMangement : MonoBehaviour
{
    public float childHeight;
    public GameObject scoreChildPrefab;
    public RectTransform contentParent;

    private float currentHeight;

    // Start is called before the first frame update
    async void Start()
    {
        Task<List<PlayerScore>> scoreTask = DatabaseScoreManager.Instance.GetScores();

        await scoreTask;

        if (scoreTask.IsFaulted)
        {
            Debug.Log("Firebase error");
        }
        else if (scoreTask.IsCompleted)
        {
            List<PlayerScore> pScores = scoreTask.Result;
            pScores.Sort((a, b) => b.score.CompareTo(a.score));
            foreach (PlayerScore score in pScores)
            {
                AddToLeaderboard(score);
            }
        }
    }

    private void AddToLeaderboard(PlayerScore score) 
    {
        GameObject scoreChild = Instantiate(scoreChildPrefab, contentParent);
        var rt = scoreChild.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rt.rect.width);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, currentHeight, rt.rect.height);
        currentHeight += childHeight;

        // Name
        scoreChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = score.name;
        // Score
        scoreChild.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = score.score.ToString();
    }
}
