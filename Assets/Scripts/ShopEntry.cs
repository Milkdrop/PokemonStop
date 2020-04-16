using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopEntry : MonoBehaviour {

    [HideInInspector]
    public int id;
    [HideInInspector]
    public int itemPrice;

    public Text itemName;
    public Text itemPriceText;
    public Button buyButton;
    [HideInInspector]
    public GameManager gm;

    public void Buy () {
        gm.BuyItem (id, itemPrice);
    }

    public void SetBuyInteractable (bool interactable) {
        buyButton.interactable = interactable;
    }
}
