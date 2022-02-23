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

public class EnemyController : MonoBehaviour, IPoolable
{
    ObjectPool EnemyPool;
    Animator characterAnimator;
    NavMeshAgent agent;
    GameObject player;

    enum States { Ready, Attack, Dead };
    private States state;
    private float timeRemaining = 0;

    void Awake()
    {
        state = States.Ready;
        characterAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Tank");
    }

    // Enable ensures that the agent is on the NavMesh.
    public void Enable()
    {
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(transform.position, out closestHit, 500, 1))
        {
            transform.position = closestHit.position;
        }
        gameObject.SetActive(true);
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == States.Ready)
        {
            agent.SetDestination(player.transform.position);
            characterAnimator.SetFloat("Speed", agent.velocity.magnitude);


            if (agent.remainingDistance < 5.0f)
            {
                agent.isStopped = true;
                characterAnimator.SetBool("Attack", true);

                state = States.Attack;
                timeRemaining = 1f;

            } else
            {
                agent.isStopped = false;
                characterAnimator.SetBool("Attack", false);
            }
        }
        if (state == States.Attack)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0)
            {
                state = States.Ready;
                if (Vector3.Distance(player.transform.position, gameObject.transform.position) < 5.0f)
                {
                    player.GetComponent<PlayerController>().DamagePlayer();
                }
            }
        }
    }

    //Upon collision with a trigger GameObject, queue the carrot death.
    private void OnTriggerEnter(Collider other)
    {
        if (state != States.Dead)
        {
            // trigger the character death.
            characterAnimator.SetBool("Death", true);
            // Destroy(gameObject, 5);
            state = States.Dead;
            if (agent.isActiveAndEnabled)
            {
                agent.isStopped = true;
            }
            StartCoroutine(DieCoroutine());
        }
    }

    IEnumerator DieCoroutine()
    {
        // Death should return object to the pool.
        yield return new WaitForSeconds(5);
        // Destroy(gameObject);
        if (EnemyPool)
        {
            EnemyPool.Return(gameObject);
        }
    }

    public void Reset()
    {
        // Reset should clear any states to ready.
        state = States.Ready;
        characterAnimator.SetBool("Death", false);
    }

    public void Deactivate()
    {
        // Deactivate should pause the agent and deactivate the object.
        agent.isStopped = true;
        gameObject.SetActive(false);
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
    }

    public void SetPool(ObjectPool pool)
    {
        EnemyPool = pool;
    }
}
