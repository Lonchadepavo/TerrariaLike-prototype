using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerraformingScripts : MonoBehaviour {
    Tilemap front_tilemap, back_tilemap;
    TileManagement _TmReference;
    InventoryManager _iManager;  

    float tool_speed = 0.2f;
    public int tile_interaction_radius;

    [HideInInspector]
    public float breaking_delay = 0;

    public AudioClip block_place_sound;

    private void Awake() {
        _TmReference = GameObject.Find("GameController").GetComponent<TileManagement>();
        _iManager = GetComponent<InventoryManager>();

        front_tilemap = _TmReference.front_tilemap;
        back_tilemap = _TmReference.back_tilemap;
    }

    private void Update() {
        Vector3Int mouse_position = front_tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3Int player_position = front_tilemap.WorldToCell(transform.position);

        if (Vector3Int.Distance(mouse_position, player_position) <= tile_interaction_radius) {
            OnTileDestroy(mouse_position);
            OnTilePlace(mouse_position, player_position);
        }
    }

    void OnTileDestroy(Vector3Int cell_pos) {
        if (Input.GetMouseButton(0)) {
            if (cell_pos.x >= 0 && cell_pos.x <= _TmReference.grid_width && cell_pos.y >= 0 && cell_pos.y <= _TmReference.grid_height) {
                if (_iManager.selected_slot < _iManager.inventory.Count) { 
                    ItemController selected_item = _iManager.inventory[_iManager.selected_slot];
                    if (selected_item.item_type > 1) {
                        if (breaking_delay <= 0) {
                            breaking_delay = tool_speed;
                            if (selected_item.OnBlockBreak(_TmReference, cell_pos, selected_item, gameObject, selected_item.item_type)) {
                                breaking_delay = 0;
                            }
                        } else {
                            breaking_delay -= Time.deltaTime;  
                        }
                    }
                }
            }
        } else {
            breaking_delay = 0;
        }
    }

    bool OnTilePlace(Vector3Int cell_pos, Vector3Int player_pos) {
        if (Input.GetMouseButton(0)) {
            if (_iManager.selected_slot < _iManager.inventory.Count) {
                ItemController selected_item = _iManager.inventory[_iManager.selected_slot];

                if (selected_item.item_type <= 1) { //CHECKS IF ITEM CAN BE PLACED
                    Tilemap temp_tilemap = front_tilemap;
                    TileData[,] temp_tiledata = _TmReference.front_tile_saves;

                    if (selected_item.item_type == 0) {
                        temp_tilemap = back_tilemap;
                        temp_tiledata = _TmReference.back_tile_saves;
                    }

                    if (selected_item.item_type == 1) {
                        if (!CheckFreeSpots(cell_pos, player_pos)) {
                           return false; 
                        }
                    }

                    if (cell_pos.x >= 0 && cell_pos.x <= _TmReference.grid_width && cell_pos.y >= 0 && cell_pos.y <= _TmReference.grid_height) {
                        if (selected_item.item_quantity > 0) {
                            TileData td = temp_tiledata[cell_pos.y, cell_pos.x];
                            if (td.Tile_name.ToLower().Equals("air_tile")) {
                                if (CheckSurroundingBlocks(cell_pos, selected_item.item_type)) {
                                    td.UpdateTile(selected_item.Item_tile_prefab);

                                    selected_item.item_quantity--;
                                    _iManager.UpdateInventoryDisplay();

                                    GetComponent<AudioSource>().clip = selected_item.place_sound;
                                    GetComponent<AudioSource>().pitch = 1 + Random.Range(-0.5f, 0.5f);
                                    GetComponent<AudioSource>().Play();
                                }
                            }
                        }
                    }
                }
            }
        }

        return true;
    }

    bool CheckSurroundingBlocks(Vector3Int tile_position, int item_type) {
        if (item_type == 1) {
            if (_TmReference.back_tile_saves[tile_position.y, tile_position.x].Tile_id != 3) {
                return true;
            }

            for (int x = -1; x <= 1; x++) {
                for (int y = 1; y >= -1; y--) {
                    if (_TmReference.front_tile_saves[tile_position.y + y, tile_position.x + x].Tile_id != 3) {
                        return true;
                    }
                }
            }
        } else {
            for (int x = -1; x <= 1; x++) {
                for (int y = 1; y >= -1; y--) {
                    if (_TmReference.back_tile_saves[tile_position.y + y, tile_position.x + x].Tile_id != 3) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    bool CheckFreeSpots(Vector3Int mouse_position, Vector3Int player_position) {
        if (mouse_position != player_position) {
            if (mouse_position != new Vector3Int(player_position.x, player_position.y +1, player_position.z)) {
                return true;
            }
        }
        
        return false;
    }


}
