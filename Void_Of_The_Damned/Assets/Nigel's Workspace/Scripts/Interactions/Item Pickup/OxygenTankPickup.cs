using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class OxygenTankPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private float refillAmount = 30f; // How much oxygen this tank restores

    public void Interact()
    {
        OxygenSystem oxygenSystem = FindFirstObjectByType<OxygenSystem>();
        if (oxygenSystem != null)
        {
            oxygenSystem.RefillOxygen(refillAmount);
            Debug.Log($"Oxygen tank used. Refilled {refillAmount} oxygen.");

            Destroy(gameObject); // Remove the tank after use
        }
        else
        {
            Debug.LogWarning("OxygenSystem not found! Cannot refill oxygen.");
        }
    }
}
