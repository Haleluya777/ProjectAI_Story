using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    [CreateAssetMenu(fileName = "SerializedDic_Anim", menuName = "ScriptableObjects/SerializedDic_Anim")]
    [System.Serializable]
    public class SerializedDic_Anim : ScriptableObject
    {
        [SerializedDictionary("EffectName", "AnimationClip")]
        public SerializedDictionary<string, AnimationClip> animationClips;
    }
}
