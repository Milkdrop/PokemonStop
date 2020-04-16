using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header ("Config")]
    public GameManager gm;
    public float modelWidth;
    public float modelHeight;
    public Text errorText;

    [Header ("Screens")]
    public GameObject registerScreen;
    public GameObject loginScreen;
    public GameObject menuScreen;

    [Header ("Registration")]
    public InputField registerName;
    public InputField registerEmail;
    public InputField registerPassword;
    public InputField registerPasswordRepeat;
    public InputField registerLocation;

    [Header ("Login")]
    public InputField loginEmail;
    public InputField loginPassword;

    void Start() {
        transform.localScale = new Vector3 (Screen.width / modelWidth, Screen.height / modelHeight, 1);

        if (gm.token == "") {
            SpawnRegisterScreen ();
        } else {
            gm.InitDaemon ();
        }
    }

    public void Register () {
        if (registerPassword.text == registerPasswordRepeat.text) {
            string[] location = registerLocation.text.Split (',');
            if (location.Length != 2) {
                PushError ("Location input is invalid. An example for valid input is 1.2, 2.3");
            } else {
                if (double.TryParse(location[0], out double num) && double.TryParse (location[1], out double num2)) {
                    gm.Register (registerName.text, registerEmail.text, registerPassword.text, location[0], location[1]);
                } else {
                    PushError ("Location input is invalid. An example for valid input is 1.2, 2.3");
                }
            }
        } else {
            PushError ("Passwords don't match!");
        }
    }

    public void Login () {
        gm.Login (loginEmail.text, loginPassword.text);
    }

    public void SpawnRegisterScreen () {
        DeactivateScreens ();
        UpdateRegisterScreen ();
        registerScreen.SetActive (true);
    }

    public void UpdateRegisterScreen () {
        StartCoroutine (UpdateRegisterLocation ());
    }

    private IEnumerator UpdateRegisterLocation () {
        if (!Input.location.isEnabledByUser) {
            Debug.Log ("User didn't enable location");
            yield break;
        }

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            Debug.Log ("Initializing location services");
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait == 0) {
            Debug.Log ("Timed out while obtaining location");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed) {
            Debug.Log ("Unable to determine device location");
            yield break;
        } else {
            registerLocation.text = Input.location.lastData.latitude + ", " + Input.location.lastData.longitude;
        }
    }

    public void SpawnLoginScreen () {
        DeactivateScreens ();
        loginScreen.SetActive (true);
    }

    public void SpawnMenuScreen () {
        DeactivateScreens ();
        menuScreen.SetActive (true);
    }

    void DeactivateScreens () {
        PushError ("");
        for (int i = 1; i < transform.childCount - 1; i++) { // First and last children are non menus
            transform.GetChild (i).gameObject.SetActive (false);
        }
    }

    public void PushError (string error) {
        errorText.text = error;
    }
}