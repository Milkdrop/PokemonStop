using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public string token;
    [HideInInspector]
    public string username;
    [HideInInspector]
    public string email;

    public UIManager uiMan;
    private HTTPRequester httpReq;

    void Start() {
        httpReq = transform.GetComponent<HTTPRequester>();
        token = PlayerPrefs.GetString ("token", "");
    }

    public void InitDaemon () {
        Input.location.Start();
        AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
        AndroidJavaObject plugin = new AndroidJavaObject ("com.hashtagh.pokeservice.PingerClass");
        
        plugin.Call ("initialize", activity, token);
        StartCoroutine (httpReq.GET ("/my-user", SetPlayerData));
    }

    public void SetPlayerData (int responseCode, string data) {
        Debug.Log (responseCode);
        Debug.Log (data);

        bool valid = false;

        if (responseCode == 200) {
            Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            if (response["status"] == "success") {
                username = response["name"];
                email = response["email"];
                uiMan.PushError ("");
                valid = true;
            } else if (response["status"] == "error") {
                uiMan.PushError (response["message"]);
            } else {
                uiMan.PushError ("Unknown Error");
            }
        } else {
            uiMan.PushError ("Unknown Error");
        }
        
        if (valid) {
            uiMan.SpawnMenuScreen ();
        } else {
            Logout ();
        }
    }

    public void Register (string name, string email, string password, string latitude, string longitude) {
        Debug.Log (latitude + " " + longitude);
        Dictionary<string, string> data = new Dictionary<string,string>{{"name", name}, {"email", email}, {"password", password}, {"safe_lat", latitude}, {"safe_long", longitude}};
        StartCoroutine (httpReq.POST ("/register", data, GetToken));
    }

    public void Login (string email, string password) {
        Dictionary<string, string> data = new Dictionary<string,string>{{"email", email}, {"password", password}};
        StartCoroutine (httpReq.POST ("/login", data, GetToken));
    }

    public void Logout () {
        PlayerPrefs.SetString ("token", "");
        token = "";
        uiMan.SpawnRegisterScreen ();
    }

    public void GetToken (int responseCode, string data) {
        Debug.Log (responseCode);
        Debug.Log (data);

        Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

        if (response == null) {
            uiMan.PushError ("API Server offline :(");
        } else if (responseCode == 200) {
            if (response["status"] == "success") {
                token = response["api_key"];
                PlayerPrefs.SetString ("token", token);
                uiMan.PushError ("");
                InitDaemon ();
            } else if (response["status"] == "error") {
                uiMan.PushError (response["message"]);
            } else {
                uiMan.PushError ("Unknown Error");
            }
        } else {
            uiMan.PushError ("Unknown Error");
        }
    }
}