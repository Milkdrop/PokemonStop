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
    public GameObject menuScreen;

    public InputField registerName;
    public InputField registerEmail;
    public InputField registerPassword;
    public InputField registerPasswordRepeat;
    public InputField registerLocation;

    public InputField loginEmail;
    public InputField loginPassword;

    public Text errorText;

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
        registerScreen.SetActive (true);
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