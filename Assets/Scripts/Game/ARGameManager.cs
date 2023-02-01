using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using System.Threading.Tasks;

public class ARGameManager : MonoBehaviour
{
    [Header("Settings")]
    public float gameDuration;
    public float beatSpawnInterval;
    public float beatSpawnDistanceMax;
    public float beatSpawnDistanceMin;
    public float beatSpawnYOffset;
    public float beatLifetime;
    public float beatSpeed;

    [Header("Animations")]
    public List<string> animations;
    public string winAnimation;

    [Header("References")]
    public AudioSource audioSrc;
    public TextMeshProUGUI scoreTxt;
    public GameObject playAgainBtn;
    public GameObject bearPrefab;
    public GameObject beatPrefab;
    public GameObject beatDestroyPrefab;
    public GameObject beatDestroyFailPrefab;
    public GameObject checkCollisionPrefab;
    public ARRaycastManager raycastManager;
    public Camera arCamera;
    public ARPlaneManager arPlaneManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isGameStarted;
    private bool isGameOver;
    private GameObject bear;
    private int score;
    private float gameTimer;
    List<GameObject> beats = new List<GameObject>();
    private IEnumerator currentAnimationPlayer;

    // Start is called before the first frame update
    void Start()
    {
        arPlaneManager.enabled = true;
        StartCoroutine(TypeOut.Instance.Type(scoreTxt, "Click on a plane to initialize", false));
        playAgainBtn.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;
        manageRayCast();
        EndGame();
    }

    private void manageRayCast()
    {
        Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Hit plane
            if (raycastManager.Raycast(ray, hits, TrackableType.Planes))
            {
                Pose hitPose = hits[0].pose;
                if (!isGameStarted && !isGameOver)
                    SetupGame(hitPose.position);
            }

            if (!isGameStarted) return;

            // Hit note
            GameObject obj = Instantiate(checkCollisionPrefab, arCamera.transform.position, Quaternion.identity);
            Destroy(obj, 5f);
            CheckCollision checkCollider = obj.GetComponent<CheckCollision>();
            checkCollider.dir = arCamera.transform.forward;
            checkCollider.speed = 100f;
            checkCollider.targetTag = "Beat";
            checkCollider.callback = HitBeat;
        }
    }

    // Set player as ready when plane selected
    private void SetupGame(Vector3 refPosition)
    {
        bear = Instantiate(bearPrefab, refPosition, Quaternion.identity);
        isGameStarted = true;
        audioSrc.time = 0;
        audioSrc.Play();
        gameTimer = Time.time;
        foreach (var plane in arPlaneManager.trackables)
            plane.gameObject.SetActive(false);
        arPlaneManager.enabled = false;
        StartCoroutine(TypeOut.Instance.Type(scoreTxt, "Score: 0", false));
        StartCoroutine(CycleDanceAnimations());
        StartCoroutine(SpawnBeats());
    }

    private void EndGame() 
    {
        if (Time.time - gameTimer > gameDuration && isGameStarted)
        {
            audioSrc.Stop();
            isGameStarted = false;
            isGameOver = true;
            ClearBeats();
            playAgainBtn.SetActive(true);

            StopCoroutine(currentAnimationPlayer);
            currentAnimationPlayer = AnimationPlayer.Instance.PlayAnimation(bear.GetComponent<Animator>(), winAnimation, true);
            StartCoroutine(currentAnimationPlayer);

            StartCoroutine(TypeOut.Instance.Type(scoreTxt, "Final Score: " + score, false));

            // Update database                                          REPLACE WITH ID HERE
            Task<PlayerScore> scoreTask = DatabaseScoreManager.Instance.GetScore("123");

            scoreTask.ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.Log("Firebase error");
                }
                else if (task.IsCompleted)
                {
                    PlayerScore pScore = task.Result;
                    if (pScore == null || pScore.score < score) // REPLACE ACC HERE
                        DatabaseScoreManager.Instance.SaveScore("123", "Adrian", score);
                }
            });
        }
    }

    private void HitBeat(Collision collision) 
    {
        score++;
        UpdateScore();
        Destroy(Instantiate(beatDestroyPrefab, collision.transform.position, Quaternion.identity), 1f);
        Destroy(collision.gameObject);
    }

    IEnumerator SpawnBeats()
    {
        List<Vector3> directions = new List<Vector3> { Vector3.right, Vector3.right * -1, Vector3.forward, Vector3.forward * -1 };
        while (isGameStarted)
        {
            yield return new WaitForSeconds(beatSpawnInterval);
            Vector3 spawnPos = bear.transform.position + (directions[Random.Range(0, directions.Count)] * Random.Range(beatSpawnDistanceMin, beatSpawnDistanceMax));
            spawnPos = new Vector3(spawnPos.x, spawnPos.y + beatSpawnYOffset, spawnPos.z);
            GameObject beat = Instantiate(beatPrefab, spawnPos, Quaternion.identity);
            beat.GetComponent<BeatController>().speed = beatSpeed;
            StartCoroutine(DestroyBeat(beat));
            beats.Add(beat);
        }
    }

    // Destroy after x time
    IEnumerator DestroyBeat(GameObject beat) 
    {
        yield return new WaitForSeconds(beatLifetime);
        if (beat == null || !isGameStarted) yield break;

        Destroy(Instantiate(beatDestroyFailPrefab, beat.transform.position, Quaternion.identity), 1f);
        Destroy(beat);
    }

    // Cycle dance animations
    IEnumerator CycleDanceAnimations()
    {
        if (isGameOver) yield break;

        int i = 0;
        foreach (string animation in animations)
        {
            currentAnimationPlayer = AnimationPlayer.Instance.PlayAnimation(bear.GetComponent<Animator>(), animation);
            yield return StartCoroutine(currentAnimationPlayer);
            i++;
        }

        if (isGameOver) yield break;
        StartCoroutine(CycleDanceAnimations());
    }

    private void ClearBeats() 
    {
        foreach (var beat in beats)
            if (beat != null)
                Destroy(beat);
    }

    private void UpdateScore() 
    {
        scoreTxt.text = "Score: " + score.ToString();
    }
}
