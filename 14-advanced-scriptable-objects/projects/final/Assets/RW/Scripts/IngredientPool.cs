/*
 * Copyright (c) 2021 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientPool : MonoBehaviour
{
    public List<IngredientObject.IngredientType> poolFilter;
    [SerializeField]
    private List<IngredientObject> pool;

    public static IngredientPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Returns an ingredient from the pool if one is available. Otherwise returns null
    /// </summary>
    /// <param name="type"></param>
    /// <returns>Ingredient Object. Null if no ingredients matching type were available</returns>
    public IngredientObject Fetch(IngredientObject.IngredientType type)
    {
        IngredientObject toReturn;
        for (int i = 0; i < pool.Count-1; i++) 
        {
            if (pool[i].type == type)
            {
                toReturn = pool[i];
                pool.RemoveAt(i);
                return toReturn;
            }
        }
        return null;
    }

    /// <summary>
    /// Return an object to the pool
    /// </summary>
    /// <param name="ingredient"></param>
    public void Add(IngredientObject ingredient)
    {
        if (poolFilter.Contains(ingredient.type))
        {
            pool.Add(ingredient);
            ingredient.ChangeState(IngredientObject.IngredientState.Raw);
            ingredient.transform.position = new Vector3(0, -10, 0);
            ingredient.gameObject.SetActive(true);
            ingredient.transform.SetParent(transform);
        }
        else
        {
            Destroy(ingredient.gameObject);
        }
    }
}
