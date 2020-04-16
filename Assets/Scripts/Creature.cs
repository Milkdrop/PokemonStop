using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

    public GameObject[] cosmetics;

    public void ClearCosmetics () {
        for (int i = 0; i < cosmetics.Length; i++) {
            cosmetics[i].gameObject.SetActive (false);
        }
    }

    public void ActivateCosmetic (int id) {
        cosmetics [id].gameObject.SetActive (true);
    }
}
