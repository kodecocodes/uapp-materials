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
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    // Interaction sphere

    public LayerMask interactionMask;
    public float interactionSphereForwardPosition;
    public Vector3 interactionSphereOffset;
    public float interactionSphereSize;

    //UI

    public GameObject interactionWindow;
    public Text interactionText;

    private PlayerAvatar playerAvatar;
    private InteractableObject interactionTarget = null;

    private void Awake()
    {
        playerAvatar = GetComponent<PlayerAvatar>();
    }

    private void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerAvatar.transform.position + playerAvatar.transform.forward * interactionSphereForwardPosition + interactionSphereOffset, interactionSphereSize, interactionMask);
        GameObject closest = null;

        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.GetComponent<InteractableObject>().canBeInteractedWith)
            {
                continue;
            }

            if (closest == null || Vector3.Distance(hitCollider.gameObject.transform.position, playerAvatar.transform.position) < Vector3.Distance(closest.transform.position, playerAvatar.transform.position))
            {
                closest = hitCollider.gameObject;
            }
        }

        if (closest != null)
        {
            interactionTarget = closest.GetComponent<InteractableObject>();
            if (!interactionWindow.activeSelf)
            {
                interactionWindow.SetActive(true);
            }
            interactionText.text = interactionTarget.interactionVerb + " " + interactionTarget.interactionName;
        }
        else
        {
            interactionTarget = null;
            if (interactionWindow.activeSelf)
            {
                interactionWindow.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward * interactionSphereForwardPosition + interactionSphereOffset, interactionSphereSize);
    }

    public void AttemptInteraction()
    {
        if (interactionTarget != null)
        {
            interactionTarget.Interact(playerAvatar);
        }
    }
}