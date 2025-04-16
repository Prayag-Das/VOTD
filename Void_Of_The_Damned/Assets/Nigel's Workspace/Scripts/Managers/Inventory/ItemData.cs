using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool isEquippable;
    public GameObject prefabWithScript; // Prefab that holds the action script (e.g., flashlight, medkit)
}
