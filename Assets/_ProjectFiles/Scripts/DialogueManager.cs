using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public float textSpeed = 10;
    public GameObject dialoguePanel;
    //public Image rightCharacterImage;
    //public Image leftCharacterImage;
    public CanvasGroup rightCanvasGroup;
    public CanvasGroup leftCanvasGroup;
    [SerializeField] private SceneTransition sceneTransition;
    public TMP_Text textArea;
    //private CharacterData rightCharacter;
    //private CharacterData leftCharacter;
    //public TMP_Text rightCharacterNameText;
    //public TMP_Text leftCharacterNameText;

    public DialogueData data;
    private Dialogue currentDialogue;
    private int currentDialogueIndex;
    private bool continueDialog;
    private bool isDialogueRunning;

   void Awake()
    {
        //dialoguePanel.SetActive(false);
        StartDialogue(data);
    }

    public void OnPrimaryClick() {
        OnDialogueContinue();
    }

    public void StartDialogue(DialogueData data)
    {
        this.data = data;
        //leftCharacter = data.dialogues.Where(d => d.characterData.IsMainCharacter).FirstOrDefault().characterData;
        //rightCharacter = data.dialogues.Where(c => !c.characterData.IsMainCharacter).FirstOrDefault().characterData;
        foreach (var dialogue in data.dialogues)
        {
            /*if (dialogue.characterData.IsMainCharacter)
            {
                leftCharacter = dialogue.characterData;
            }
            if (!dialogue.characterData.IsMainCharacter)
            {
                rightCharacter = dialogue.characterData;
            }
            if (leftCharacter != null && rightCharacter != null)
            {
                break;
            }*/
        }
        /*if (leftCharacter.Sprite == null)
        {
            leftCharacterImage.gameObject.SetActive(false);
        }
        else
        {
            leftCharacterImage.sprite = leftCharacter.Sprite;
        }
        if (rightCharacter.Sprite == null)
        {
            rightCharacterImage.gameObject.SetActive(false);
        }
        else
        {
            rightCharacterImage.sprite = rightCharacter.Sprite;
        }
        leftCharacterNameText.text = leftCharacter.Name;
        rightCharacterNameText.text = rightCharacter.Name;*/
        currentDialogueIndex = 0;
        ShowDialogue(data.dialogues[currentDialogueIndex]);
        //InputEventSender.OnInputLeftClicked += OnDialogueContinue;
        dialoguePanel.SetActive(true);
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        /*if (dialogue.characterData == rightCharacter)
        {
            rightCanvasGroup.alpha = 1f;
            leftCanvasGroup.alpha = .5f;
        }
        else if (dialogue.characterData == leftCharacter)
        {
            rightCanvasGroup.alpha = .5f;
            leftCanvasGroup.alpha = 1f;
        }*/
        currentDialogue = dialogue;
        StartCoroutine(ShowTextCoroutine());
    }

    private IEnumerator ShowTextCoroutine()
    {
        isDialogueRunning = true;
        if (!currentDialogue.extendPrevious)
        {
            textArea.text = "";
        }
        else
        {
            textArea.text += " ";
        }

        string text = currentDialogue.text;
        float waitTime = 1 / textSpeed;
        foreach (var character in text)
        {
            textArea.text += character;
            if (!continueDialog)
            {
                yield return new WaitForSeconds(waitTime);
            }
            // Äänen soitto tähän!
        }
        continueDialog = false;
        isDialogueRunning = false;
        currentDialogueIndex++;
    }

    private void OnDialogueContinue()
    {
        if (isDialogueRunning)
        {
            continueDialog = true;
        }
        else
        {
            if (currentDialogueIndex >= data.dialogues.Count)
            {
                EndDialogue();
                return;
            }
            ShowDialogue(data.dialogues[currentDialogueIndex]);
        }
    }

    private void EndDialogue()
    {
        //InputEventSender.OnInputLeftClicked -= OnDialogueContinue;
        dialoguePanel.SetActive(false);
        sceneTransition.StartTransitionOut();
        //rightCharacter = null;
        //rightCharacter = null;
    }
}
