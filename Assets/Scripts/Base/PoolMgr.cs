using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// 用于敌人生成的缓存池(与卡牌的不一样，涉及到位置和旋转的问题，所以直接开了个新的类避免混淆
/// </summary>
public class PoolMgr : BaseMgr<PoolMgr>
{
    //缓存池容器
    public Dictionary<string, PoolData> PoolDic = new Dictionary<string, PoolData>();

    private GameObject poolObj;

    /// <summary>
    /// 往外拿东西，含初始坐标
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public void GetObj(string name, Vector3 position, UnityAction<GameObject> callBack)
    {
        //有抽屉 并且抽屉里有东西
        if (PoolDic.ContainsKey(name) && PoolDic[name].poolList.Count > 0)
        {
            if(callBack!=null) callBack(PoolDic[name].GetObj(position));
            else PoolDic[name].GetObj(position);
        }
        else//通过异步加载资源 创建对象给外部用
        {
            Resources.Load<GameObject>(name);
        }
    }

    /// <summary>
    /// 往外拿东西，不含初始坐标
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public void GetObj(string name, UnityAction<GameObject> callBack)
    {
        //有抽屉 并且抽屉里有东西
        if (PoolDic.ContainsKey(name) && PoolDic[name].poolList.Count > 0)
        {
            if(callBack!=null) callBack(PoolDic[name].GetObj());
            else PoolDic[name].GetObj();
        }
        else//通过加载资源 创建对象给外部用
        {
            callBack(Resources.Load<GameObject>(name));
        }

    }

    public void PushObj(GameObject obj)
    {
        PushObj(obj.name,obj);
    }

    /// <summary>
    /// 换暂时不用的东西给我
    /// </summary>
    public void PushObj(string name, GameObject obj)
    {
        if (poolObj == null)
            poolObj = new GameObject("Pool");

        //里面有抽屉
        if (PoolDic.ContainsKey(name))
        {
            PoolDic[name].PushObj(obj);
        }
        //里面没有抽屉
        else
        {
            PoolDic.Add(name, new PoolData(obj, poolObj));
        }
    }

    /// <summary>
    /// 清空缓存池
    /// 在关卡切换时使用
    /// </summary>
    public void Clear()
    {
        PoolDic.Clear();
        poolObj = null;
    }
}