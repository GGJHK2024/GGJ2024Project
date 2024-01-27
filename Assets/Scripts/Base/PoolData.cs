using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽屉数据  池子中的一列容器
/// </summary>
public class PoolData:MonoBehaviour
{
    //抽屉中 对象挂载的父节点
    public GameObject fatherObj;
    //对象的容器
    public List<GameObject> poolList;

    /// <summary>
    /// obj是作为对象池挂载的内容，理解为子弹
    /// poolObj就是弹夹了
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="poolObj"></param>
    public PoolData(GameObject obj, GameObject poolObj)
    {
        //给我们的抽屉 创建一个父对象 并且把他作为我们pool(衣柜)对象的子物体
        fatherObj = poolObj;
        obj.transform.SetParent(fatherObj.transform);
        poolList = new List<GameObject>();
        PushObj(obj);
    }

        /// <summary>
    /// 往抽屉里面 压都东西
    /// </summary>
    /// <param name="obj"></param>
    public void PushObj(GameObject obj)
    {
        //失活 让其隐藏
        obj.SetActive(false);
        //存起来
        poolList.Add(obj);
        //设置父对象
        obj.transform.SetParent(fatherObj.transform);
    }
    
    /// <summary>
    /// 从抽屉里面取东西, 不含初始坐标
    /// </summary>
    /// <returns></returns>
    public GameObject GetObj()
    {
        GameObject obj = null;
        //取出第一个
        obj = poolList[0];
        poolList.RemoveAt(0);
        //激活 让其显示
        obj.SetActive(true);
        //断开了父子关系
        obj.transform.SetParent(null);
        return obj;
    }

    // 这是一个根本不会执行的协程，但是他可以让子弹变得正常，但也会报错（不用管
    IEnumerator Wait(){
        yield return new WaitForSeconds(0.5f);
        print("等待完成");
        yield break;
    }

    /// <summary>
    /// 从抽屉里面取东西, 含初始坐标
    /// </summary>
    /// <returns></returns>
    public GameObject GetObj(Vector3 position)
    {
        GameObject obj = null;
        //取出第一个
        obj = poolList[0];
        poolList.RemoveAt(0);

        // 设置初始位置
        obj.transform.position = position;
        StartCoroutine(Wait());

        //激活 让其显示
        obj.SetActive(true);
        //断开了父子关系
        obj.transform.SetParent(null);
        return obj;
    }
}