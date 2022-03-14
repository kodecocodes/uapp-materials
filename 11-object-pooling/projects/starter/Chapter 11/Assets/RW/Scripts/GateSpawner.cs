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

public class GateSpawner : MonoBehaviour
{
    public GameObject Enemy;

    public GameObject Gate;
    public GameObject Container;
    public float height = 5;
    public float offset = 0;

    private enum State { Ready, Raising, Lowering};
    private State state;
    public Vector3 origin;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Ready;
        origin = Gate.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Raising)
        {
            if (offset < height)
            {
                offset += Time.deltaTime;
                Gate.transform.Translate(new Vector3(0, 0, Time.deltaTime));
            } else
            {
                state = State.Lowering;
            }
        }

        if (state == State.Lowering)
        {
            if (offset > 0)
            {
                offset -= Time.deltaTime;
                Gate.transform.Translate(new Vector3(0, 0, -Time.deltaTime));
            } else
            {
                state = State.Ready;
            }
        }
    }

    public void SpawnEnemies(int number)
    {
        if (state == State.Ready)
        {
            for (int i = 0; i < number; i++)
            {
                GameObject enemy = Instantiate(Enemy, Gate.transform.parent);
                if (enemy != null)
                {
                    Vector3 forward = Gate.transform.forward;
                    enemy.transform.position = Gate.transform.position + Gate.transform.forward * 5f;
                    enemy.GetComponent<EnemyController>().Enable();
                }
            }
            state = State.Raising;
        }
    }
}
