using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BiomeData {
    public int biome_id;
    public string biome_name;
    public BiomeTypes biome_type;

    public int min_biome_appearences;
    public int max_biome_appearences;
    int total_biome_appearences;

    [Range(0.0f,1.0f)]
    public float biome_min_width;
    [Range(0.0f,1.0f)]
    public float biome_max_width;

    Vector2[] biome_bounds = new Vector2[2];

    public Sprite biome_background_image;

    public TilePrefab[] top_biome_tiles;
    public TilePrefab[] mid_biome_tiles;
    public TilePrefab[] bottom_biome_tiles;

    [HideInInspector]
    public GameObject biome_go;

    public Vector2[] Biome_bounds { get => biome_bounds; set => biome_bounds = value; }
    public int Total_biome_appearences { get => total_biome_appearences; set => total_biome_appearences = value; }
}
