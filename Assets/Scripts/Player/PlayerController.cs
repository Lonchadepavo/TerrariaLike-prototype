using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField]
    private float player_speed, jump_force;

    Rigidbody2D _pRigidbody;
    Animator _pAnimator;

    [SerializeField]
    Transform _groundPosition;

    bool is_grounded, is_jumping;

    public float block_place_speed;

    void Awake() {
        _pRigidbody = GetComponent<Rigidbody2D>();  
        _pAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Update() {
        PlayerJump();
    }

    private void FixedUpdate() {
        PlayerMovement();
        
    }

    void PlayerMovement() {
        float hor_axis = Input.GetAxisRaw("Horizontal");

        _pRigidbody.velocity = new Vector2(hor_axis * player_speed * Time.fixedDeltaTime, _pRigidbody.velocity.y);

        if (hor_axis != 0)
            transform.localScale = new Vector3(hor_axis, 1, 1);
    }

    void PlayerJump() {
        float hor_axis = Input.GetAxisRaw("Horizontal");

       _pAnimator.SetBool("is_walking",Convert.ToBoolean(hor_axis));
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (CheckIfGrounded()) { 
                _pRigidbody.AddForce(Vector2.up * jump_force);
            }
        }
    }

    bool CheckIfGrounded() {
        RaycastHit2D hit2D = Physics2D.Raycast(_groundPosition.position, Vector2.down * 0.15f);

        if (hit2D) {
            if (hit2D.transform.tag.Contains("ground")) {
                return true;
            }
        }

        return true;
    }
}
