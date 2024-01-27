using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;


/// <summary>
/// 移动控制 WASD / left joystick
/// 张嘴（自动拾取） space / right trigger
/// 冲刺 shift / B
/// p1棕色 p2白色（暂定
/// </summary>
public class PlayerController : MonoBehaviour
{
    /*基本属性*/
    public float speed = 0.0f;      // 基础速度，速度越快，冲刺造成的伤害越高，伤害等于击飞值的变量
    public float current_speed = 0.0f;//实际速度
    public float attack_speed = 15f;//允许攻击的速度
    public float max_speed = 0.0f; // 角色属性的速度上限
    public float pick_area = 5.0f;  // 拾取范围
    private bool one_life = false; // 一命模式
    public bool one_life_smash = false; // 被一命击杀了
    public Vector4 boundary = new Vector4(25, -25, -33, 33);  // 可移动边界；上下左右

    public Animator anim; // 动画机
    public SpriteRenderer img;  // 贴图sp

    [Header("撞击相关")]
    public bool is_hit = false;//是否为可以造成有效攻击
    public bool is_hitting = false;//是否在被击飞状态
    public bool is_smashing = false;//在击破撞到墙前
    public float hit_prop = 0.0f;   // 击飞值，击飞值越高，被击飞概率越大
    public float max_hit_prop = 100.0f;  // 最大击飞值
    public float smash_odds = 0.0f; //击破概率
    public int smash_count = 0;       // 被击破的次数
    public float dash_cd = 10.0f;   // 冲刺CD，10s
    public float dash_speed_k = 25.0f;  // 冲刺速度系数
    public bool is_dash = false;        // 是否冲刺
    public bool is_doubledash = false;  // 是否二段冲刺
    public float hit_dmg = 0.0f;    // 裸体撞击伤害
    // public FoodsInfo hit_out;    // 受到撞击吐多少东西

    public int dash_count = 0; // 冲刺次数
    
    [Header("护盾相关")]
    public bool is_shield = false;  // 是否带有一次性护盾

    /*控制属性*/
    public GameObject otherP;
    private Player2 kbdinput = null;    // player input
    private Player2 gpdinput = null;
    private Vector2 moveVec = Vector2.zero; // direction
    private Rigidbody player = null;
    

    /*材质*/
    Material fl;

    private void Awake()
    {
        kbdinput = new Player2();
        gpdinput = new Player2();
        player = GetComponent<Rigidbody>();
        fl = img.material;
        var allGamepads = Gamepad.all;
        if (this.gameObject.name.Contains("1"))
        {
            var user1 = InputUser.PerformPairingWithDevice(allGamepads[0]);
            user1.AssociateActionsWithUser(kbdinput);
        }
        else
        {
            var user2 = InputUser.PerformPairingWithDevice(allGamepads[1]);
            user2.AssociateActionsWithUser(gpdinput);
        }
    }
    
    private void OnEnable()
    {
        AudioMgr.GetInstance().PlaySound((this.gameObject.name.Contains("1"))?"Audios/P1宣战":"Audios/P2宣战");
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
            kbdinput.Player.OpenMouse.canceled += PutDown;
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
            gpdinput.Player.OpenMouse.canceled += PutDown;
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
            kbdinput.Player.OpenMouse.canceled -= PutDown;
        }
        else
        {
            gpdinput.Disable();
            gpdinput.Player.Move.performed -= Move;
            gpdinput.Player.Move.canceled -= OnMovementCanceled;
            gpdinput.Player.Dash.performed -= Dash;
            gpdinput.Player.OpenMouse.performed -= GrabSth;
            gpdinput.Player.OpenMouse.canceled -= PutDown;
        }
    }

    private void FixedUpdate()
    {
        player.AddForce(moveVec * speed);
        Attackspeed();
    }

    private void Update()
    {
        transform.GetChild(5).gameObject.SetActive(is_shield ? true : false);
        if (is_hitting)
        {
            this.transform.GetChild(3).GetComponent<ParticleSystem>().Play();
            anim.Play(gameObject.name.Contains("1")?"Brown_Hitted":"White_Hitted");
        }

        if (moveVec.x != 0 && moveVec.y != 0)
        {
            transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            if (!is_dash)
            {
                if (current_speed < 15.0f)   // 在动,且不在跑
                {
                    anim.Play(gameObject.name.Contains("1")?"Brown_Run":"White_Run");
                } else  // 达到伤害速度
                {
                    anim.Play(gameObject.name.Contains("1")?"Brown_FastRun":"White_Fast_Run");
                }
            }
            else
            {
                if (current_speed < 6.0f)   // 在动,且不在跑
                {
                    anim.Play(gameObject.name.Contains("1")?"Brown_Run":"White_Run");
                }
            }
        }else {
            transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
            anim.Play("New State");
        }
    }

    private void LateUpdate()
    {
        BoundaryCheck();
    }

    /// <summary>
    /// 拾取（只拾取可拾取范围内最近的武器）
    /// </summary>
    private void GrabSth(InputAction.CallbackContext context)
    {
        anim.Play(gameObject.name.Contains("1")?"Brown_Eat":"White_Eat");
        
        List<Collider> weaponCanBeGrab = new List<Collider>();
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, pick_area);
        Collider[] orderedByProximity = hitColliders.OrderBy(c => (this.transform.position - c.transform.position).sqrMagnitude).ToArray();
        
        // 从近到远排序可拾取范围内的道具
        foreach (var hitCollider in orderedByProximity)
        {
            if (!hitCollider.CompareTag("weapon"))  // 只抓取武器
                continue;
            
            if (!hitCollider.transform.GetComponent<WeaponsInfo>().isFlying) // 武器在飞行过程中无法抓取
            {
                weaponCanBeGrab.Add(hitCollider);
                break;
            }
            else
            {
                if (!hitCollider.transform.GetComponent<WeaponsInfo>().canBePickWhileFlying) continue; // （特殊除外，例如弯刀
                weaponCanBeGrab.Add(hitCollider);
                break;
            }
        }

        if (weaponCanBeGrab.Count == 0) // 周围没有可拾取的武器
            return;
        
        weaponCanBeGrab[0].transform.GetComponent<WeaponsInfo>().isFlying = false;
        weaponCanBeGrab[0].transform.SetParent(this.transform.GetChild(0));
        weaponCanBeGrab[0].transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        weaponCanBeGrab[0].transform.localPosition = new Vector3(0, 0, -0.05f);
        EventCenter.GetInstance().EventTrigger("On"+ weaponCanBeGrab[0].name + "PickUp");
    }

    /// <summary>
    /// 释放武器（如果持有的话）
    /// </summary>
    /// <param name="context"></param>
    private void PutDown(InputAction.CallbackContext context)
    {
        anim.Play(gameObject.name.Contains("1")?"Brown_Eat":"White_Eat");
        
        if (this.transform.GetChild(0).childCount == 0) return;
        
        // 有拾取的武器时
        GameObject weapon = this.transform.GetChild(0).GetChild(0).gameObject;
        weapon.GetComponent<Rigidbody>().isKinematic = false;
        weapon.transform.SetParent(GameObject.Find("PoolWeapons").transform); // 释放武器
        
        if (moveVec is { x: 0, y: 0 } && !weapon.GetComponent<WeaponsInfo>().isOnce)      // 玩家在静止时放下武器，武器不会飞出或消耗耐久
        {
            return;
        }
            
        if (!weapon.GetComponent<WeaponsInfo>().isOnce) // 非一次性武器在释放时耐久-1
        {
            weapon.GetComponent<WeaponsInfo>().Break();
        }
        
        weapon.GetComponent<WeaponsInfo>().Buff();  // 释放时各自的特殊事件

        // 武器弹射
        if (is_dash)
        {
            weapon.GetComponent<Rigidbody>().AddForce(moveVec * dash_speed_k * 20);   // 冲刺时弹射更快
            weapon.GetComponent<WeaponsInfo>().isFlying = true; // 武器处于飞行过程
        }
        else
        {
            weapon.GetComponent<Rigidbody>().AddForce(moveVec * dash_speed_k * 10);
            weapon.GetComponent<WeaponsInfo>().isFlying = true;
        }
    }

    /// <summary>
    /// 冲刺
    /// 二段冲刺：连续冲两次再进入cd
    /// </summary>
    /// <param name="context"></param>
    private void Dash(InputAction.CallbackContext context)
    {
        if (!is_dash || (is_doubledash && dash_count <= 1))
        {
            AudioMgr.GetInstance().PlaySound((this.gameObject.name.Contains("1"))?"Audios/p1冲刺":"Audios/p2冲刺");
            
            anim.Play(gameObject.name.Contains("1")?"Brown_Dash":"White_Dash");
            
            player.AddForce(moveVec * speed * dash_speed_k);
            is_dash = true;
            dash_count++;
            Invoke(nameof(OnDashCdEnding), dash_cd);    // cd后重新变为可冲刺的状态
        }
    }
    
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="context"></param>
    private void Move(InputAction.CallbackContext context)
    {
        this.GetComponent<AudioSource>().Play();
        moveVec = context.ReadValue<Vector2>();
        player.transform.rotation = Quaternion.Euler(0, moveVec.x >= 0 ? 180 : 0, 0);
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    /// <param name="context"></param>
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        this.GetComponent<AudioSource>().Stop();
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

    /// <summary>
    /// 边界检测
    /// </summary>
    private void BoundaryCheck()
    {
        if (this.transform.position.y > boundary.x)
        {
            transform.position = new Vector3(transform.position.x,boundary.x,0);
        }
        if (this.transform.position.y < boundary.y)
        {
            transform.position = new Vector3(transform.position.x,boundary.y,0);
        }
        if (this.transform.position.x > boundary.w)
        {
            transform.position = new Vector3(boundary.w,transform.position.y,0);
        }
        if (this.transform.position.x < boundary.z)
        {
            transform.position = new Vector3(boundary.z,transform.position.y,0);
        }
    }
    
    /// <summary>
    /// 攻击速度
    /// </summary>
    private void Attackspeed()
    {
        current_speed = player.velocity.magnitude;//获取实际速度
        if (current_speed >= attack_speed)//判定是否可以造成伤害
        {
            //需要一个动画改变
            is_hit = true;
        }
        else
        {
            is_hit = false;
        }

        if(current_speed <= 8)
        { 
            is_hitting = false;
            fl.SetInt("_BeAttack",0);
        } 
    }

    /// <summary>
    /// 碰撞检测
    /// </summary>
    private void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.CompareTag("Player"))   // 碰撞玩家
        {
            var otherPlayer = hitCollider.gameObject.GetComponent<PlayerController>();
            player.AddForce(-moveVec * current_speed * 90);//碰撞的基础弹飞，增加打击感同时避免重复判定
            if (is_hit && otherPlayer.is_hit) //双方可以进行攻击
            {
                if (is_shield)  // 如果有护盾，抵挡一次伤害并且护盾破碎
                {
                    is_shield = false;
                    EventCenter.GetInstance().EventTrigger("McdonaldBreak");
                }
                else
                {   
                    is_hitting = true;
                    fl.SetInt("_BeAttack",1);
                    hit_prop += 0.5f;
                    AudioMgr.GetInstance().PlaySound((this.gameObject.name.Contains("1"))?"Audios/P1受击":"Audios/P2受击");
                    anim.Play(this.gameObject.name.Contains("1")?"Brown_Hitted":"White_Hitted");
                    this.transform.GetChild(3).GetComponent<ParticleSystem>().Play();
                    player.AddForce(-moveVec * current_speed * hit_prop * 10); //两者会额外小幅击飞并小幅叠加击飞值

                }
            }
            else if(!is_hit && otherPlayer.is_hit)//我方不能攻击
            {
                if (is_shield)  // 如果有护盾，抵挡一次伤害并且护盾破碎
                {
                    is_shield = false;
                    EventCenter.GetInstance().EventTrigger("McdonaldBreak");
                }
                else
                {
                    is_hitting = true;
                    fl.SetInt("_BeAttack",1);
                    hit_prop += 2f;
                    AudioMgr.GetInstance().PlaySound((this.gameObject.name.Contains("1"))?"Audios/P1受击":"Audios/P2受击");
                    anim.Play(this.gameObject.name.Contains("1")?"Brown_Hitted":"White_Hitted");
                    this.transform.GetChild(3).GetComponent<ParticleSystem>().Play();
                    player.AddForce(otherPlayer.moveVec * otherPlayer.current_speed * hit_prop * 10);//我方会额外大幅击飞并叠加击飞值
                    // 击飞和击破
                    if(hit_prop > max_hit_prop)//当达到最大击飞值后开始叠加一击击破率
                    {
                        smash_odds += hit_prop;
                        hit_prop = max_hit_prop;
                        if (smash_odds > UnityEngine.Random.Range(0,100))
                        {
                            if (is_shield)  // 如果有护盾，抵挡一次伤害并且护盾破碎
                            {
                                is_shield = false;
                                EventCenter.GetInstance().EventTrigger("McdonaldBreak");
                            }
                            else
                            {
                                print(this.gameObject.name + "被一击必杀了！");

                                AudioMgr.GetInstance().PlaySound((this.gameObject.name.Contains("1"))?"Audios/P1被击飞":"Audios/P2被击飞");
                                player.AddForce(otherPlayer.moveVec * 5000);
                                is_smashing = true;
                            }
                        }
                    }
                }
            }


        }

        if (hitCollider.CompareTag("food")) // 吃到食物
        {
            // print("get food: " + hitCollider.name);
            var food = hitCollider.GetComponent<FoodsInfo>();
            food.GeneralEffect(this);
            // 食物有buff效果
            if (food.Settings.is_buff == 1)
            {
                food.Buff();
                StartCoroutine(DeBuffFromFood(food));
            }
        }

        if (hitCollider.CompareTag("weapon")) //碰撞武器
        {

            var weapon = hitCollider.GetComponent<WeaponsInfo>();
            if (weapon.isFlying && weapon.flying_timer >= 0.08f)    // 当玩家撞到飞行中的武器时（可以在飞的过程中叼住但没叼到也算），会受到基础伤害并被轻微撞击
            {
                if (is_shield)  // 如果有护盾，抵挡一次伤害并且护盾破碎
                {
                    is_shield = false;
                    EventCenter.GetInstance().EventTrigger("McdonaldBreak");
                }
                else
                {
                    weapon.Damage(this.gameObject);
                    fl.SetInt("_BeAttack",1);
                    weapon.HitPlayerWhileFlying(this);
                    // 击飞和击破
                    if(hit_prop > max_hit_prop)//当达到最大击飞值后开始叠加一击击破率
                    {
                        smash_odds += hit_prop;
                        hit_prop = max_hit_prop;
                        if (smash_odds > UnityEngine.Random.Range(0,100))
                        {
                            if (is_shield)  // 如果有护盾，抵挡一次伤害并且护盾破碎
                            {
                                is_shield = false;
                                EventCenter.GetInstance().EventTrigger("McdonaldBreak");
                            }
                            else
                            {
                                print(gameObject.name + "被一击必杀了！");

                                AudioMgr.GetInstance().PlaySound((this.gameObject.name.Contains("1"))?"Audios/P1被击飞":"Audios/P2被击飞");
                                player.AddForce(weapon.rigidbody.velocity * 5000);
                                is_smashing = true;
                            }
                        }
                    }
                }
            }

        }

        if (hitCollider.CompareTag("wall")) //碰撞墙壁弹反
        {
            if(current_speed >= 9)
            {
                player.AddForce(-player.velocity.normalized * current_speed * 100); 
            }
            if(is_smashing)
            {
                // todo: otherPlayer无法行动，有光照在他身上，直到另一个玩家回来
                print(otherP.name + " 沐浴在圣光中");
                otherP.transform.GetChild(4).gameObject.SetActive(true);
                if (otherP.gameObject.name.Contains("1"))
                {
                    otherP.GetComponent<PlayerController>().kbdinput.Disable();
                }
                else
                {
                    otherP.GetComponent<PlayerController>().gpdinput.Disable();
                }
                SmashAndBack();
            }
        }


    }

    IEnumerator DeBuffFromFood(FoodsInfo f)
    {
        yield return new WaitForSeconds(f.Settings.buff_time);
        f.Debuff();

        yield return null;
    }

    /// <summary>
    /// 被击破和重生
    /// </summary>
    private void SmashAndBack()
    {
            player.Sleep();
            Color ready = new Color(1f, 1f, 1f, 0.5f);
            this.transform.Find("truePlayer").GetComponent<SpriteRenderer>().color = ready;
            smash_count ++;
            if(one_life){one_life_smash = true; GameMgr.GameOverAdd(); return;}
            if(smash_count >= 2){GameMgr.GameOver();return;}
            speed = 20.0f;
            hit_prop = 0.0f;
            smash_odds = 0.0f;
            this.transform.localScale = new Vector3(1.5f , 1.5f , 1.5f);
            this.transform.position = new Vector3(0,0,0);

            is_smashing = false;
            Invoke("Ready", 3f);
            
    }

    /// <summary>
    /// 重生
    /// </summary>
    private void Ready()
    {
            transform.Find("truePlayer").GetComponent<SpriteRenderer>().color = new Color(255,255,255,255);
            // 另一个玩家重新控制
            print(otherP.name + " 重新控制");
            if (otherP.gameObject.name.Contains("1"))
            {
                otherP.GetComponent<PlayerController>().kbdinput.Enable();
            }
            else
            {
                otherP.GetComponent<PlayerController>().gpdinput.Enable();
            }
            otherP.transform.GetChild(4).gameObject.SetActive(false);
            player.WakeUp(); 
    }
}
