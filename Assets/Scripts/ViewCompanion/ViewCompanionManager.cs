using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ViewCompanionManager : MonoBehaviour
{
    [Header("Phrases")]
    public List<string> phrases;

    [Header("Animations")]
    public string startAnimation;
    public List<string> animations;

    [Header("References")]
    public TextMeshProUGUI msgUI;
    public GameObject bearPrefab;
    public GameObject checkCollisionPrefab;
    public ARRaycastManager raycastManager;
    public Camera arCamera;
    public ARPlaneManager arPlaneManager;

    private bool isPlayerReady;
    private GameObject bear;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private int phraseIndex;
    private int animationIndex;

    // Start is called before the first frame update
    void Start()
    {
        arPlaneManager.enabled = true;
        StartCoroutine(TypeOut.Instance.Type(msgUI, "Click on a plane to initialize", false));
    }

    // Update is called once per frame
    void Update()
    {
        manageRayCast();
    }

    private void manageRayCast() 
    {
        Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            // Hit plane
            if (raycastManager.Raycast(ray, hits, TrackableType.Planes))
            {
                Pose hitPose = hits[0].pose;

                if (!isPlayerReady)
                    Setup(hitPose.position);
            }

            // Hit bear
            GameObject obj = Instantiate(checkCollisionPrefab, arCamera.transform.position, Quaternion.identity);
            Destroy(obj, 5f);
            CheckCollision checkCollider = obj.GetComponent<CheckCollision>();
            checkCollider.dir = arCamera.transform.forward;
            checkCollider.speed = 100f;
            checkCollider.targetTag = "Bear";
            checkCollider.callback = HitBear;
        }
    }

    public void HitBear(Collision collision) 
    {
        if (TypeOut.Instance.isTalking || AnimationPlayer.Instance.isAnimationPlaying) return;

        // Say phrase
        StartCoroutine(TypeOut.Instance.Type(msgUI, phrases[phraseIndex], false));
        phraseIndex = phraseIndex+1 >= phrases.Count ? 0 : phraseIndex+1;

        // Do dance
        StartCoroutine(AnimationPlayer.Instance.PlayAnimation(bear.GetComponent<Animator>(), animations[animationIndex]));
        animationIndex = animationIndex + 1 >= animations.Count ? 0 : animationIndex + 1;
    }

    // Set player as ready when plane selected
    private void Setup(Vector3 refPosition) 
    {
        foreach (var plane in arPlaneManager.trackables)
            plane.gameObject.SetActive(false);
        arPlaneManager.enabled = false;
        msgUI.text = "";
        bear = Instantiate(bearPrefab, refPosition, Quaternion.identity);
        isPlayerReady = true;

        StartCoroutine(AnimationPlayer.Instance.PlayAnimation(bear.GetComponent<Animator>(), startAnimation));
        StartCoroutine(TypeOut.Instance.Type(msgUI, "Hello Student!", false));
    }

}
