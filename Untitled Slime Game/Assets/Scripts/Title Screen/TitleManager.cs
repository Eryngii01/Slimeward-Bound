using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour {
    [SerializeField]
    private Texture2D _cursorSprite;

    // Start is called before the first frame update
    void Start() {
        Cursor.SetCursor(_cursorSprite, Vector2.zero, CursorMode.Auto);
    }
}
