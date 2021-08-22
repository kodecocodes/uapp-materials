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

public class Workstation : InteractionObject
{
    // The state an ingredient should be in for use
    public IngredientObject.IngredientState stateIn;
    // The state an ingredient will be in after process
    public IngredientObject.IngredientState stateOut;

    public Transform processingPosition;

    public UnityEvent OnInteract;

    public float processingTime = 3f;
    private float processedTime = 0f;
    private bool processing;
    private IngredientObject processedIngredient;
    private PlayerController interactingPlayer;

    public override void Interact(PlayerController player)
    {
        interactingPlayer = player;
        Process(player.ingredient);
    }

    private void ReturnIngredient()
    {
        interactingPlayer.TakeIngredient(processedIngredient);
    }

    public void Process(IngredientObject ingredient)
    {
        if (ingredient.state == stateIn)
        {
            interactingPlayer.ToggleMovement(false);
            ingredient.transform.parent = processingPosition;
            ingredient.Lerp(ingredient.transform, processingPosition);
            processing = true;
            OnInteract?.Invoke();
            if (processedIngredient != ingredient)
            {
                processedIngredient = ingredient;
                processedTime = 0f;
            }
        }
    }

    public void Update()
    {
        if (processing)
        {
            processedTime += Time.deltaTime;
            if (processedTime >= processingTime)
            {
                // Ingredient has been processed, change it's state
                processedIngredient.ChangeState(stateOut);
                processing = false;
                ReturnIngredient();
            }
        }
    }
}
