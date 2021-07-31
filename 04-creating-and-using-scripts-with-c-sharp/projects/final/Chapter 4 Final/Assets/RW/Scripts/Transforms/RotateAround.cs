using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour
{
    public Vector3 rotationSpeed;
    public Space rotationSpace;

    private void Update()
    {
        gameObject.transform.Rotate(rotationSpeed, rotationSpace);
    }
}