using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarrotController : MonoBehaviour
{
    Animator characterAnimator;
    NavMeshAgent agent;
    GameObject player;

    enum States { Ready, Attack, Dead };
    private States state;
    private float timeRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {
        state = States.Ready;
        characterAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Tank");
    }

    // Update is called once per frame
    void Update()
    {
        if (state == States.Ready)
        {
            if (!agent.isOnNavMesh)
            {
                agent.enabled = false;
                agent.enabled = true;
            } 
            else
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
        }
        if (state == States.Attack)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0)
            {
                state = States.Ready;
                if (Vector3.Distance(player.transform.position, gameObject.transform.position) < 5.0f)
                {
                    player.GetComponent<MoveToPointClick>().DamagePlayer();
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
            Destroy(gameObject, 5);
            state = States.Dead;
            agent.isStopped = true;
        }
    }
}
