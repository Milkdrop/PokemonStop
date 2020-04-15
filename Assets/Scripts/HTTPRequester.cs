using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class HTTPRequester : MonoBehaviour {
    
    public static string apiEndpoint = "http://api.peymen.com/";
    private static string returnValue;

    public static string GET (string path) {
        StartCoroutine (InternalGET (path));

        return returnValue;
    }

    private static void SetReturnValue (string value) {
        returnValue = value;
    }

    public static IEnumerator InternalGET (string path) {
        UnityWebRequest www = UnityWebRequest.Get (apiEndpoint + path);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            Debug.Log("Form upload complete!");
        }

        returnValue = www.downloadHandler.text
    }

    private static IEnumerator InternalPOST () {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        UnityWebRequest www = UnityWebRequest.Post ("https://webhook.site/5de62bce-d222-456a-9232-5ba6824e1905/gm", formData);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            Debug.Log("Form upload complete!");
        }
    }
}
