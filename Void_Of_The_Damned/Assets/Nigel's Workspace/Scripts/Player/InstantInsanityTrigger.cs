using UnityEngine;
using System.Collections;

public class InstantInsanityTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SanitySystem sanity = FindFirstObjectByType<SanitySystem>();
            if (sanity != null)
            {
                sanity.TriggerInstantInsanity();
            }
        }
    }
}
