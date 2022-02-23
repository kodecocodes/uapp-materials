/// Copyright (c) 2021 Razeware LLC
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// Notwithstanding the foregoing, you may not use, copy, modify, merge, publish,
/// distribute, sublicense, create a derivative work, and/or sell copies of the
/// Software in any work that is designed, intended, or marketed for pedagogical or
/// instructional purposes related to programming, coding, application development,
/// or information technology.  Permission for such use, copying, modification,
/// merger, publication, distribution, sublicensing, creation of derivative works,
/// or sale is expressly withheld.
///
/// This project and source code may use libraries or frameworks that are
/// released under various Open-Source licenses. Use of those libraries and
/// frameworks are governed by their own individual licenses.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    public ObjectPool ProjectilePool;
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
        //Debug.Log("Try to move to a new position");

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 100))
        {
            agent.destination = hit.point;
            //Debug.Log("Moving to a new position");
        }
    }

    public void OnFire()
    {
        // On a mouse click, get the mouse current position as Vector2Control.
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Mouse.html
        float x =  Mouse.current.position.x.ReadValue();
        float y = Mouse.current.position.y.ReadValue();

        //Debug.Log("Fire at mouse " + x + ", " + y);

        // Next, ray trace to find where on the map the player clicked.
        // https://docs.unity3d.com/ScriptReference/Camera.ScreenToWorldPoint.html
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, Camera.main.nearClipPlane));

        // Find the raycast to hit the surface, turn turret to face.
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 target = hit.point;
            target.y = turretHorizontal.transform.position.y;
            turretHorizontal.transform.LookAt(target);
        }

        Vector3 forward = turretVertical.transform.forward;
        Vector3 velocity = forward * launchVelocity;
        Vector3 velocityHand = new Vector3(velocity.z, velocity.y, velocity.x);

        // Where the projectile is started and directed.
        Transform cannon = turretVertical.transform;
        // Get a projectile from the pool.
        GameObject projectile = ProjectilePool.Get();
        if (projectile)
        {
            projectile.transform.position = cannon.position;
            projectile.transform.rotation = cannon.rotation;
            projectile.GetComponent<Rigidbody>().AddForce(velocity);
        }
    }

    public void DamagePlayer()
    {
        health -= UnityEngine.Random.Range(0, 5);
    }
}
