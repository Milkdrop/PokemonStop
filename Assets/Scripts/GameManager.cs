using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public UIManager uiMan;
    private HTTPRequester httpReq;

    [HideInInspector]
    public string token;
    [HideInInspector]
    public string username;
    [HideInInspector]
    public string email;

    [HideInInspector]
    public int creatureLevel;
    [HideInInspector]
    public int creatureXP;
    [HideInInspector]
    public int creatureMaxXP;

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
        uiMan.SpawnMenuScreen ();
        StartCoroutine (httpReq.GET ("/my-user", SetPlayerData));
    }

    public void SetPlayerData (int responseCode, string data) {
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
            CancelInvoke ("RequestCreatureData");
            InvokeRepeating ("RequestCreatureData", 0, 5);
        } else {
            Logout ();
        }
    }

    public void RequestCreatureData () {
        StartCoroutine (httpReq.GET ("/my-entity", SetCreatureData));
    }
    
    public void SetCreatureData (int responseCode, string data) {
        bool valid = false;

        if (responseCode == 200) {
            Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            if (response["status"] == "success") {
                creatureLevel = int.Parse (response["level"]);
                creatureXP = int.Parse (response["exp"]);
                creatureMaxXP = int.Parse (response["max_exp"]);
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
            uiMan.UpdateMenuScreen ();
        } else {
            uiMan.PushError ("Cannot fetch creature data");
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
        CancelInvoke ("RequestCreatureData");
        PlayerPrefs.SetString ("token", "");
        token = "";
        uiMan.SpawnRegisterScreen ();
    }

    public void GetToken (int responseCode, string data) {
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