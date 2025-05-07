using UnityEngine;

public class BlueBox : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Task-3-Station-Blue"))
        {
            Destroy(gameObject);
        }
    }
}
