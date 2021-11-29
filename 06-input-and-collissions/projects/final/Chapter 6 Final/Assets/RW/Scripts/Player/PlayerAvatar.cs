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

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAvatar : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = 20f;
    public Animator animator;

    private CharacterController characterController;
    private Vector2 movementInput = Vector2.zero;
    private bool allowInput = true;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (movementInput != Vector2.zero && allowInput)
        {
            UpdateMovementAndRotation();
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            movementInput = Vector2.zero;
            animator.SetFloat("Speed", 0f);
        }

        UpdateGravity();
    }

    private void UpdateMovementAndRotation()
    {
        Vector3 movementVector = new Vector3(-movementInput.y, 0, movementInput.x);
        characterController.Move(movementVector * movementSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementVector), Time.deltaTime * rotationSpeed);
    }

    private void UpdateGravity()
    {
        characterController.Move(Vector3.down * gravity * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!allowInput)
        {
            return;
        }

        movementInput = context.ReadValue<Vector2>();
    }
}