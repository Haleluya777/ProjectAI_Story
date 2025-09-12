using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public int id;
    public string characterName; //캐릭터 이름
    public Sprite characterSprite; //캐릭터 스프라이트
}

[CreateAssetMenu(fileName = "CharacterMap", menuName = "ScriptableObjects/Character/CharacterMap")]
public class CharacterMap : ScriptableObject
{
    public CharacterData[] characters; //캐릭터 데이터

    public CharacterData GetCharacter(int index)
    {
        if (index < 0 || index >= characters.Length)
        {
            Debug.Log($"유효하지 않은 캐릭터 인덱스. Index : {index}");
            return null;
        }
        return characters[index];
    }
}
