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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;

    [SerializeField]
    private float playerSpeed = 2.0f;
    public InputAction moveInput;

    [SerializeField]
    private List<InteractionObject> pickUpZones;
    [SerializeField]
    private Workstation choppingBoard;
    [SerializeField]
    private Prop knife;
    [SerializeField]
    private Workstation sink;
    [SerializeField]
    private PlateStack plateStack;
    [SerializeField]
    private ThePass thePass;
    [HideInInspector]
    public Plate carriedPlate;

    // state for holding something or not
    [SerializeField]
    private bool holding;
    [SerializeField]
    Transform holdingPosition;
    [SerializeField]
    Transform knifePosition;
    // The Ingredient the player is holding
    public IngredientObject ingredient { get; private set; }

    #region Monobehaviours

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        moveInput.Enable();
    }

    private void FixedUpdate()
    {
        if (moveInput.enabled)
        {
            Vector3 move = new Vector3(-moveInput.ReadValue<Vector2>().y, 0, moveInput.ReadValue<Vector2>().x);
            if (move.magnitude > 0.01f)
            {
                Vector3 targetForward = Vector3.RotateTowards(transform.forward, move, 6.238f * Time.fixedDeltaTime, 2);
                controller.Move(playerSpeed * Time.fixedDeltaTime * move);
                transform.forward = targetForward;
            }
            else
            {
                controller.Move(Vector3.zero);
            }
        }

    }

    public void ToggleMovement(bool enabled = true)
    {
        if (enabled) moveInput.Enable();
        else moveInput.Disable();
    }

    #endregion // Monobehaviours

    #region Input Actions

    public void PickUp(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            if (!holding)
            {
                CheckPickups();
            }
            if (carriedPlate != null)
            {
                CheckPass();
            }
            CheckPlate();
        }
    }

    public void Chop(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            if (holding)
            {
                if (choppingBoard.CanInteract(transform) && ingredient.state == choppingBoard.stateIn)
                {
                    choppingBoard.Interact(this);
                    knife.transform.SetParent(knifePosition);
                    knife.Lerp(knife.transform, knifePosition, 0.33f);
                    choppingBoard.OnProcessingComplete.AddListener(ReturnKnife);
                }
                else if (sink.CanInteract(transform) && ingredient.state == sink.stateIn)
                {
                    sink.Interact(this);
                }
            }
        }
    }

    private void ReturnKnife()
    {
        knife.transform.SetParent(knife.defaultPosition);
        knife.Lerp(knife.transform, knife.defaultPosition, 0.33f);
        choppingBoard.OnProcessingComplete.RemoveListener(ReturnKnife);
    }

    #endregion // Input Actions

    #region Interaction Validators

    private bool CheckPickups()
    {
        foreach (InteractionObject trough in pickUpZones)
        {
            if (trough.CanInteract(transform))
            {
                trough.Interact(this);                
                return true;
            }
        }
        ingredient = null;
        return false;
    }

    private bool CheckPlate()
    {
        // check for nearby plates to interact with. 
        if(plateStack.availablePlate.CanInteract(transform))
        {
            plateStack.availablePlate.Interact(this);
            return true;
        }
        return false;
    }

    private bool CheckPass()
    {
        if (carriedPlate != null)
        {
            carriedPlate.GetComponent<Collider>().enabled = false;

            if (thePass.CanInteract(transform))
            {
                holding = false;

                thePass.Interact(this);
                return true;
            }
            else
            {
                carriedPlate.GetComponent<Collider>().enabled = true;
            }
        }
        return false;
    }

    #endregion // Interaction Validators

    public void TakeIngredient(IngredientObject newIngredient)
    {
        ingredient = newIngredient;
        ingredient.transform.parent = holdingPosition;
        ingredient.Lerp(ingredient.transform, holdingPosition, 0.5f);
        holding = true;

        ToggleMovement();
    }

    /// <summary>
    /// Place the ingredient on a plate
    /// </summary>
    /// <param name="plate"></param>
    public void SetIngredient(Plate plate)
    {
        ingredient.transform.SetParent(plate.transform);
        ingredient.Lerp(ingredient.transform, plate.transform);
        ingredient = null;
        holding = false;

    }

    public void TakePlate(Plate plate)
    {
        carriedPlate = plate;
        plate.transform.SetParent(holdingPosition);
        plate.transform.localPosition = Vector3.zero;
        holding = true;

    }

}
