using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;

    public void Interact()
    {
        InventorySystem inventory = Object.FindFirstObjectByType<InventorySystem>();
        if (inventory != null && itemData != null)
        {
            inventory.AddItem(itemData);
            Debug.Log($"Picked up {itemData.itemName}");
            Destroy(gameObject); // Remove item from scene
        }
    }
}