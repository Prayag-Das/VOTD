using UnityEngine;

public class StarDestroyerMover : MonoBehaviour
{
    public float speed = 12f;       // Speed of movement
    public float resetZ = 260f;     // Z position to reset to
    public float thresholdZ = -170f; // Z position to reset at

    void Update()
    {
        // Move toward negative Z
        transform.position += new Vector3(0f, 0f, -speed * Time.deltaTime);

        // Teleport back if passed the threshold
        if (transform.position.z <= thresholdZ)
        {
            Vector3 pos = transform.position;
            pos.z = resetZ;
            transform.position = pos;
        }
    }
}
