using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public Sprite Sprite;
    public bool IsMainCharacter;
    public string Name;
    public string ID;
}
