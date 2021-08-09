using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class MoveToPointClick : MonoBehaviour
{
    NavMeshAgent agent;
    public GameObject projectile;
    public float launchVelocity = 700f;

    // To rotate a turret to target an enemy
    public GameObject turretVertical;
    public GameObject turretHorizontal;

    [SerializeField]
    private int health = 1000;
    [SerializeField]
    private int fullHealth = 1000;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() { }

    public void Reset()
    {
        health = fullHealth;
    }

    public int GetHealth()
    {
        return health;
    }

    public float GetPercentHealth()
    {
        return (float)health / (float)fullHealth;
    }

    public void OnMove()
    {
        RaycastHit hit;
        Debug.Log("Try to move to a new position");

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 100))
        {
            agent.destination = hit.point;
            Debug.Log("Moving to a new position");
        }
    }

    public void OnFire()
    {
        // Rotate the Cannon to the target direction.
        /* TODO: get where the mouse was clicked to rotate to this target.
        Transform targetObj = turretVertical.transform;
        Vector3 point = targetObj.position;
        point.y = 0.0f;
        transform.LookAt(point);
        */

        Vector3 forward = turretVertical.transform.forward;
        Vector3 velocity = forward * launchVelocity;
        Vector3 velocityHand = new Vector3(velocity.z, velocity.y, velocity.x);

        // Where the projectile is started and directed.
        Transform cannon = turretVertical.transform;

        // GameObject fork = Instantiate(projectile, transform.position + new Vector3(0, 2.5f, 0), transform.rotation);
        // fork.GetComponent<Rigidbody>().AddRelativeForce(velocityHand);
        // fork.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(launchVelocity*0.2f, 0, launchVelocity * 0.8f));

        GameObject fork = Instantiate(projectile, cannon.position, cannon.rotation);
        fork.GetComponent<Rigidbody>().AddForce(velocity);
    }

    public void DamagePlayer()
    {
        health -= UnityEngine.Random.Range(0, 5);
    }
}
