using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialogues
{
    public List<TextAsset> assets = new List<TextAsset>();
}

[CreateAssetMenu(fileName = "FixedDialoguesMap", menuName = "ScriptableObjects/Dialogues/FixedDialoguesMap")]
public class FixedDialoguesMap : ScriptableObject
{
    public Dialogues[] dialogues;

    public TextAsset GetDialogues(int Day, int Routine)
    {
        return dialogues[Day].assets[Routine];
    }
}
