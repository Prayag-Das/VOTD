using UnityEngine;

public class GreenBox : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Task-3-Station-Green"))
        {
            Destroy(gameObject);
        }
    }
}
