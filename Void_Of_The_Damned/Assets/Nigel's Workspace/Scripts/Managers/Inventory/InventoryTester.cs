using UnityEngine;
using System.Collections;

public class InventoryTester : MonoBehaviour
{
    public InventorySystem inventorySystem;
    public ItemData testItem;

    void Start()
    {
        // Try adding the item
        inventorySystem.AddItem(testItem);
        Debug.Log("Added item: " + testItem.itemName);

        // Check if player has the item
        if (inventorySystem.HasItem(testItem))
        {
            Debug.Log("Item exists in inventory.");
        }

        // Remove the item
        inventorySystem.RemoveItem(testItem);
        Debug.Log("Removed item: " + testItem.itemName);

        // Check again
        if (!inventorySystem.HasItem(testItem))
        {
            Debug.Log("Item no longer in inventory.");
        }
    }
}

