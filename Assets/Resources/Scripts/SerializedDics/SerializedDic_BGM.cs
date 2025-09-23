using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    [CreateAssetMenu(fileName = "SerializedDic_BGM", menuName = "ScriptableObjects/SerializedDic_BGM")]
    [System.Serializable]
    public class SerializedDic_BGM : ScriptableObject
    {
        [SerializedDictionary("BGMNum", "AudioClip")]
        public SerializedDictionary<int, AudioClip> bgmMap;
    }
}
