using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BiomeTypesData {
    public BiomeTypes biome_type;

    [Range(0,100)]
    public int biome_type_height;

    public Vector2 biome_y_bounds;

    [HideInInspector]
    public GameObject biome_go;
}
