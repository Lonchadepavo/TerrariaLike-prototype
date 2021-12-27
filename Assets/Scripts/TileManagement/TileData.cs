using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileData {
    int tile_id; 
    string tile_name;
    int tile_type; //FRONT OR BACK
    Vector2Int tile_coords;

    float tile_strength;
    float tile_hp;

    int tile_biome;
    TileBase tile_sprite;
    TilePrefab tile_prefab;

    float tile_propagation_strength;
    float tile_propagation_resistance;

    float tile_propagation_speed;
    float tile_propagation_cooldown = 0;


    List<GameObject> drop_list = new List<GameObject>();

    public TileData (int tile_x_coord, int tile_y_coord, TilePrefab tile_data) {
        Tile_coords = new Vector2Int(tile_x_coord, tile_y_coord);
        this.Tile_id = tile_data.tile_id;
        this.Tile_name = tile_data.tile_name;
        this.Tile_type = tile_data.tile_type;
        this.Tile_strength = tile_data.tile_strength;
        this.Tile_hp = tile_data.tile_hp;
        this.Tile_propagation_strength = tile_data.tile_propagation_strength;
        this.Tile_propagation_resistance = tile_data.tile_propagation_resistance;
        this.Tile_propagation_speed = tile_data.tile_propagation_speed;
        this.Drop_list = tile_data.drop_list;
        this.Tile_sprite = tile_data.tile_sprites[(int) Random.Range(0, tile_data.tile_sprites.Length)];
        this.Tile_prefab = tile_data;
    }

    public void UpdateTile(TilePrefab tile_data) {
        this.Tile_id = tile_data.tile_id;
        this.Tile_name = tile_data.tile_name;
        this.Tile_type = tile_data.tile_type;
        this.Tile_strength = tile_data.tile_strength;
        this.Tile_hp = tile_data.tile_hp;
        this.Tile_propagation_strength = tile_data.tile_propagation_strength;
        this.Tile_propagation_resistance = tile_data.tile_propagation_resistance;
        this.Tile_propagation_speed = tile_data.tile_propagation_speed;
        this.Drop_list = tile_data.drop_list;
        this.Tile_sprite = tile_data.tile_sprites[(int) Random.Range(0, tile_data.tile_sprites.Length)];
        this.Tile_prefab = tile_data;
    }

    public void BlockUpdate() {
        
    }

    public void DamageTile(float damage) {
        this.Tile_hp -= damage;
    }

    public int Tile_id { get => tile_id; set => tile_id = value; }
    public int Tile_type { get => tile_type; set => tile_type = value; }
    public Vector2Int Tile_coords { get => tile_coords; set => tile_coords = value; }
    public float Tile_strength { get => tile_strength; set => tile_strength = value; }
    public float Tile_hp { get => tile_hp; set => tile_hp = value; }
    public int Tile_biome { get => tile_biome; set => tile_biome = value; }
    public TileBase Tile_sprite { get => tile_sprite; set => tile_sprite = value; }
    public List<GameObject> Drop_list { get => drop_list; set => drop_list = value; }
    public string Tile_name { get => tile_name; set => tile_name = value; }
    public TilePrefab Tile_prefab { get => tile_prefab; set => tile_prefab = value; }
    public float Tile_propagation_speed { get => tile_propagation_speed; set => tile_propagation_speed = value; }
    public float Tile_propagation_strength { get => tile_propagation_strength; set => tile_propagation_strength = value; }
    public float Tile_propagation_resistance { get => tile_propagation_resistance; set => tile_propagation_resistance = value; }
    public float Tile_propagation_cooldown { get => tile_propagation_cooldown; set => tile_propagation_cooldown = value; }
}
