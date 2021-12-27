using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    [Header("Inventory settings")]    
    public int inventory_max_size;

    //INTERNAL SETTINGS
    [HideInInspector]
    public int selected_slot = 0;
    public List<ItemController> inventory = new List<ItemController>();
    bool showing_inventory = false;
    GameObject dragging_item;

    [Header("Inventory components")]
    public GameObject inventory_gameobject;
    Transform[] inventory_slots = new Transform[36];
    AudioSource _aSource;

    public AudioClip item_pickup_sound;

    public Sprite inv_slot_sprite;
    public Sprite inv_selected_slot_sprite;

    public GameObject carrying_item;
    public GameObject item_information_card;
    public GameObject item_template;

    void Awake() {
        _aSource = GetComponent<AudioSource>();  
        dragging_item = item_template;

        int slot_counter = 0;
        foreach(Transform slot in inventory_gameobject.transform) {
            inventory_slots[slot_counter] = slot;
            inventory.Add(item_template.GetComponent<ItemController>());
            slot_counter++;
        }

        UpdateSelectedSlot();
        ChangeInventoryStatus(showing_inventory);
        
    }

    void Update() {
        ChangeSelectedSlot();
        ItemAttract();
        ItemPickup();

        DropItem(); //Q
        OpenInventory(); //E
        DragInventoryItem(selected_slot); //F
    }

    void OpenInventory() {
        if (Input.GetKeyDown(KeyCode.E)) {
            showing_inventory = !showing_inventory;
            ChangeInventoryStatus(showing_inventory);  
        }
    }

    void ChangeInventoryStatus(bool status) {
        for (int i = 9; i < inventory_slots.Length; i++) {
            inventory_slots[i].gameObject.SetActive(status);
        } 
    }

    void ItemAttract() {
        Collider2D[] colliding_items = Physics2D.OverlapCircleAll(transform.position, 1.0f);

        foreach (Collider2D item_col in colliding_items) {
            if (item_col.CompareTag("ItemEntity")) {
                if (item_col.GetComponent<ItemController>().item_state == ItemController.ItemState.itemFree) {
                    item_col.GetComponent<ItemController>().item_state = ItemController.ItemState.itemAttracted;
                    item_col.GetComponent<Rigidbody2D>().isKinematic = true;
                    item_col.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    item_col.GetComponent<BoxCollider2D>().isTrigger = true;
                }
            }
        }
    }

    void ItemPickup() {
        Collider2D colliding_item = Physics2D.OverlapCircle(transform.position, 0.2f);

        if (colliding_item.CompareTag("ItemEntity")) {
            if (colliding_item.GetComponent<ItemController>().item_state != ItemController.ItemState.itemDragged) {
                int inventory_position = CheckMatchingItem(colliding_item.gameObject);

                if (inventory_position != -1) {
                    _aSource.clip = item_pickup_sound;
                    _aSource.Stop();
                    _aSource.Play();

                    if (inventory[inventory_position].item_quantity < colliding_item.GetComponent<ItemController>().item_stacksize) {
                        if (inventory[inventory_position].item_quantity == 0) {
                            inventory[inventory_position] = GetSavedItem(colliding_item.gameObject);
                            inventory[inventory_position].item_quantity = 0;
                        }
                        inventory[inventory_position].item_quantity += colliding_item.GetComponent<ItemController>().item_quantity;

                    } else {
                        inventory.Add(GetSavedItem(colliding_item.gameObject));
                    
                    }

                    Destroy(colliding_item.gameObject);
                    UpdateInventoryDisplay();
                    UpdateCarryingItemDisplay();
                }

            }

        }
    }

    int CheckMatchingItem(GameObject go) {
        int item_pos = 0;
        foreach (ItemController item in inventory) {
            if (item.item_quantity == 0) {
                return item_pos;
            }

            if (item.item_id == go.GetComponent<ItemController>().item_id) {
                if (item.item_quantity < item.item_stacksize) {
                    return item_pos;
                }
            }

            item_pos++;
        }

        return -1;
    }

    public void DragInventoryItem(int passed_slot) {    
        if (Input.GetKeyDown(KeyCode.F)) {
            if ((dragging_item.GetComponent<ItemController>().item_quantity >= inventory[passed_slot].item_stacksize || dragging_item.GetComponent<ItemController>().item_quantity == 0
                || dragging_item.GetComponent<ItemController>().item_quantity == dragging_item.GetComponent<ItemController>().item_stacksize)) {
                    SwapItems(passed_slot);

            } else {
                if (inventory[passed_slot].item_quantity < inventory[passed_slot].item_stacksize) {
                    int amount_to_transfer = dragging_item.GetComponent<ItemController>().item_quantity;
                    if (inventory[passed_slot].item_quantity + amount_to_transfer > inventory[passed_slot].item_stacksize) {
                        amount_to_transfer -= (inventory[passed_slot].item_stacksize - inventory[passed_slot].item_quantity); //TESTEAR
                    }

                    inventory[passed_slot].item_quantity += amount_to_transfer;
                    dragging_item.GetComponent<ItemController>().item_quantity -= amount_to_transfer;

                    if (dragging_item.GetComponent<ItemController>().item_quantity <= 0) {
                        Destroy(GameObject.Find("Dragged item"));
                        dragging_item = item_template;

                    }
                }    
            }

            UpdateInventoryDisplay();
            UpdateCarryingItemDisplay();
        }
    }

    void SwapItems(int passed_slot) {
        Destroy(GameObject.Find("Dragged item"));

        GameObject dragged_item = Instantiate(item_template, transform.position, Quaternion.identity);
        dragged_item.name = "Dragged item";
        dragged_item.GetComponent<ItemController>().UpdateItem(inventory.ElementAt(passed_slot));
        dragged_item.GetComponent<ItemController>().item_state = ItemController.ItemState.itemDragged;
        dragged_item.GetComponent<SpriteRenderer>().sprite = dragged_item.GetComponent<ItemController>().item_sprite;
        dragged_item.GetComponent<Rigidbody2D>().freezeRotation = true;

        dragging_item = dragged_item;
        inventory[passed_slot] = dragging_item.GetComponent<ItemController>();
    }

    void DropItem() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (dragging_item.GetComponent<ItemController>().item_quantity > 0) {
                GameObject dropped_item = Instantiate(dragging_item, dragging_item.transform.position, Quaternion.identity);

                dropped_item.name = "Item" + dropped_item.GetComponent<ItemController>().item_name;
                dropped_item.GetComponent<ItemController>().UpdateItem(dragging_item.GetComponent<ItemController>());
                dropped_item.AddComponent<BoxCollider2D>();
                dropped_item.GetComponent<ItemController>().item_state = ItemController.ItemState.itemFree;
                dropped_item.GetComponent<Rigidbody2D>().freezeRotation = false;

                Destroy(GameObject.Find("Dragged item"));
                dragging_item = item_template;
            }
        }
    }

    void ChangeSelectedSlot() {
        for (int i = 1; i < 10; i++) {
            if (Input.GetKeyDown(i.ToString())) {
                selected_slot = i-1;
                UpdateSelectedSlot();
                UpdateCarryingItemDisplay();
            }
        }
    }

    ItemController GetSavedItem(GameObject go) {
        ItemController go_controller = go.GetComponent<ItemController>();
        ItemController saved_item = new ItemController(); //ARREGLAR

        saved_item.item_id = go_controller.item_id;
        saved_item.item_type = go_controller.item_type;
        saved_item.item_name = go_controller.item_name;
        saved_item.item_description = go_controller.item_description;
        saved_item.item_durability = go_controller.item_durability;
        saved_item.item_strength = go_controller.item_strength;
        saved_item.item_quantity = go_controller.item_quantity;
        saved_item.item_stacksize = go_controller.item_stacksize;
        saved_item.item_sprite = go_controller.item_sprite;
        saved_item.dig_sound = go_controller.dig_sound;
        saved_item.break_sound = go_controller.break_sound;
        saved_item.place_sound = go_controller.place_sound;
        saved_item.item_state = go_controller.item_state;
        saved_item.Item_tile_prefab = go_controller.Item_tile_prefab;

        return saved_item;
    }

    #region Inventory graphical update
    public void UpdateInventoryDisplay() {
        for (int i = 0; i < inventory_max_size; i++) {
            ItemController item = inventory[i];
            
            if (item.item_quantity > 0) {
                inventory_slots[i].GetChild(0).gameObject.SetActive(true);

                inventory_slots[i].GetChild(0).GetChild(0).GetComponent<Image>().sprite = item.item_sprite;
                inventory_slots[i].GetChild(0).GetChild(1).GetComponent<Text>().text = item.item_quantity.ToString();
            } else {
                inventory_slots[i].GetChild(0).gameObject.SetActive(false);
                carrying_item.GetComponent<SpriteRenderer>().sprite = null;
            }
        }
    }

    public void UpdateCarryingItemDisplay() {
        Sprite temp_sprite = null;
        if (selected_slot < inventory.Count) {
            if (inventory[selected_slot].item_quantity > 0) {
                temp_sprite = inventory[selected_slot].item_sprite;
            }
        }

        carrying_item.GetComponent<SpriteRenderer>().sprite = temp_sprite;
    }

    public void UpdateSelectedSlot() {
        foreach (Transform slot in inventory_slots) {
            slot.GetComponent<Image>().sprite = inv_slot_sprite;
        }

        inventory_slots[selected_slot].GetComponent<Image>().sprite = inv_selected_slot_sprite;
    }
    #endregion
}
