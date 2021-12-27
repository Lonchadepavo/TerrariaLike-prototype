using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSaver : MonoBehaviour {
    public static void SaveTiles (ref TileData[,] save_tiles, TileData td, TilePrefab tile_data, int begin_y, int max_height, int grid_height) {

        if (max_height == grid_height)
            max_height -= 1;

        for (int i = begin_y; i <= max_height; i++) {
            TileData tile = new TileData(td.Tile_coords.x, i, tile_data);
            save_tiles[i, td.Tile_coords.x] = tile;
        }
    }
}
