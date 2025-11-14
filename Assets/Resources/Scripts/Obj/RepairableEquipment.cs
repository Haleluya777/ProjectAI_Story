using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentDatas
{
    public string name;
    public string detail;
    public int progress;
    public Sprite sprite;
    public int reactedCharacterID;
}

[CreateAssetMenu(fileName = "EquipmentMap", menuName = "ScriptableObjects / Equipment / EquipmentMap")]
public class RepairableEquipment : ScriptableObject
{
    public EquipmentDatas[] datas;
    public EquipmentDatas GetEquipment(int index)
    {
        if (index < 0 || index >= datas.Length)
        {
            Debug.Log($"유효하지 않은 장비 인덱스. Index : {index}");
            return null;
        }
        return datas[index];
    }
}
