using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Narration", menuName = "Narrations")]
public class NarrationBase : ScriptableObject 
{

    [System.Serializable]
    public class Text
    {
        [TextArea(4, 8)]
        public string text;
        public Sprite illustration;
    }

    [Header("Insert narration text and pictures below")]
    public Text[] narrationInfo;
}
