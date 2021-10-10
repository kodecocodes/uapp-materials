using UnityEngine;

public class Ammunition : MonoBehaviour
{
    public float movementSpeed = 5f;

    private void Update()
    {
        transform.Translate(Vector3.right * movementSpeed * Time.deltaTime, Space.World);

        if (transform.position.x > 20f)
        {
            Destroy(gameObject);
        }
    }
}