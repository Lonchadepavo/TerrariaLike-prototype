using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ItemController : MonoBehaviour {
    static int current_id = 0;

    public int item_id;
    public int item_type; //0 - BACK BLOCKS, 1 - FRONT BLOCKS, 2 - PICKAXE, 3 - HAMMER, 4 - RESOURCES

    public string item_name;
    public string item_description;

    public float item_durability;
    public float item_strength;
    
    public int item_stacksize;
    public int item_quantity;

    public Sprite item_sprite;

    public ItemState item_state;

    float follow_speed = 2.0f;

    TilePrefab item_tile_prefab;

    GameObject _player;

    public AudioClip dig_sound;
    public AudioClip break_sound;
    public AudioClip place_sound;

    public TilePrefab Item_tile_prefab { get => item_tile_prefab; set => item_tile_prefab = value; }

    public enum ItemState {itemFree, itemDragged, itemAttracted};

    void Start() {
        item_sprite = GetComponent<SpriteRenderer>().sprite; 
        _player = GameObject.Find("Player");
    }

    void Update() {
        FollowPlayer();
        FollowMouse();
    }

    void FollowPlayer() {
        if (item_state == ItemState.itemAttracted) {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, follow_speed * Time.deltaTime);
       
            Vector3 player_distance = transform.position - _player.transform.position;
            follow_speed = player_distance.magnitude * 2.0f;
            follow_speed = Mathf.Abs(Mathf.Clamp(follow_speed, 3.0f, 100.0f));
        }
    }

    void FollowMouse() {
        if (item_state == ItemState.itemDragged) {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public bool OnBlockBreak(TileManagement tm_reference, Vector3Int tile_position, ItemController selected_item, GameObject player, int break_mode = 2) {
        TileData td = tm_reference.front_tile_saves[tile_position.y, tile_position.x];;
        bool can_drop = false;

        switch(break_mode) {
            case 2: //PICKAXE
                tm_reference.front_tilemap.SetTile(tile_position, null);
                break;

            case 3: //HAMMER
                td = tm_reference.back_tile_saves[tile_position.y, tile_position.x];
            break;
        }

        td.DamageTile((selected_item.item_strength/td.Tile_strength));

        if (td.Tile_id != 3) {
            if (td.Tile_hp <= 0) {
                can_drop = true;
            }

            player.GetComponent<AudioSource>().clip = dig_sound;
            player.GetComponent<AudioSource>().pitch = 1 + Random.Range(-0.5f,0.5f);
            player.GetComponent<AudioSource>().Play();
        }

        if (can_drop) {
            Vector3 item_position = tm_reference.front_tilemap.CellToWorld(tile_position);
            GameObject go_item = Instantiate(td.Drop_list[0], new Vector3(item_position.x + 0.3f, item_position.y + 0.4f, item_position.z), Quaternion.identity);
            go_item.GetComponent<ItemController>().item_tile_prefab = td.Tile_prefab;
            go_item.GetComponent<ItemController>().item_id = td.Tile_id;

            td.UpdateTile(tm_reference.generation_tiles[3].GetComponent<TilePrefab>());
            player.GetComponent<AudioSource>().clip = break_sound;
            player.GetComponent<AudioSource>().pitch = 1 + Random.Range(-0.5f,0.5f);
            player.GetComponent<AudioSource>().Play();
            return true;
        }

        return false;
    }

    public void UpdateItem(ItemController item) {
        item_id = item.item_id;
        item_type = item.item_type;
        item_name = item.item_name;
        item_description = item.item_description;
        item_durability = item.item_durability;
        item_strength = item.item_strength;
        item_quantity = item.item_quantity;
        item_stacksize = item.item_stacksize;
        item_sprite = item.item_sprite;
        dig_sound = item.dig_sound;
        break_sound = item.break_sound;
        place_sound = item.place_sound;
        item_state = item.item_state;
        Item_tile_prefab = item.Item_tile_prefab;

    }

    int GenerateItemId() {
        current_id++;
        return current_id - 1;
    }
}
