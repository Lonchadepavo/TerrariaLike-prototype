using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotInfo : MonoBehaviour, IPointerClickHandler {
    public int slot_number;

     public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left)
            GameObject.Find("Player").GetComponent<InventoryManager>().DragInventoryItem(slot_number);
     }
}
