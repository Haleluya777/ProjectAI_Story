using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[System.Serializable]
public class CharacterData
{
    public int id;
    public string characterName; //캐릭터 이름
    //public Dictionary<string, Sprite> characterSprite = new Dictionary<string, Sprite>(); //캐릭터 스프라이트
    public SerializedDictionary characterSpriteMap; //캐릭터 스프라이트
    public int affaction; //캐릭터 호감도
    public int CurrentdialogueNum; //현재 대화 진행도
    public int dialogueLineNum; //현재 대화 진행도 내의 대사 진행도
    public List<TextAsset> dialogueFiles; //로비에서 캐릭터를 선택했을 때 하는 대화 파일들.
    public string CharacterFloor; //캐릭터가 있는 층 이름.
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
