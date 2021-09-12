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
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    // Normal text

    public GameObject dialogueWindow;
    public Text speakerText;
    public Text lineText;
    public GameObject continueIndicator;

    // Questions

    public GameObject answersParent;
    public Text firstOption;
    public Text secondOption;
    public RectTransform optionSelector;

    // Player input

    public PlayerAvatar playerAvatar;
    private PlayerInput playerInput;

    // Conversation

    private Conversation activeConversation;
    private int dialogueIndex;
    private DialogueStarter dialogueStarter;
    private bool firstOptionSelected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = false;
    }

    public void StartConversation(Conversation conversation, DialogueStarter dialogueStarter)
    {
        playerAvatar.DisableInput();
        activeConversation = conversation;
        this.dialogueStarter = dialogueStarter;
        dialogueIndex = 0;
        SetDialogueWindowVisibility(true);
        ShowLine();
        playerInput.enabled = true;
    }

    public void EndConversation()
    {
        playerInput.enabled = false;
        playerAvatar.EnableInput();
        activeConversation = null;
        SetDialogueWindowVisibility(false);
        dialogueStarter.OnConversationEnd();
    }

    private void ShowLine()
    {
        DialogueLine currentLine = activeConversation.dialogueLines[dialogueIndex];
        speakerText.text = currentLine.speaker;
        lineText.text = currentLine.text;

        if (!currentLine.thisIsAQuestion)
        {
            SwitchToNormalText();
        }
        else
        {
            SwitchToQuestion(currentLine);
        }
    }

    private void SwitchToNormalText()
    {
        answersParent.SetActive(false);
        optionSelector.gameObject.SetActive(false);
        continueIndicator.SetActive(true);
    }

    private void SwitchToQuestion(DialogueLine currentLine)
    {
        answersParent.SetActive(true);
        continueIndicator.SetActive(false);
        optionSelector.gameObject.SetActive(true);
        firstOption.text = currentLine.dialogueQuestion.firstOption;
        secondOption.text = currentLine.dialogueQuestion.secondOption;
        firstOptionSelected = true;
        UpdateOptionSelectorPostion();
    }

    private void SetDialogueWindowVisibility(bool visible)
    {
        dialogueWindow.SetActive(visible);
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!activeConversation || !context.performed)
        {
            return;
        }

        DialogueLine currentLine = activeConversation.dialogueLines[dialogueIndex];

        if (!currentLine.thisIsAQuestion)
        {
            NormalTextInteract(currentLine);
        }
        else
        {
            QuestionInteract(currentLine);
        }
    }

    private void NormalTextInteract(DialogueLine currentLine)
    {
        dialogueIndex++;

        if (activeConversation.dialogueLines.Length > dialogueIndex)
        {
            ShowLine();
        }
        else
        {
            if (currentLine.exitGameAfterConversation)
            {
                SceneManager.LoadScene("Title");
            }
            else
            {
                EndConversation();
            }
        }
    }

    private void QuestionInteract(DialogueLine currentLine)
    {
        if (firstOptionSelected)
        {
            // Enter first dialogue
            StartConversation(currentLine.dialogueQuestion.conversationWhenFirstOptionWasSelected, dialogueStarter);
        }
        else
        {
            // Enter second dialogue
            StartConversation(currentLine.dialogueQuestion.conversationWhenSecondOptionWasSelected, dialogueStarter);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (activeConversation != null && context.performed)
        {
            DialogueLine currentLine = activeConversation.dialogueLines[dialogueIndex];

            if (currentLine.thisIsAQuestion)
            {
                firstOptionSelected = !firstOptionSelected;
                UpdateOptionSelectorPostion();
            }
        }
    }

    private void UpdateOptionSelectorPostion()
    {
        if (firstOptionSelected)
        {
            optionSelector.position = new Vector3(optionSelector.position.x, firstOption.GetComponent<RectTransform>().position.y, optionSelector.position.z);
        }
        else
        {
            optionSelector.position = new Vector3(optionSelector.position.x, secondOption.GetComponent<RectTransform>().position.y, optionSelector.position.z);
        }
    }
}