using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUtility : MonoBehaviour
{
    public static SceneUtility instance;
    private Animator canvasAnimator;
    private string nextScene;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        canvasAnimator = GetComponent<Animator>();
        if (canvasAnimator != null )
            canvasAnimator.Play("TransitionEnter");
    }

    public void ChangeScene(string sceneName) 
    {
        nextScene = sceneName;
        if (canvasAnimator != null)
            canvasAnimator.Play("TransitionExit");
        else
            SceneManager.LoadScene(nextScene);
    }

    public void RestartScene()
    {
        nextScene = SceneManager.GetActiveScene().name;
        if (canvasAnimator != null)
            canvasAnimator.Play("TransitionExit");
        else
            SceneManager.LoadScene(nextScene);
    }

    public void TransitionAnimFinished()
    {
        SceneManager.LoadScene(nextScene);
    }
}
