using UnityEngine;

public class HazardShower : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SanitySystem sanitySystem = other.GetComponent<SanitySystem>();

            if (sanitySystem != null)
            {
                sanitySystem.CleanseSanity();
            }
        }
    }
}
