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
        player.GetComponent<PlayerController>().Reset();
        state = States.Countdown;
        timeRemaining = 3;
        UpdateGUI();
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (player.GetComponent<PlayerController>().GetHealth() < 0)
        {
            // TODO: what happens on a loss
        }

        if (state == States.Countdown)
        {
            // TODO: setup the countdown
        }

        if (state == States.Fight)
        {
            // TODO: setup the GUI text
        }

        if (state == States.Battle)
        {
            // TODO: what changes on entering battle
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
