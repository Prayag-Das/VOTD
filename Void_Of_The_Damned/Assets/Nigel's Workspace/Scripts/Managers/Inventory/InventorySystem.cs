using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    [Header("Inventory")]
    public List<InventorySlot> inventory = new List<InventorySlot>();
    public int maxSlots = 20;

    public void AddItem(ItemData item)
    {
        InventorySlot slot = inventory.Find(i => i.itemData == item);
        if (slot != null)
        {
            slot.quantity++;
        }
        else if (inventory.Count < maxSlots)
        {
            inventory.Add(new InventorySlot(item, 1));
        }
        else
        {
            Debug.Log("Inventory full!");
        }
    }

    public void RemoveItem(ItemData item)
    {
        InventorySlot slot = inventory.Find(i => i.itemData == item);
        if (slot != null)
        {
            slot.quantity--;
            if (slot.quantity <= 0)
                inventory.Remove(slot);
        }
    }

    public bool HasItem(ItemData item)
    {
        return inventory.Exists(i => i.itemData == item);
    }
}
