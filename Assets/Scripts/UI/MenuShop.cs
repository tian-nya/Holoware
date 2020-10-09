using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuShop : Menu
{
    public SaveData save;
    public ShopItem[] shopItems;
    public TextMeshProUGUI coins;

    public void UpdateShop()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            if (shopItems[i].menu == null) shopItems[i].menu = this;
            shopItems[i].UpdateItem();
        }
        coins.text = save.loadedSave.coins.ToString();
    }
}
