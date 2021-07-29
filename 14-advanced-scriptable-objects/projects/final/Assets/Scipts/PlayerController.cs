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
    private InteractionObject choppingBoard;
    [SerializeField]
    private InteractionObject sink;

    // state for holding something or not
    private bool holding;

    #region Monobehaviours

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        moveInput.Enable();
    }

    private void FixedUpdate()
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
        animator.SetFloat("Speed", controller.velocity.magnitude / playerSpeed);
    }

    #endregion // Monobehaviours

    #region Input Actions

    public void PickUp(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            if (!holding)
            {
                if (CheckPickups())
                {
                    holding = true;
                    animator.SetBool("Holding", holding);
                }
            }
            else
            {
                Debug.Log("Player tried to put something down");
                holding = false;
                animator.SetBool("Holding", holding);
            }
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
                    Debug.LogFormat("Player is chopping!");
                    animator.SetTrigger("Chop");
                }
                else if (sink.CanInteract(transform))
                {
                    Debug.LogFormat("Player is washing!");
                    animator.SetTrigger("Wash");
                }
            }
        }
    }

    #endregion // Input Actions

    #region Interaction Validators

    private bool CheckPickups()
    {
        foreach (InteractionObject i in pickUpZones)
        {
            if (i.CanInteract(transform))
            {
                Debug.LogFormat("Player picked up from {0}", i.name);
                return true;
            }
        }
        return false;
    }

    #endregion // Interaction Validators

}
