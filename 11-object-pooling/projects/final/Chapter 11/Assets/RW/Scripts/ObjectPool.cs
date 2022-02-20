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

public class ObjectPool : MonoBehaviour
{
    public int PoolSize;

    public GameObject[] Prefabs;

    [SerializeField]
    private Queue<GameObject> pool = new Queue<GameObject>();

    // Add another GameObject to the managed pool.
    public void Add(GameObject anObject)
    {
        // 1.
        IPoolable poolable = anObject.GetComponent<IPoolable>();
        if (poolable != null)
        {
            // 2.
            pool.Enqueue(anObject);
            poolable.SetPool(this);
        }
    }

    // Fetch an object from the managed pool.
    public GameObject Get()
    {
        // 1. 
        if (pool.Count > 0)
        {
            // 2.
            GameObject toReturn = pool.Dequeue();
            toReturn.GetComponent<IPoolable>().Reset();
            return toReturn;
        }
        // 3.
        return null;
    }

    public void Return(GameObject anObject)
    {
        // 1.
        IPoolable poolable = anObject.GetComponent<IPoolable>();
        if (poolable != null)
        {
            // 2.
            poolable.Deactivate();
            Add(anObject);
        }
    }

    public void Awake()
    {
        // 1. 
        int prefabIndex = 0;
        for (int i = 1; i <= PoolSize; i++)
        {
            // 2.
            GameObject poolMember = Instantiate(Prefabs[prefabIndex], transform);
            // 3.
            poolMember.SetActive(false);
            Add(poolMember);
            // 4.
            if (++prefabIndex == Prefabs.Length)
            {
                prefabIndex = 0;
            }
        }
    }
}