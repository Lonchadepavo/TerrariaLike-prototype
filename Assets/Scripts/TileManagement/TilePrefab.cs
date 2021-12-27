using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePrefab : MonoBehaviour {
    [Header("Tile general settings")]
    public int tile_id;
    public string tile_name;

    [Header("Tile health settings")]
    public float tile_strength;
    public float tile_hp;

    [Header("Tile propagation settings")]
    public float tile_propagation_strength;
    public float tile_propagation_resistance;
    public float tile_propagation_speed;

    public TileBase[] tile_sprites; //TILE SPRITE
    public int tile_type; //FRONT OR BACK

    [Header("Tile drops")]
    public List<GameObject> drop_list = new List<GameObject>();
}
