using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 移动控制 WASD / left joystick   √(?
/// 张嘴（自动拾取） space / ?
/// 冲刺 shift / ?
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Player1 kbdinput = null;    // player input
    private Player2 gpdinput = null;
    private Vector2 moveVec = Vector2.zero; // direction
    private Rigidbody player = null;    // keyboard
    public float speed = 0.0f;      // move speed

    private void Awake()
    {
        kbdinput = new Player1();
        player = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        kbdinput.Enable();
        if (this.name.Contains("1"))    // kbd ctrl
        {
            kbdinput.Enable();
            kbdinput.Player.Move.performed += Move;
            kbdinput.Player.Move.canceled += OnMovementCanceled;
        }
        else
        {
            gpdinput.Enable();
            gpdinput.Player.Move.performed += Move;
            gpdinput.Player.Move.canceled += OnMovementCanceled;
        }
    }

    private void OnDisable()
    {
        if (this.name.Contains("1"))    // kbd ctrl
        {
            kbdinput.Disable();
            kbdinput.Player.Move.performed -= Move;
            kbdinput.Player.Move.canceled -= OnMovementCanceled;
        }
        else
        {
            gpdinput.Disable();
            gpdinput.Player.Move.performed -= Move;
            gpdinput.Player.Move.canceled -= OnMovementCanceled;
        }
    }

    private void FixedUpdate()
    {
        player.AddForce(new Vector3(moveVec.x,0,moveVec.y) * speed);
    }

    private void Move(InputAction.CallbackContext context)
    {
        moveVec = context.ReadValue<Vector2>();
        player.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(moveVec.x >= 0 ? "Arts/-c" : "Arts/c");
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        moveVec = Vector2.zero;
    }
    
}
