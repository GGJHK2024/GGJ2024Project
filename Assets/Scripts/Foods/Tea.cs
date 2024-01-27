using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tea : FoodsInfo
{
    /// <summary>
    /// 功夫茶：允许二段冲刺
    /// </summary>
    public override void Buff()
    {
        player.is_doubledash = true;
    }

    /// <summary>
    /// 不再允许二段跳
    /// </summary>
    public override void Debuff()
    {
        player.is_doubledash = false;
    }
}
