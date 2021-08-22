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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupArea : InteractionObject
{
    [SerializeField] private IngredientObject.IngredientType ingredientType;
    [SerializeField] private List<IngredientObject> availableIngredients;
    [SerializeField] private List<Transform> spawnPositions;
    [SerializeField] private Transform dropPosition;

    [SerializeField] private IngredientPool publicSupply;

    [SerializeField]
    private float spawnRate = 5f;
    private float timeSinceSpawn = 0f;

    private void Start()
    {
        timeSinceSpawn = UnityEngine.Random.Range(0f, spawnRate);
        foreach (IngredientObject ingredient in availableIngredients)
        {
            ingredient.Lerp(dropPosition, spawnPositions[availableIngredients.IndexOf(ingredient)]);
        }
    }

    private void Update()
    {
        timeSinceSpawn += Time.deltaTime;
        if (timeSinceSpawn > spawnRate)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        if (availableIngredients.Count < spawnPositions.Count)
        {
            //space for more ingredients
            IngredientObject newIngredient = publicSupply.Fetch(ingredientType);
            if (newIngredient != null)
            {
                availableIngredients.Add(newIngredient);
                newIngredient.transform.parent = transform;
                newIngredient.Lerp(dropPosition, spawnPositions[availableIngredients.IndexOf(newIngredient)]);
            }
        }
        timeSinceSpawn = 0;
    }

    public override void Interact(PlayerController player)
    {
        if (availableIngredients.Count == 0)
        {
            return;
        }

        player.TakeIngredient(availableIngredients[0]);
        availableIngredients.RemoveAt(0);
        Shuffle();
    }

    private void Shuffle()
    {
        for(int i = 0; i < availableIngredients.Count; i++)
        {
            availableIngredients[i].Lerp(availableIngredients[i].gameObject.transform, spawnPositions[i]);
        }
    }
}
