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
    
    /*基本属性
    * 击飞值，击飞值越高，被击飞概率越大
    * 被击飞的次数
    * 速度，速度越快，冲刺造成的伤害越高，伤害等于击飞值的变量
    * 冲刺CD，10s
    * 体积
    * 是否带有一次性护盾
    * 是否带有持续护盾
    * 持续护盾时间
    * 是否二段冲刺
    * 二段冲刺buff时间
    * 受到撞击吐多少东西
    * 裸体撞击伤害
    * 角色属性的上限（血量/速度）
     */
    public float speed = 0.0f;
    public float volum_scale = 1.0f;
    [Header("撞击相关")]
    public float hit_prop = 0.0f;
    public int hit_count = 0;
    public float dash_cd = 0.0f;
    public float dash_speed_k = 25.0f;
    public bool is_doubledash = false;
    public float hit_dmg = 0.0f;
    [Header("护盾相关")]
    public bool is_shield = false;
    public float shield_cd = 0.0f;
    // public float 这是啥
    // 这是啥
    
    /*控制属性*/
    
    private Player1 kbdinput = null;    // player input
    private Player2 gpdinput = null;
    private Vector2 moveVec = Vector2.zero; // direction
    private Rigidbody player = null;    // keyboard

    private void Awake()
    {
        kbdinput = new Player1();
        gpdinput = new Player2();
        player = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        kbdinput.Enable();
        if (this.name.Contains("1"))    // kbd ctrl
        {
            kbdinput.Enable();
            // wasd
            kbdinput.Player.Move.performed += Move;
            kbdinput.Player.Move.canceled += OnMovementCanceled;
            // shift
            kbdinput.Player.Dash.performed += Dash;
        }
        else
        {
            gpdinput.Enable();
            // left joystick
            gpdinput.Player.Move.performed += Move;
            gpdinput.Player.Move.canceled += OnMovementCanceled;
            // B
            gpdinput.Player.Dash.performed += Dash;
        }
    }

    private void OnDisable()
    {
        if (this.name.Contains("1"))    // kbd ctrl
        {
            kbdinput.Disable();
            kbdinput.Player.Move.performed -= Move;
            kbdinput.Player.Move.canceled -= OnMovementCanceled;
            kbdinput.Player.Dash.performed -= Dash;
        }
        else
        {
            gpdinput.Disable();
            gpdinput.Player.Move.performed -= Move;
            gpdinput.Player.Move.canceled -= OnMovementCanceled;
            gpdinput.Player.Dash.performed -= Dash;
        }
    }

    private void FixedUpdate()
    {
        player.AddForce(moveVec * speed);
    }

    /// <summary>
    /// 冲刺
    /// </summary>
    /// <param name="context"></param>
    private void Dash(InputAction.CallbackContext context)
    {
        // print(moveVec);
        player.AddForce(moveVec * speed * dash_speed_k);
    }
    
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="context"></param>
    private void Move(InputAction.CallbackContext context)
    {
        moveVec = context.ReadValue<Vector2>();
        player.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(moveVec.x >= 0 ? "Arts/-c" : "Arts/c");
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    /// <param name="context"></param>
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        moveVec = Vector2.zero;
    }
    
}
