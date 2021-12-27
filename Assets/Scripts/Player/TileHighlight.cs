using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileHighlight : MonoBehaviour {
    public Tilemap tilemap;

    void Update() {
        Vector3Int mouse_position = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector2 mouse_position_units = tilemap.CellToWorld(mouse_position);

        transform.position = new Vector2(mouse_position_units.x + GetComponent<SpriteRenderer>().bounds.size.x/2, mouse_position_units.y + GetComponent<SpriteRenderer>().bounds.size.y/2);
    }
}
