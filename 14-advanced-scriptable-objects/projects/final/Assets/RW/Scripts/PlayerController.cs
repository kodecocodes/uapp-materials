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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    [SerializeField]
    private float playerSpeed = 2.0f;
    public InputAction moveInput;

    [SerializeField]
    private List<InteractionObject> pickUpZones;
    [SerializeField]
    private Workstation choppingBoard;
    [SerializeField]
    private Workstation sink;
    [SerializeField]
    private PlateStack plateStack;

    // state for holding something or not
    [SerializeField]
    private bool holding;
    [SerializeField]
    Transform holdingPosition;

    private Vector3 knifePosition = new Vector3(0, 0.577f, 1.61f);
    // The Ingredient the player is holding
    public IngredientObject ingredient { get; private set; }

    #region Monobehaviours

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
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
        animator.SetFloat("Speed", controller.velocity.magnitude / playerSpeed);
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
            CheckPlate();
        }
    }

    public void Chop(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            if (holding)
            {
                if (choppingBoard.CanInteract(transform))
                {
                    choppingBoard.Interact(this);
                }
                else if (sink.CanInteract(transform))
                {
                    sink.Interact(this);
                }
            }
        }
    }

    #endregion // Input Actions

    public void SetAnimationTrigger(string name)
    {
        animator.SetTrigger(name);
    }

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

    #endregion // Interaction Validators

    public void TakeIngredient(IngredientObject newIngredient)
    {
        ingredient = newIngredient;
        ingredient.transform.parent = holdingPosition;
        ingredient.Lerp(ingredient.transform, holdingPosition, 0.5f);
        holding = true;
        animator.SetBool("Holding", holding);
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
        animator.SetBool("Holding", holding);
    }

    public void TakePlate(Plate plate)
    {
        plate.transform.SetParent(holdingPosition);
        plate.transform.localPosition = Vector3.zero;
        holding = true;
        animator.SetBool("Holding", holding);
    }

}
