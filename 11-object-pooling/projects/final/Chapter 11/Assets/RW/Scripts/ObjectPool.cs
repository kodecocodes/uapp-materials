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
    // DesiredPoolSize
    public int PoolSize;

    // Prefabs
    public GameObject[] Prefabs;

    [SerializeField]
    private Queue<GameObject> pool = new Queue<GameObject>();

    public void Awake()
    {
        // Attempt to automatically instantiate the pool from Prefabs.
        int j = 0;
        for (int i = 1; i <= PoolSize; i++)
        {
            int index = j++;
            GameObject poolMember = Instantiate(Prefabs[index], transform);
            poolMember.SetActive(false);
            IPoolable[] poolableList = poolMember.GetComponents<IPoolable>();
            foreach (IPoolable poolable in poolableList)
            {
                poolable.SetPool(this);
            }
            pool.Enqueue(poolMember);
            if (j == Prefabs.Length)
            {
                j = 0;
            }
        }
    }

    // Add another GameObject to the managed pool.
    public void Add(GameObject anObject)
    {
        IPoolable[] poolableList = anObject.GetComponents<IPoolable>();
        foreach (IPoolable poolable in poolableList)
        {
            poolable.SetPool(this);
        }
        pool.Enqueue(anObject);
    }

    // Fetch an object from the managed pool.
    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject toReturn = pool.Dequeue();
            toReturn.GetComponent<IPoolable>().Reset();
            return toReturn;
        }
        return null;
    }

    public void Return(GameObject anObject)
    {
        // Return an object back to the pool.
        IPoolable[] poolableList = anObject.GetComponents<IPoolable>();
        foreach (IPoolable poolable in poolableList) { 
            poolable.Deactivate();
        }
        pool.Enqueue(anObject);
    }

    public void Dispose()
    {
        // Should destroy all the objects in the pool.
        while (pool.Count > 0)
        {
            Destroy(pool.Dequeue());
        }
    }
}
