using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePropagationScript : MonoBehaviour {
    TileManagement _tManagement;

    void Start() {
        _tManagement = GetComponent<TileManagement>();    
    }

    void Update() {
        TilePropagation();
    }

    void TilePropagation() {
        for (int i = _tManagement.min_mapload_values.y; i < _tManagement.max_mapload_values.y; i++) {
            for (int j = _tManagement.min_mapload_values.x; j < _tManagement.max_mapload_values.x; j++) {
                TileData td = _tManagement.front_tile_saves[i,j];

                if (td.Tile_propagation_strength > 0) {
                    if (td.Tile_propagation_cooldown >= td.Tile_propagation_speed) {
                        td.Tile_propagation_cooldown = 0;

                        for (int x = -1; x <= 1; x++) {
                            for (int y = 1; y >= -1; y--) {
                                if (_tManagement.front_tile_saves[i + y, j + x].Tile_propagation_resistance < td.Tile_propagation_strength) {

                                    if (Random.Range(0,100) >= 80) {
                                        _tManagement.front_tile_saves[i + y, j + x].UpdateTile(td.Tile_prefab);
                                    }
                                }
                            }
                        }
                    } else {
                        td.Tile_propagation_cooldown += Time.deltaTime;
                    }
                }

            }   
        }
    }

}
