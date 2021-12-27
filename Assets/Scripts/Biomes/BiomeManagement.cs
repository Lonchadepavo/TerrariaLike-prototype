using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BiomeManagement : MonoBehaviour {

    [Header("Biome types")]
    public BiomeTypesData[] biome_types_list;

    [Header("Biomes")]
    public BiomeData default_biome;
    public BiomeData[] biome_list;
    [HideInInspector]
    public List<GameObject> biome_tilemaps = new List<GameObject>();

    int grid_width, grid_height;
    Tilemap front_tilemap;

    void Awake() {
        grid_width = GetComponent<TileManagement>().grid_width;
        grid_height = GetComponent<TileManagement>().grid_height;

        front_tilemap = GetComponent<TileManagement>().front_tilemap;        
    }

    public void BiomeTypesSetup() {
        int height = 0;

        for (int i = 0; i < biome_types_list.Length; i++) {
            GameObject biome = new GameObject();
            biome.tag = "BiomeType";

            biome.AddComponent<BiomeInfo>();
            biome.AddComponent<BoxCollider2D>();
            biome.GetComponent<BoxCollider2D>().isTrigger = true;

            biome.name = biome_types_list[i].biome_type.ToString();

            int biome_height = (int) (grid_height*((float) (biome_types_list[i].biome_type_height/100.0f)));
            Vector3 bounds = front_tilemap.CellToWorld(new Vector3Int(grid_width, biome_height, 0));

            biome.transform.localScale = bounds;
            biome.transform.position = new Vector2(bounds.x/2, (front_tilemap.CellToWorld(new Vector3Int(grid_width, grid_height, 0)).y - bounds.y/2) - height);
            biome_types_list[i].biome_go = biome;


            height += (int) bounds.y;
        }

    }

    public void BiomeSetup() {
        int[] biome_widths_in_units = new int[] {0,0,0};
        int[] biome_widths_in_cells = new int[] {0,0,0};

        int number_of_biomes = 0;

        for (int i = 0; i < biome_list.Length; i++) {
            int appearences = (int) UnityEngine.Random.Range(biome_list[i].min_biome_appearences, biome_list[i].max_biome_appearences);
            biome_list[i].Total_biome_appearences = appearences;
            number_of_biomes += appearences;
        }
        
        while (number_of_biomes > 0) {
            for (int i = 0; i < biome_list.Length; i++) {
                GameObject biome = new GameObject();
                BiomeData biome_data = biome_list[i];

                if (biome_data.Total_biome_appearences > 0) {
                    biome.tag = "Biome";

                    GameObject biome_parent = biome_types_list[(int) biome_data.biome_type].biome_go;

                    biome.AddComponent<BiomeInfo>();
                    biome.AddComponent<BoxCollider2D>();
                    biome.GetComponent<BoxCollider2D>().isTrigger = true;

                    biome.name = biome_data.biome_name;
            
                    float random_width = UnityEngine.Random.Range(biome_data.biome_min_width, biome_data.biome_max_width);
 
                    int current_biome_width_in_cells = (int) (random_width * grid_width);
                    float current_biome_width_in_units = front_tilemap.CellToWorld(new Vector3Int(current_biome_width_in_cells, 0,0)).x;

                    //AJUSTAR EL WORLD_WIDTH_SCALE PARA QUE NO SE SALGA DEL LÍMITE
                    if (biome_widths_in_units[(int)biome_data.biome_type] + current_biome_width_in_units > front_tilemap.CellToWorld(new Vector3Int(grid_width, 0, 0)).x) {
                        current_biome_width_in_units = front_tilemap.CellToWorld(new Vector3Int(grid_width, 0, 0)).x - biome_widths_in_units[(int)biome_data.biome_type];
                    }

                    biome.transform.localScale = new Vector2(current_biome_width_in_units, biome_parent.transform.localScale.y);
                    biome.transform.position = new Vector2(biome_widths_in_units[(int)biome_data.biome_type] + biome.transform.localScale.x/2, biome_parent.transform.position.y);

                    biome.GetComponent<BiomeInfo>().biome_name = biome.name;
                    biome.GetComponent<BiomeInfo>().biome_width = current_biome_width_in_cells;
            
                    biome_data.Biome_bounds[0] = new Vector2(biome_widths_in_cells[(int) biome_data.biome_type], biome_types_list[(int) biome_data.biome_type].biome_y_bounds.x); //CAMBIAR LAS COORDENADAS Y MÁS ADELANTE
                    biome_data.Biome_bounds[1] = new Vector2(biome_widths_in_cells[(int) biome_data.biome_type] + current_biome_width_in_cells - 1, biome_types_list[(int) biome_data.biome_type].biome_y_bounds.y);

                    biome_widths_in_units[(int)biome_data.biome_type] += (int) current_biome_width_in_units;
                    biome_widths_in_cells[(int) biome_data.biome_type] += current_biome_width_in_cells;

                    biome.transform.parent = biome_parent.transform;
                    biome_data.Total_biome_appearences--;
                    number_of_biomes--;
                }
            }
        }
    }

    GameObject GenerateNewTilemap(string tilemap_type, string tilemap_name) {
        GameObject tilemap = new GameObject();
        tilemap.AddComponent<Tilemap>();
        tilemap.AddComponent<TilemapRenderer>();
        tilemap.AddComponent<TilemapCollider2D>();
        tilemap.AddComponent<Rigidbody2D>();
        tilemap.AddComponent<CompositeCollider2D>();
            
        tilemap.name = "Biome_" + tilemap_type + "_" + tilemap_name;
        tilemap.GetComponent<TilemapRenderer>().sortingLayerName = tilemap_type;
        tilemap.transform.parent = GameObject.Find("Grid").transform;

        biome_tilemaps.Add(tilemap);

        return tilemap;
    }

}
