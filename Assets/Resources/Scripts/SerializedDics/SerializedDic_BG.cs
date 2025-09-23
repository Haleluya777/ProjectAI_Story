using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    [CreateAssetMenu(fileName = "SerializedDic_BG", menuName = "ScriptableObjects/SerializedDic_BG")]
    [System.Serializable]
    public class SerializedDic_BG : ScriptableObject
    {
        [SerializedDictionary("BGNum", "Sprite")]
        public SerializedDictionary<int, Sprite> bgMap;
    }
}