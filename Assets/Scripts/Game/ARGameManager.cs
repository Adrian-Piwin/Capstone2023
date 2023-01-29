using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ARGameManager : MonoBehaviour
{
    [Header("Settings")]
    public string song;
    public float songDuration;

    public float bpm;
    public float beatSpawnDistanceMax;
    public float beatSpawnDistanceMin;
    public float beatLifetime;
    public float beatSpeed;

    [Header("Animations")]
    public List<string> animations;
    public List<float> animationDuration;
    public string failAnimation;
    public float failAnimationDuration;
    public string winAnimation;
    public float winAnimationDuration;

    [Header("References")]
    public TextMeshProUGUI scoreTxt;
    public GameObject bearPrefab;
    public GameObject beatPrefab;
    public ARRaycastManager raycastManager;
    public Camera arCamera;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isGameStarted;
    private GameObject bear;
    private int score;
    private bool isAltAnimationPlaying;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        manageRayCast();
    }

    private void manageRayCast()
    {
        Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            // Hit plane
            if (raycastManager.Raycast(ray, hits, TrackableType.Planes))
            {
                Pose hitPose = hits[0].pose;
                if (!isGameStarted)
                    SetupGame(hitPose.position);
            }
        }
    }

    // Set player as ready when plane selected
    private void SetupGame(Vector3 refPosition)
    {
        UpdateScore();
        bear = Instantiate(bearPrefab, refPosition, Quaternion.identity);
        isGameStarted = true;
        StartCoroutine(CycleDanceAnimations(bear.GetComponent<Animator>()));
        StartCoroutine(SpawnBeats());
    }

    private void HitBeat(GameObject beat) 
    {
        score++;
        UpdateScore();
        StartCoroutine(PlayAltAnimation(bear.GetComponent<Animator>(), winAnimation, winAnimationDuration));
    }

    IEnumerator SpawnBeats()
    {
        List<Vector3> directions = new List<Vector3> { bear.transform.forward, bear.transform.forward * -1, bear.transform.right, bear.transform.right * -1 };

        while (isGameStarted)
        {
            yield return new WaitForSeconds(60 / bpm);
            GameObject beat = Instantiate(beatPrefab, directions[Random.Range(0, directions.Count)] * Random.Range(beatSpawnDistanceMin, beatSpawnDistanceMax), Quaternion.identity);
            beat.GetComponent<BeatController>().speed = beatSpeed;
            DestroyBeat(beat);
        }
    }

    // Queue to destroy, lose point if destryed by time
    IEnumerator DestroyBeat(GameObject beat) 
    {
        yield return new WaitForSeconds(beatLifetime);
        Destroy(beat);
        score--;
        UpdateScore();
        StartCoroutine(PlayAltAnimation(bear.GetComponent<Animator>(), failAnimation, failAnimationDuration));
    }

    // Cycle dance animations
    IEnumerator CycleDanceAnimations(Animator animator)
    {
        int i = 0;
        foreach (string animation in animations)
        {
            // Stop new animation from playing if animation was replaced
            while (isAltAnimationPlaying) yield return null;

            // Play dance animation
            animator.Play(animation);
            yield return new WaitForSeconds(animationDuration[i]);
            i++;
        }

        StartCoroutine(CycleDanceAnimations(animator));
    }

    IEnumerator PlayAltAnimation(Animator animator, string animation, float animationDuration)
    {
        isAltAnimationPlaying = true;
        animator.Play(animation);
        yield return new WaitForSeconds(animationDuration);
        isAltAnimationPlaying = false;
    }

    private void UpdateScore() 
    {
        scoreTxt.text = "Score: " + score.ToString();
    }
}
