using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lean.Localization;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour
{
    [HideInInspector] public MenuShop menu;
    public string skinName;
    public int skinIndex, price;
    public Button buyButton, equipButton;
    public TextMeshProUGUI buyButtonPrice;
    public LeanLocalizedTextMeshProUGUI equipButtonText;
    public Color inUseColor;
    public Image bg;
    bool purchased;

    public void UpdateItem()
    {
        purchased = false;
        for (int i = 0; i < menu.save.loadedSave.ownedSkins.Count; i++)
        {
            if (menu.save.loadedSave.ownedSkins[i] == skinName)
            {
                purchased = true;
                break;
            }
        }
        if (purchased)
        {
            buyButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(true);
            if (menu.save.loadedSave.currentSkin == skinIndex)
            {
                equipButtonText.TranslationName = "InUse";
                bg.color = inUseColor;
            } else
            {
                equipButtonText.TranslationName = "Use";
                bg.color = Color.clear;
            }
        } else
        {
            buyButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
            buyButtonPrice.text = price.ToString();
            bg.color = Color.clear;
        }
    }

    public void Buy()
    {
        if (menu.save.loadedSave.coins < price || purchased) return;
        menu.save.loadedSave.ownedSkins.Add(skinName);
        menu.save.loadedSave.coins -= price;
        menu.save.SaveFile();
        menu.UpdateShop();
        EventSystem.current.SetSelectedGameObject(equipButton.gameObject);
    }

    public void Equip()
    {
        if (!purchased) return;
        menu.save.loadedSave.currentSkin = skinIndex;
        menu.save.SaveFile();
        menu.UpdateShop();
    }
}
