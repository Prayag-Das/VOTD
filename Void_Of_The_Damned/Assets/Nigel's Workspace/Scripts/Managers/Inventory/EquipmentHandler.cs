using UnityEngine;
using System.Collections;


public class EquipmentHandler : MonoBehaviour
{
    [Header("Equip Settings")]
    [SerializeField] private KeyCode equipKey = KeyCode.Alpha1;  // Can change in Inspector
    [SerializeField] private GameObject signalTunerPrefab;       // Drag prefab or GameObject
    [SerializeField] private Transform equipPoint;               // Where the tool appears (e.g. player hand)

    private GameObject currentEquippedItem;
    private bool isEquipped = false;

    private void Update()
    {
        HandleEquipToggle();
    }

    private void HandleEquipToggle()
    {
        if (Input.GetKeyDown(equipKey))
        {
            if (isEquipped)
                UnequipItem();
            else
                EquipItem();
        }
    }

    private void EquipItem()
    {
        if (signalTunerPrefab != null && currentEquippedItem == null)
        {
            currentEquippedItem = Instantiate(signalTunerPrefab, equipPoint.position, equipPoint.rotation, equipPoint);
            isEquipped = true;
        }
    }

    private void UnequipItem()
    {
        if (currentEquippedItem != null)
        {
            Destroy(currentEquippedItem);
            isEquipped = false;
        }
    }
}
