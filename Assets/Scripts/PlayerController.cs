using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 移动控制 WASD / left joystick
/// 张嘴（自动拾取） space / right trigger
/// 冲刺 shift / B
/// </summary>
public class PlayerController : MonoBehaviour
{
    /*基本属性*/
    public float speed = 0.0f;      // 速度，速度越快，冲刺造成的伤害越高，伤害等于击飞值的变量
    public float max_speed = 20.0f; // 角色属性的速度上限
    public float volum_scale = 1.0f;    // 体积
    
    [Header("撞击相关")]
    public float hit_prop = 0.0f;   // 击飞值，击飞值越高，被击飞概率越大
    public float max_hit_prop = 20.0f;  // 最大击飞值（？
    public int hit_count = 0;       // 被击飞的次数
    public float dash_cd = 10.0f;   // 冲刺CD，10s
    public float dash_speed_k = 25.0f;  // 冲刺速度系数
    public bool is_dash = false;        // 是否冲刺
    public bool is_doubledash = false;  // 是否二段冲刺
    public float doubledash_time = 10.0f;   // 二段冲刺buff时间
    public float hit_dmg = 0.0f;    // 裸体撞击伤害
    // public ?? hit_out;   // 受到撞击吐多少东西,类还没写

    private int dash_count = 0;
    
    [Header("护盾相关")]
    public bool is_tem_shield = false;  // 是否带有一次性护盾
    public bool is_shield = false;  // 是否带有持续护盾
    public float shield_cd = 0.0f;  // 持续护盾时间

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
            // space
            kbdinput.Player.OpenMouse.performed += GrabSth;
        }
        else
        {
            gpdinput.Enable();
            // left joystick
            gpdinput.Player.Move.performed += Move;
            gpdinput.Player.Move.canceled += OnMovementCanceled;
            // B
            gpdinput.Player.Dash.performed += Dash;
            // right trigger
            gpdinput.Player.OpenMouse.performed += GrabSth;
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
            kbdinput.Player.OpenMouse.performed -= GrabSth;
        }
        else
        {
            gpdinput.Disable();
            gpdinput.Player.Move.performed -= Move;
            gpdinput.Player.Move.canceled -= OnMovementCanceled;
            gpdinput.Player.Dash.performed -= Dash;
            gpdinput.Player.OpenMouse.performed -= GrabSth;
        }
    }

    private void FixedUpdate()
    {
        player.AddForce(moveVec * speed);
    }

    /// <summary>
    /// 拾取（只拾取可拾取范围内最近的武器）
    /// </summary>
    private void GrabSth(InputAction.CallbackContext context)
    {
        print("open mouth");
    }

    /// <summary>
    /// 冲刺
    /// 二段冲刺：连续冲两次再进入cd
    /// </summary>
    /// <param name="context"></param>
    private void Dash(InputAction.CallbackContext context)
    {
        // todo: fix double dash problem
        if (!is_dash && !is_doubledash)   // 不在冲刺，这时可以允许玩家冲刺(单)
        {
            player.AddForce(moveVec * speed * dash_speed_k);
            is_dash = true;
        }
        
        if (is_dash && is_doubledash && dash_count == 0)   // 允许二段冲刺且还未执行
        {
            print("二次撞击");
            player.AddForce(moveVec * speed * dash_speed_k);
            dash_count++;   // 标记已经第二次冲刺（1）
        }
        
        Invoke(nameof(OnDashCdEnding), dash_cd);    // cd后重新变为可冲刺的状态

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

    /// <summary>
    /// 冲刺cd倒计时
    /// </summary>
    private void OnDashCdEnding()
    {
        print("cd冷却结束");
        dash_count = 0;
        is_dash = false;
    }
    
}
