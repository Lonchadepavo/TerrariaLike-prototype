using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInformationController : MonoBehaviour {
    public TextMeshProUGUI item_name;
    public TextMeshProUGUI item_description;

    public void UpdateText(ItemController item) {
        item_name.text = item.item_name;
        item_description.text = item.item_description;
    }

}
