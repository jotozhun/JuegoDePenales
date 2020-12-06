using Firebase.Auth;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FacebookManager : MonoBehaviour
{
    public Button btnLogin, btnLogout, btnName;
    Firebase.Auth.FirebaseAuth auth;

    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void FacebookLogin()
    {
        var permissions = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(permissions, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            SignInFirebase(aToken);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions) Debug.Log(perm);
            SceneManager.LoadScene("Radio/scenes/RadioScene");
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }
    public void FacebookLogout()
    {
        FB.LogOut();
    }

    public void SignInFirebase(AccessToken accessToken){
      Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken.TokenString);
      auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
          if (task.IsCanceled) {
            Debug.LogError("SignInWithCredentialAsync was canceled.");
            return;
          }
          if (task.IsFaulted) {
            Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
            return;
          }

          Firebase.Auth.FirebaseUser newUser = task.Result;
          Debug.LogFormat("User signed in successfully: {0} ({1})",
              newUser.DisplayName, newUser.UserId);
      });
    }

    public void GetName()
    {
        FB.API("me?fields=name", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        {
            if (result.ResultDictionary != null)
            {
                foreach (string key in result.ResultDictionary.Keys)
                {
                    Debug.Log(key + " : " + result.ResultDictionary[key].ToString());
                    if (key == "name")
                        btnName.GetComponentInChildren<Text>().text = result.ResultDictionary[key].ToString();
                }
            }
        });
    }

    void Start()
    {
        btnLogin.onClick.AddListener(() => FacebookLogin());
        btnLogout.onClick.AddListener(() => FacebookLogout());
        btnName.onClick.AddListener(() => GetName());
    }
}