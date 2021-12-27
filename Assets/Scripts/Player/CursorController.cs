using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {
    public Texture2D cursor_texture;

    void Start() {
        //UpdateCursor(cursor_texture);
    }

    public void UpdateCursor(Texture2D texture) {
        Cursor.SetCursor(texture, new Vector2(texture.width/2, texture.height/2), CursorMode.ForceSoftware);        
    }
}
