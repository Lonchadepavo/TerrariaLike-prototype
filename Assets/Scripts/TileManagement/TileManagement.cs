using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TileManagement : MonoBehaviour {

    //TILE IDS: 0 - GRASS, 1 - DIRT, 2 - STONE, 3 - AIR

    public Grid tile_grid;
    public Tilemap front_tilemap, back_tilemap;

    public GameObject[] generation_tiles = new GameObject[1];

    //TERRAIN GENERATION VALUES
    [Header("Terrain generation values")]
    public int grid_width;
    public int grid_height;
    public float terrain_scale;
    public float terrain_seed;
    public bool dynamic_map_load;

    //TERRAIN HEIGHT VALUES
    [Header("Terrain heights")]
    public float terrain_height;
    public float rock_height;

    //HEIGHT MULTIPLIER VALUES
    [Header("Height multipliers")]
    public int terrain_height_multiplier;
    public int rock_height_multiplier;

    //TERRAIN SMOOTHNESS VALUES
    [Header("Terrain smoothness")]
    public float terrain_grass_smoothness;
    public float terrain_rock_smoothness;

    //TERRAIN PATCHES
    [Header("Terrain patches")]
    public float terrain_patches_widthscale;
    public float terrain_patches_heightscale;
    [Range(0.0f,1.0f)]
    public float stone_density;

    //CAVE GENERATION
    [Header("Cave generation (PHASE 1)")]
    public float caves_ph1_widthscale;
    public float caves_ph1_heightscale;
    [Range(0.0f,1.0f)]
    public float cave_ph1_growth;

    [Header("Cave generation (PHASE 2)")]
    public float caves_ph2_widthscale;
    public float caves_ph2_heightscale;
    [Range(0.0f,1.0f)]
    public float cave_ph2_growth;

    [Header("Cave generation (PHASE 3)")]
    public float caves_ph3_widthscale;
    public float caves_ph3_heightscale;
    [Range(0.0f,1.0f)]
    public float cave_ph3_growth;

    [Header("Ore generation")]
    public OreGenerationData[] ore_generation_data;

    [HideInInspector]
    public TileData[,] front_tile_saves;
    public TileData[,] back_tile_saves;

    //COMPONENTS
    [Header("Components")]
    public Transform _pTransform;
    BiomeManagement _bmReference;

    float seed_randomizer;

    [HideInInspector]
    public Vector2Int min_mapload_values;
    public Vector2Int max_mapload_values;

    void Start() {
        front_tile_saves = new TileData[grid_height, grid_width];
        back_tile_saves = new TileData[grid_height, grid_width];

        terrain_seed = Random.Range(0f, 99999f);
        seed_randomizer = Random.Range(0f,99999f);

        _bmReference = GetComponent<BiomeManagement>();

        _bmReference.BiomeTypesSetup();
        _bmReference.BiomeSetup();
        TerrainGenerator();
    }

    void Update() {
        if (dynamic_map_load)
            DynamicLoadMap();
    }

    void TerrainGenerator() {
        front_tilemap.ClearAllTiles();

        for (int x_grid = 0; x_grid < grid_width; x_grid++) {
            float xCoord = (float) x_grid / grid_width * terrain_scale;

            int perlin_grass_height = GetPerlinHeight((xCoord + terrain_seed), 0f,
            terrain_grass_smoothness, terrain_height, terrain_height_multiplier);

            int perlin_rock_height = GetPerlinHeight((xCoord + terrain_seed + 100.0f), 0f,
            terrain_rock_smoothness, rock_height, rock_height_multiplier);

            //AIR TILES
            TileData air_tile = new TileData(x_grid, grid_height, generation_tiles[3].GetComponent<TilePrefab>());

            MapSaver.SaveTiles(ref front_tile_saves, air_tile, generation_tiles[air_tile.Tile_id].GetComponent<TilePrefab>(), 0, grid_height, grid_height);
            MapSaver.SaveTiles(ref back_tile_saves, air_tile, generation_tiles[air_tile.Tile_id].GetComponent<TilePrefab>(), 0, grid_height, grid_height);

            //TERRAIN PATCHES (STONE AND DIRT PATCHES) (FALTA MEJORAR EL SISTEMA PARA AÑADIR GRAVILLA Y QUE EL TERRENO QUEDE MEJOR)
            for (int y_grid = perlin_grass_height; y_grid >= 0; y_grid--) {
                float patches_xCoord = (float) x_grid / grid_width * terrain_patches_widthscale;
                float patches_yCoord = (float) y_grid / perlin_rock_height * terrain_patches_heightscale;

                float perlin_patch = Mathf.PerlinNoise(patches_xCoord + terrain_seed, patches_yCoord + terrain_seed);

                TileData front_patch_tile;
                TileData back_patch_tile;
                BiomeData bmData = _bmReference.default_biome;

                if (_bmReference.biome_list.Length > 0) {
                    for (int i = 0; i < _bmReference.biome_list.Length; i++) {
                        BiomeData current_biome = _bmReference.biome_list[i];
                        if (CheckIfInBounds(new Vector2(x_grid, y_grid), current_biome.Biome_bounds[0], current_biome.Biome_bounds[1])) {
                            bmData = current_biome;
                            break;
                        }
                    }
                }

                if (y_grid == perlin_grass_height) { //TOP LAYER
                    front_patch_tile = new TileData(x_grid, y_grid, bmData.top_biome_tiles[0]);
                    back_patch_tile = new TileData(x_grid, y_grid, bmData.top_biome_tiles[1]);
                 
                } else if (y_grid > perlin_rock_height){ //MID LAYER
                    front_patch_tile = new TileData(x_grid, y_grid, bmData.mid_biome_tiles[0]);
                    back_patch_tile = new TileData(x_grid, y_grid, bmData.mid_biome_tiles[1]);
                   
                } else {
                    if (perlin_patch <= stone_density) { //BOTTOM LAYER TILES
                        front_patch_tile = new TileData(x_grid, y_grid, bmData.bottom_biome_tiles[0]);
                        back_patch_tile = new TileData(x_grid, y_grid, bmData.bottom_biome_tiles[1]);

                    } else { //MID LAYER TILES
                        front_patch_tile = new TileData(x_grid, y_grid, bmData.mid_biome_tiles[0]);
                        back_patch_tile = new TileData(x_grid, y_grid, bmData.mid_biome_tiles[1]);

                    }
                }
                front_tile_saves[y_grid, x_grid] = front_patch_tile;
                back_tile_saves[y_grid, x_grid] = back_patch_tile;   
            }

            //CAVES GEN (PHASE 1)
            CaveGeneration(x_grid, caves_ph1_widthscale, caves_ph1_heightscale, cave_ph1_growth, terrain_seed, perlin_rock_height, 0);

            //CAVES GEN (PHASE 2)
            CaveGeneration(x_grid, caves_ph2_widthscale, caves_ph2_heightscale, cave_ph2_growth, seed_randomizer, perlin_grass_height, 0);

            //CAVES GEN (PHASE 3)
            CaveGeneration(x_grid, caves_ph3_widthscale, caves_ph3_heightscale, cave_ph3_growth, seed_randomizer, perlin_rock_height, 0);
        }

        //ORE GENERATION
        for (int i = 0; i < ore_generation_data.Length; i++) {
            OreGenerationData current_ore = ore_generation_data[i];
            OreGeneration(300, 10, current_ore);
        }

        if (!dynamic_map_load)
            FillTerrain();
    }

    void CaveGeneration(int x_grid, float x_scale, float y_scale, float cave_growth, float seed, int max_height, int min_height) {
        for (int y_grid = max_height; y_grid >= min_height; y_grid--) {
            float cave_xCoord = (float) x_grid / grid_width * x_scale;
            float cave_yCoord = (float) y_grid / max_height * y_scale;

                
            float perlin_cave = Mathf.PerlinNoise(cave_xCoord + (seed), cave_yCoord + (seed));

            if (perlin_cave <= cave_growth) {
                TileData front_patch_tile = new TileData(x_grid, y_grid, generation_tiles[3].GetComponent<TilePrefab>());

                front_tile_saves[y_grid, x_grid] = front_patch_tile;
            }               
        }
    }

    void OreGeneration(float x_scale, float y_scale, OreGenerationData ore) {
        float seed = Random.Range(0f, 99999f);

        for (int x_grid = 0; x_grid < grid_width; x_grid++) {
            for (int y_grid = ore.max_height; y_grid >= ore.min_height; y_grid--) {
                float cave_xCoord = (float) x_grid / grid_width * x_scale;
                float cave_yCoord = (float) y_grid / ore.max_height * y_scale;

                
                float perlin_ore = Mathf.PerlinNoise(cave_xCoord + (seed), cave_yCoord + (seed));

                if (perlin_ore <= ore.ore_rarity) {
                    for (int spawneable_tiles = 0; spawneable_tiles < ore.spawneable_tiles.Length; spawneable_tiles++) {
                        if (front_tile_saves[y_grid, x_grid].Tile_id == ore.spawneable_tiles[spawneable_tiles].tile_id) {
                            int ore_variant = Random.Range(0,2);
                            TileData front_patch_tile = new TileData(x_grid, y_grid, ore.ore_prefab);

                            front_tile_saves[y_grid, x_grid] = front_patch_tile;
                            break;
                        }
                    }
                }               
            }
        }
    }

    void FillTerrain() {
        for (int i = 0; i < grid_height; i++) {
            for (int j = 0; j < grid_width; j++) {
                TileData front_td = front_tile_saves[i,j];
                TileData back_td = back_tile_saves[i,j];
            
                front_tilemap.SetTile(new Vector3Int(front_td.Tile_coords.x, front_td.Tile_coords.y, 0), front_td.Tile_sprite);
                back_tilemap.SetTile(new Vector3Int(back_td.Tile_coords.x, back_td.Tile_coords.y, 0), back_td.Tile_sprite);
            }
        }
    }

    void DynamicLoadMap() {
        Vector3Int player_pos = front_tilemap.WorldToCell(_pTransform.position);

        min_mapload_values = new Vector2Int(player_pos.x - 14, player_pos.y - 9);
        max_mapload_values = new Vector2Int(player_pos.x + 14, player_pos.y + 9);

        min_mapload_values.x = Mathf.Clamp(min_mapload_values.x, 0, grid_width);
        max_mapload_values.x = Mathf.Clamp(max_mapload_values.x, 0, grid_width);

        min_mapload_values.y = Mathf.Clamp(min_mapload_values.y, 0, grid_height);
        max_mapload_values.y = Mathf.Clamp(max_mapload_values.y, 0, grid_height);

        front_tilemap.ClearAllTiles();
        back_tilemap.ClearAllTiles();

        for (int i = min_mapload_values.y; i < max_mapload_values.y; i++) {
            for (int j = min_mapload_values.x; j < max_mapload_values.x; j++) {
                TileData front_td = front_tile_saves[i,j];
                TileData back_td = back_tile_saves[i,j];

                if (front_td != null) {
                    if (front_tilemap.GetTile(new Vector3Int(front_td.Tile_coords.x, front_td.Tile_coords.y, 0)) == null) {
                        front_tilemap.SetTile(new Vector3Int(front_td.Tile_coords.x, front_td.Tile_coords.y, 0), front_td.Tile_sprite);
                        if (front_td.Tile_id == 3) {
                            back_tilemap.SetTile(new Vector3Int(back_td.Tile_coords.x, back_td.Tile_coords.y, 0), back_td.Tile_sprite);
                        }
                    }
                }
            }
        }
    }

    int GetPerlinHeight(float x_coord, float y_coord, float smoothness, float height_value, float height_multiplier) {
        return (int) ((Mathf.PerlinNoise(x_coord / smoothness, y_coord) + height_value) * height_multiplier);
    }

    bool CheckIfInBounds(Vector2 cell, Vector2 min_bound, Vector2 max_bound) {
        if (cell.x >= min_bound.x && cell.y >= min_bound.y) {
            if (cell.x <= max_bound.x && cell.y <= max_bound.y) {
                return true;
            }
        }
        return false;
    }
}
