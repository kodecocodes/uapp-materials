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

public class Plate : InteractionObject
{
    [SerializeField] private List<IngredientObject.IngredientType> ingredients;
    public Recipe Recipe { get; private set; }

    public UnityEvent OnPlateServed;

    /// <summary>
    /// The player can either place an ingredient on the plate if they are carrying one, or take the plate
    /// </summary>
    /// <param name="player"></param>
    public override void Interact(PlayerController player)
    {
        if (player.ingredient != null)
        {
            if (player.ingredient.state == IngredientObject.IngredientState.Chopped)
            {
                if (!ingredients.Contains(player.ingredient.type))
                {
                    ingredients.Add(player.ingredient.type);
                }
                player.SetIngredient(this);
                CheckRecipes();
            }
        }
        else
        {
            player.TakePlate(this);
        }
    }

    /// <summary>
    /// Runs through potential Recipes
    /// </summary>
    private void CheckRecipes()
    {
        Recipe recipe = OrderBook.CheckRecipes(ingredients);
        if (recipe == null)
        {
            // No recipe yet, leave it as it is
            return;
        }
        else
        {
            // found a recipe, so replace all the current ingredients with the recipe model
            foreach (IngredientObject ingredient in transform.GetComponentsInChildren<IngredientObject>())
            {
                ingredient.gameObject.SetActive(false);
            }
            GameObject dish = Instantiate(recipe.prefab, transform);
            dish.transform.SetParent(transform);
            dish.transform.localPosition = Vector3.zero;
            dish.transform.localRotation = Quaternion.identity;
            Recipe = recipe;
        }
    }

    public void Serve()
    {
        // Check for a recipe
        if (Recipe != null)
        {
            Recipe.serveEvent?.Raise();
        }
        else
        {
            // Player served a non-recipe dish, award some points
            OrderBook.instance.Service();
        }
        // Return ingredients to the pool
        foreach (IngredientObject ingredient in transform.GetComponentsInChildren<IngredientObject>(true))
        {
            IngredientPool.Instance.Add(ingredient);
        }

        OnPlateServed?.Invoke();
        Destroy(gameObject, 1f);
    }
}
