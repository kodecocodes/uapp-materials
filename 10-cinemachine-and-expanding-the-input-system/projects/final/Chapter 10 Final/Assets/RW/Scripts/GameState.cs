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

using Cinemachine;
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
    public GameObject DestroyedTank;
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

    // Priority of the cameras (2 makes active)
    const int lowPriority = 1;
    const int highPriority = 2;
    const int spawnTimer = 5;

    // 1. Cameras
    public CinemachineVirtualCamera zoomCamera;
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera gateCamera;

    // 2. Activate Camera method
    public void ActivateCamera(CinemachineVirtualCamera camera)
    {
        zoomCamera.Priority = lowPriority;
        playerCamera.Priority = lowPriority;
        gateCamera.Priority = lowPriority;
        camera.Priority = highPriority;
        camera.MoveToTopOfPrioritySubqueue();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Tank");
        player.GetComponent<NavMeshAgent>().isStopped = true;
        Reset();
    }

    private void Reset()
    {
        player.SetActive(true);
        player.GetComponent<PlayerController>().Reset();
        state = States.Countdown;
        timeRemaining = 3;
        UpdateGUI();
        ActivateCamera(zoomCamera);
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (player.GetComponent<PlayerController>().GetHealth() < 0)
        {
            state = States.Lose;

            // Hide the player.
            player.SetActive(false);
            ActivateCamera(zoomCamera);
            UpdateGUI();

            // Replace with destroyed tank.
            DestroyedTank.transform.position = player.transform.position;
            DestroyedTank.transform.rotation = player.transform.rotation;
            DestroyedTank.SetActive(true);
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
            // Create initial enemies and zoom.
            int gate = 0;
            GateSpawner g = spawners[gate];
            g.SpawnEnemies(5);
            ActivateCamera(gateCamera);

            if (timeRemaining < 0)
            {
                ActivateCamera(playerCamera);
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

        HealthBar.sizeDelta = new Vector2(735 * player.GetComponent<PlayerController>().GetPercentHealth(), 65);
    }
}
