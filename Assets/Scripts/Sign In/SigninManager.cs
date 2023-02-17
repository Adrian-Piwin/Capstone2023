using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;

public class SigninManager : MonoBehaviour
{
    [Header("References")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    public TMP_InputField usernameInput;
    public GameObject usernamePrompt;

    public TextMeshProUGUI msg;
    public SceneManagment sceneManager;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    private void Start()
    {
        InitializeFirebase();
        
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            //sceneManager.ChangeScene("SampleScene");
        }
    }

    // Handle initialization of the necessary firebase modules:
    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    // Handle removing subscription and reference to the Auth instance.
    // Automatically called by a Monobehaviour after Destroy is called on it.
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    public void SignInClicked() 
    {
        if (emailInput.text.Trim() == "" || passwordInput.text.Trim() == "")
        {
            msg.text = "One of your fields are missing!";
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                msg.text = "There was a problem with signing in, try again";
                return;
            }

            // Firebase user signed in
            Debug.Log("User signed in successfully");

            sceneManager.ChangeScene("SampleScene");
        });
    }
    public void SignUpClicked() 
    {
        if (emailInput.text.Trim() == "" || passwordInput.text.Trim() == "") 
        {
            msg.text = "One of your fields are missing!";
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                SignUpFailed();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                SignUpFailed();
                return;
            }

            // Firebase user has been created.
            Debug.Log("User signed up successfully");
        });

        usernamePrompt.SetActive(true);
    }

    public void UsernameDoneClicked() 
    {
        if (usernameInput.text.Trim() == "")
        {
            msg.text = "Username is missing!";
            return;
        }

        // Save username to profile
        string username = usernameInput.text.Trim();
        Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
        {
            DisplayName = username
        };
        user.UpdateUserProfileAsync(profile).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdateUserProfileAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                msg.text = "There was a problem with your username, try to sign up again";
                return;
            }

            Debug.Log("User profile updated successfully.");
        });

        sceneManager.ChangeScene("SampleScene");
    }

    private void SignUpFailed() 
    {
        usernamePrompt.SetActive(false);
        msg.text = "There was a problem with signing up, try again";
    }
}
