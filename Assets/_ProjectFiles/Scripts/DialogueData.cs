using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
    public List<Dialogue> dialogues;
    public string Name;
    public string ID;
}

[System.Serializable]
public class Dialogue
{
    public CharacterData characterData;
    [TextArea]
    public string text;
    public bool extendPrevious;
}
