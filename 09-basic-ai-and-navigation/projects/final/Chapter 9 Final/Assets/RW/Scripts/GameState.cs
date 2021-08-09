using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    // Assign the empty GameObject that will contain the spawned enemies.
    public GameObject Enemies;
    private GameObject player;

    // Allow to hook-up different spawner sites.
    public List<GateSpawner> spawners = new List<GateSpawner>();

    // For the "Countdown" messages
    public Text MessageBar;

    // For the "Health" messages 
    public RectTransform HealthBar;

    enum States { Countdown, Fight, Battle, Lose };
    private States state;
    private float timeRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Tank");
        player.GetComponent<NavMeshAgent>().isStopped = true;
        Reset();
    }

    private void Reset()
    {
        player.GetComponent<MoveToPointClick>().Reset();
        state = States.Countdown;
        timeRemaining = 3;
        UpdateGUI();
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (player.GetComponent<MoveToPointClick>().GetHealth() < 0)
        {
            state = States.Lose;
            player.GetComponent<NavMeshAgent>().isStopped = true;
            UpdateGUI();
        }

        if (state == States.Countdown)
        {
            if (timeRemaining < 0)
            {
                state = States.Fight;
                timeRemaining = 1.0f;
            }

            UpdateGUI();
        }

        if (state == States.Fight)
        {
            if (timeRemaining < 0)
            {
                state = States.Battle;
                UpdateGUI();
            }
        }

        if (state == States.Battle)
        {
            player.GetComponent<NavMeshAgent>().isStopped = false;
            // Randomly spawn 
            if (timeRemaining < 0)
            {
                int index = UnityEngine.Random.Range(0, spawners.Count);
                int spawnCount = UnityEngine.Random.Range(3, 8);

                GateSpawner g = spawners[index];
                g.SpawnEnemies(spawnCount);

                timeRemaining = 2;
            }
            UpdateGUI();
        }
    }

    void UpdateGUI()
    {
        switch (state)
        {
            case States.Countdown:
                int timer = (int) Math.Ceiling(timeRemaining);
                MessageBar.text = timer.ToString();
                break;
            case States.Fight:
                MessageBar.text = "Fight!";
                break;
            case States.Battle:
                MessageBar.text = "";
                break;
            case States.Lose:
                MessageBar.text = "You Lose!";
                break;
        }

        HealthBar.sizeDelta = new Vector2(735 * player.GetComponent<MoveToPointClick>().GetPercentHealth(), 65);
    }
}
