using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OreGenerationData {
    public string ore_name;
    public TilePrefab ore_prefab;
    [Range(0.0f,1.0f)]
    public float ore_rarity;
    public int max_height;
    public int min_height;
    public TilePrefab[] spawneable_tiles;
}
