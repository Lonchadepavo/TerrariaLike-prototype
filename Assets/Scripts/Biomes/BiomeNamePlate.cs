using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BiomeNamePlate : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Biome")) {
            GameObject biome_name = GameObject.Find("BiomeName");
            biome_name.GetComponent<TextMeshProUGUI>().text = collision.GetComponent<BiomeInfo>().biome_name;
            biome_name.GetComponent<Animator>().SetBool("show_biome_name", true);
        }
    }
}
