using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class CreatureUIManager : MonoBehaviour
{
    [Header ("Config")]
    public UIManager uiMan;
    public HTTPRequester httpReq;
    public GameManager gm;
    public GameObject LoadingScreen;
    public Transform Screens;

    [Header ("Main Creature")]
    public Transform Creatures;
    public Text UsernameText;
    public Transform DistanceObject;
    public Text DistanceText;
    public Text DayCounterText;
    public RectTransform XPBarFill;
    public Text XPText;

    [Header ("Leaderboard")]
    private float lastLeaderboardUpdate;
    public GameObject LeaderboardScreen;
    public GameObject LeaderboardEntry;
    public Transform LeaderboardContentParent;

    void Start () {
        LoadingScreen.SetActive (true);
        for (int i = 0; i < 100; i++) {
            GameObject leaderboardEntry = Instantiate (LeaderboardEntry, Vector3.zero, Quaternion.identity, LeaderboardContentParent);
            leaderboardEntry.SetActive (false);
        }
    }

    public void UpdateMenuScreen () {
        LoadingScreen.SetActive (false);
        UsernameText.text = "Hello, " + gm.username + "!";
        DistanceObject.localPosition = new Vector2 (-320f + UsernameText.preferredWidth, DistanceObject.localPosition.y);
        DistanceText.color = new Color (255, 255, 255);

        if (gm.distanceFromHome == null) {
            DistanceText.text = "0m";
        } else {
            DistanceText.text = gm.distanceFromHome + "m";
            if (int.Parse (gm.distanceFromHome) > 50) {
                DistanceText.color = new Color (255, 127, 127);
            }
        }

        DayCounterText.text = gm.creatureLevel + "";
        XPText.text = gm.creatureXP + "/" + gm.creatureMaxXP + "xp";

        if (gm.creatureXP == 0) {
            XPBarFill.sizeDelta = new Vector2 (0, XPBarFill.rect.height);
        } else {
            int width = 50 + (int) (((double) gm.creatureXP / gm.creatureMaxXP) * (633 - 50));
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

    public void ToggleLeaderboardScreen () {
        bool isActive = LeaderboardScreen.activeSelf;
        DeactivateScreens ();
        if (!isActive) {
            if (lastLeaderboardUpdate == 0 || Time.time - lastLeaderboardUpdate > 60) {
                lastLeaderboardUpdate = Time.time;
                StartCoroutine (httpReq.GET ("/leaderboard", PopulateLeaderboardData));
            } else {
                LeaderboardScreen.SetActive (true);
            }
        }
    }

    public void PopulateLeaderboardData (int responseCode, string data) {
        bool valid = false;

        if (responseCode == 200) {
            List<Dictionary<string,string>> response = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(data);
            LeaderboardContentParent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 200 * response.Count + 100);

            for (int i = 0; i < response.Count && i < 100; i++) {
                LeaderboardEntry entry = LeaderboardContentParent.GetChild (i).GetComponent<LeaderboardEntry> ();

                entry.entryLevel.text = response[i]["level"];
                entry.entryUsername.text = response[i]["name"];
                entry.entryResearchPts.text = "Research Points (RP): " + response[i]["research_points"];
                entry.entryPosition.text = "#" + (i + 1);

                entry.transform.localPosition = new Vector2 (450, - 100 - 200 * i);
                entry.gameObject.SetActive (true);
            }

            for (int i = response.Count; i < 100; i++) {
                LeaderboardContentParent.GetChild (i).gameObject.SetActive (false);
            }

            valid = true;
        } else {
            uiMan.PushError ("Unknown Error");
        }

        if (valid) {
            LeaderboardScreen.SetActive (true);
        } else {
            uiMan.PushError ("Unknown Error");
        }
    }

    void DeactivateScreens () {
        for (int i = 0; i < Screens.childCount; i++) {
            Screens.GetChild (i).gameObject.SetActive (false);
        }
    }
}
