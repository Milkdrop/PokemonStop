using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameManager gm;
    public float modelWidth;
    public float modelHeight;

    void Start() {
        transform.localScale = new Vector3 (Screen.width / modelWidth, Screen.height / modelHeight, 1);

        if (gm.token == "") {
            SpawnRegisterScreen ();
        }
    }

    public void SpawnRegisterScreen () {

    }

    public void SpawnLoginScreen () {

    }
}