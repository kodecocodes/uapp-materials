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
using UnityEngine.Events;

public class OrderBook : MonoBehaviour
{
    [Serializable]
    public class RecipeEvent : UnityEvent<Recipe> { }
    [Serializable]
    public class StringEvent : UnityEvent<string> { }

    public List<Recipe> recipes;

    public static OrderBook instance;

    private List<Recipe> orders = new List<Recipe>();
    private int score;
    private float orderFrequency = 10f;
    private float timeSinceLastOrder = 0f;
    private int maxOrders = 4;

    public RecipeEvent OnOrderCreated;
    public RecipeEvent OnOrderFilled;
    public StringEvent OnScoreUpdated;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            foreach (Recipe recipe in recipes)
            {
                recipe.ingredients.Sort();
            }
            OnScoreUpdated?.Invoke(score.ToString());
            CreateOrder();
            maxOrders = recipes.Count + 1;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Recipe CheckRecipes(List<IngredientObject.IngredientType> ingredients)
    {
        ingredients.Sort();
        foreach (Recipe recipe in instance.recipes)
        {
            bool match = true;
            foreach (IngredientObject.IngredientType type in recipe.ingredients)
            {
                //Debug.LogFormat("Check {0} on plate? {1}", type, ingredients.Contains(type));
                if (!ingredients.Contains(type))
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                return recipe;
            }
        }

        return null;
    }

    public void CreateOrder()
    {
        Recipe newOrder = recipes[0];
        orders.Add(newOrder);
        OnOrderCreated?.Invoke(newOrder);
    }

    public void Service(Recipe recipe)
    {
        if (instance.orders != null && instance.orders.Count > 0)
        {
            if (instance.orders.Contains(recipe))
            {
                instance.orders.Remove(recipe);
                OnOrderFilled?.Invoke(recipe);
                score += recipe.score;
            }
            else
            {
                score += 5;
            }
        }
        OnScoreUpdated?.Invoke(score.ToString());
    }

    public void Service()
    {
        score += 5;
        OnScoreUpdated?.Invoke(score.ToString());
    }

    private void Update()
    {
        if (orders.Count < maxOrders)
        {
            if (timeSinceLastOrder < orderFrequency)
            {
                timeSinceLastOrder += Time.deltaTime;
            }
            else
            {
                CreateOrder();
                timeSinceLastOrder = 0;
            }
        }
    }
}
