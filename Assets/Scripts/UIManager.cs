using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameManager gm;
    public float modelWidth;
    public float modelHeight;

    public GameObject registerScreen;
    public GameObject loginScreen;

    public InputField registerEmail;
    public InputField registerPassword;
    public InputField registerPasswordRepeat;
    public InputField registerLat;
    public InputField registerLong;

    public InputField loginEmail;
    public InputField loginPassword;

    void Start() {
        transform.localScale = new Vector3 (Screen.width / modelWidth, Screen.height / modelHeight, 1);

        if (gm.token == "") {
            SpawnRegisterScreen ();
        }
    }

    public void Register () {
        if (registerPassword.text == registerPasswordRepeat.text) {
            gm.Register (registerEmail.text, registerPassword.text, registerLat.text, registerLong.text);
        } else {
            Debug.Log ("Passwords don't match");
        }
    }

    public void SpawnRegisterScreen () {
        DeactivateScreens ();
        registerScreen.SetActive (true);
    }

    public void SpawnLoginScreen () {
        DeactivateScreens ();
        loginScreen.SetActive (true);
    }

    void DeactivateScreens () {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild (i).gameObject.SetActive (false);
        }
    }
}