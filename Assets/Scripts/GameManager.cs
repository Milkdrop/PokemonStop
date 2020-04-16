using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public UIManager uiMan;
    public CreatureUIManager creatureUIMan;
    private HTTPRequester httpReq;

    private AndroidJavaObject plugin;

    [HideInInspector]
    public string token;
    [HideInInspector]
    public string username;
    [HideInInspector]
    public string email;

    [HideInInspector]
    public string distanceFromHome;

    [HideInInspector]
    public int creatureLevel;
    [HideInInspector]
    public int creatureXP;
    [HideInInspector]
    public int creatureMaxXP;

    [HideInInspector]
    public int researchPoints;

    [HideInInspector]
    public List<int> Costumes;

    void Start() {
        Input.location.Start (2, 2);
        httpReq = transform.GetComponent<HTTPRequester>();
        token = PlayerPrefs.GetString ("token", "");
    }

    public void InitDaemon () {
        AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
        plugin = new AndroidJavaObject ("com.hashtagh.pokeservice.PingerClass");
        
        plugin.Call ("initialize", activity, token);
        uiMan.SpawnMenuScreen ();
        InvokeRepeating ("UpdateUserData", 0, 60);
        UpdateCostumes ();
    }

    public void UpdateUserData () {
        StartCoroutine (httpReq.GET ("/my-user", SetPlayerData));
    }

    public void SetPlayerData (int responseCode, string data) {
        bool valid = false;

        if (responseCode == 200) {
            Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            if (response["status"] == "success") {
                username = response["name"];
                email = response["email"];
                distanceFromHome = response["distance"];
                creatureLevel = int.Parse (response["level"]);
                creatureXP = int.Parse (response["exp"]);
                creatureMaxXP = int.Parse (response["max_exp"]);
                researchPoints = int.Parse (response["research_points"]);

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
            creatureUIMan.UpdateMenuScreen ();
        } else {
            uiMan.PushError ("Couldn't fetch player data");
        }
    }

    public void BuyItem (int id, int price) {
        if (researchPoints >= price)
            researchPoints -= price;
        StartCoroutine (httpReq.GET ("/shop/buy/" + id, UpdateCostumes));
    }

    public void UpdateCostumes (int responseCode, string data) {
        UpdateCostumes ();
    }

    public void UpdateCostumes () {
        StartCoroutine (httpReq.GET ("/my-costumes", UpdateCostumeData));
    }

    public void UpdateCostumeData (int responseCode, string data) {
        bool valid = false;

        if (responseCode == 200) {
            List<Dictionary<string,string>> response = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(data);
            Costumes = new List<int>();

            for (int i = 0; i < response.Count; i++) {
                Costumes.Add (int.Parse (response[i]["costume_id"]));
            }

            valid = true;
        } else {
            uiMan.PushError ("Unknown Error");
        }
        
        if (valid) {
            creatureUIMan.UpdateCreatureCostumes ();
            UpdateUserData ();
        } else {
            uiMan.PushError ("Couldn't fetch player data");
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
        plugin.Call ("stop");
        CancelInvoke ("UpdateUserData");
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