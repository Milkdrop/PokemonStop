using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private int i = 0;
    public Text uiText;

    void Start() {
        InvokeRepeating ("PingServer", 0, 1);
    }

    void PingServer () {
        i++;
        uiText.text = i + "";
    }
}
