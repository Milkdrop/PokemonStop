using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public string token;
    private HTTPRequester httpReq;

    void Start() {
        httpReq = transform.GetComponent<HTTPRequester>();
        Input.location.Start();
        token = PlayerPrefs.GetString ("token", "");

        AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
        AndroidJavaObject plugin = new AndroidJavaObject ("com.hashtagh.pokeservice.PingerClass");
        
        plugin.Call ("initialize", activity, token);
    }

    public void Register (string email, string password, string latitude, string longitude) {
        string returnValue = httpReq.GET ("");
        Debug.Log ("Register return value: " + returnValue);
    }
}
