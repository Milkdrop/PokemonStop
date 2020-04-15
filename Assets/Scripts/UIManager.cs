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

    [Header ("Creature Screen")]
    public GameObject FetchingMask;
    public Transform Creatures;
    public Text UsernameText;
    public Text DayCounterText;
    public RectTransform XPBarFill;
    public Text XPText;

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

    public void UpdateMenuScreen () {
        FetchingMask.SetActive (false);
        UsernameText.text = "Hello, " + gm.username + "!";
        DayCounterText.text = gm.creatureLevel + "";
        XPText.text = gm.creatureXP + "/" + gm.creatureMaxXP + "xp";

        if (gm.creatureXP == 0) {
            XPBarFill.sizeDelta = new Vector2 (0, XPBarFill.rect.height);
        } else {
            int width = 50 + (gm.creatureXP / gm.creatureMaxXP) * (633 - 50);
            XPBarFill.sizeDelta = new Vector2 (width, XPBarFill.rect.height);
        }
        
        int creatureEvolution = 0;
        if (gm.creatureLevel >= 30) {
            creatureEvolution = 2;
        } else if (gm.creatureLevel >= 5) {
            creatureEvolution = 1;
        }

        for (int i = 0; i < Creatures.childCount; i++) {
            GameObject child = Creatures.GetChild (i).gameObject;

            if (i == creatureEvolution)
                child.gameObject.SetActive (true);
            else
                child.gameObject.SetActive (false);
        }
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