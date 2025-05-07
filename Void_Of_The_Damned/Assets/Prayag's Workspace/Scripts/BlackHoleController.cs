using UnityEngine;

public class BlackHoleController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector3 startPosition = new Vector3(136.5f, 9.3f, -857.44f);
    [SerializeField] private Vector3 endPosition = new Vector3(104.3f, 9.3f, -220.1f);
    [SerializeField] private float duration = 420f; // 7 minutes

    private float elapsedTime = 0f;

    private void Start()
    {
        transform.position = startPosition;
    }

    private void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
        }
    }
}
