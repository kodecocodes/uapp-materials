using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour
{
    public Vector3 rotationSpeed;
    public Space rotationSpace;

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, rotationSpace);
    }
}