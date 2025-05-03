using UnityEngine;
using System.Collections;


public class OxygenRefillClosedProxy : MonoBehaviour, IInteractable
{
    [SerializeField] private OxygenRefillStation actualStation;

    public void Interact()
    {
        Debug.Log("Oxygen refill complete.");
    }
}
