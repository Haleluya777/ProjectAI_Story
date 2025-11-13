using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    [CreateAssetMenu(fileName = "SerializedDic_CutScene", menuName = "ScriptableObjects/SerializedDic_CutScene")]
    [System.Serializable]
    public class SerializedDic_CutScene : ScriptableObject
    {
        [SerializedDictionary("SceneNum", "Sprite")]
        public SerializedDictionary<int, Sprite> cutSceneMap;
    }
}
