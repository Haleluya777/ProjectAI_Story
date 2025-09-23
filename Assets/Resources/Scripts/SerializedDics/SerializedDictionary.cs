using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AYellowpaper.SerializedCollections
{
    [CreateAssetMenu(fileName = "SerializedDictionary", menuName = "ScriptableObjects/SerializedDictionary")]
    [System.Serializable]
    public class SerializedDictionary : ScriptableObject
    {
        [SerializedDictionary("Emotion", "Sprite")]
        public SerializedDictionary<string, Sprite> sprites;
    }
}

