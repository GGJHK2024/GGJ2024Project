using UnityEngine;
using UnityEditor;
using System;
using XlsWork;
using XlsWork.FoodsXls;

[CustomEditor(typeof(FoodsInfo))]//将本模块指定为UnitInfo组件的编辑器自定义模块
public class FoodsInfo_Editor : Editor
{
    public override void OnInspectorGUI()//对UnitInfo在Inspector中的绘制方式进行接管
    {
        DrawDefaultInspector();//绘制常规内容

        if (GUILayout.Button("从配表ID刷新食物数据"))//添加按钮和功能——当组件上的按钮被按下时
        {
            FoodsInfo foodsInfo = (FoodsInfo)target;
            Init(foodsInfo);
        }
    }

    public void Init(FoodsInfo instance)
    {
        Action initfood;

        var dictionary = FoodProps.LoadExcelAsDictionary();

        if (!dictionary.ContainsKey(instance.InitFromID))
        {
            Debug.LogErrorFormat("未能在 foods 配表中找到指定的ID:{0}", instance.InitFromID);
            return;
        }
        IndividualData item = dictionary[instance.InitFromID];

        initfood = (() =>
        {
            instance.Settings.id = Convert.ToInt32(item.Values[0]);
            instance.Settings.name = Convert.ToString(item.Values[1]);
            instance.Settings.is_buff = Convert.ToInt32(item.Values[2]);
            instance.Settings.volume = Convert.ToInt32(item.Values[3]);
            instance.Settings.speed = Convert.ToInt32(item.Values[4]);
            instance.Settings.buff_time = Convert.ToInt32(item.Values[5]);
        });

        initfood();
    }
}