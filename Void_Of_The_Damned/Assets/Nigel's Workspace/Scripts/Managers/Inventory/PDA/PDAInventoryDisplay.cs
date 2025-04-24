using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


public class PDAInventoryDisplay : MonoBehaviour
{
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private Transform itemListParent;
    [SerializeField] private GameObject itemUIPrefab;

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (Transform child in itemListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (InventorySlot slot in inventorySystem.inventory)
        {
            GameObject itemUI = Instantiate(itemUIPrefab, itemListParent);
            itemUI.GetComponentInChildren<TextMeshProUGUI>().text = $"{slot.itemData.itemName} x{slot.quantity}";
            //itemUI.GetComponentInChildren<Image>().sprite = slot.itemData.icon;
        }
    }
}
