using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class HTTPRequester : MonoBehaviour {
    
    public delegate void callBackFun(int responseCode, string data);

    private string apiEndpoint = "https://webhook.site/5de62bce-d222-456a-9232-5ba6824e1905";

    public IEnumerator GET (string path, callBackFun callBack) {
        UnityWebRequest www = UnityWebRequest.Get (apiEndpoint + path);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }

        callBack ((int) www.responseCode, www.downloadHandler.text);
    }

    public IEnumerator POST (string path, Dictionary<string,string> data, callBackFun callBack) {
        UnityWebRequest www = UnityWebRequest.Post (apiEndpoint + path, data);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        
        callBack ((int) www.responseCode, www.downloadHandler.text);
    }
}
