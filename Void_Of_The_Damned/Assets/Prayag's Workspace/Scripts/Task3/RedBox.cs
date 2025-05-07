using UnityEngine;

public class RedBox : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Task-3-Station-Red"))
        {
            Destroy(gameObject);
        }
    }
}
