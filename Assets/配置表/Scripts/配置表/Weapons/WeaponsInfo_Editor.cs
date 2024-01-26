using UnityEngine;
using UnityEditor;
using System;
using XlsWork;
using XlsWork.WeaponsXls;

[CustomEditor(typeof(WeaponsInfo))]//将本模块指定为WeaponsInfo组件的编辑器自定义模块
public class WeaponsInfo_Editor : Editor
{
    public override void OnInspectorGUI()//对UnitInfo在Inspector中的绘制方式进行接管
    {
        DrawDefaultInspector();//绘制常规内容

        if (GUILayout.Button("从配表ID刷新武器数据"))//添加按钮和功能——当组件上的按钮被按下时
        {
            WeaponsInfo weaponsInfo = (WeaponsInfo)target;
            Init(weaponsInfo);
        }
    }

    public void Init(WeaponsInfo instance)
    {
        Action init;

        var dictionary = WeaponProps.LoadExcelAsDictionary();

        if (!dictionary.ContainsKey(instance.InitFromID))
        {
            Debug.LogErrorFormat("未能在 weapons 配表中找到指定的ID:{0}", instance.InitFromID);
            return;
        }
        IndividualData item = dictionary[instance.InitFromID];

        init = (() =>
        {
            instance.Settings.id = Convert.ToInt32(item.Values[0]);
            instance.Settings.name = Convert.ToString(item.Values[1]);
            instance.Settings.durable = Convert.ToInt32(item.Values[2]);
            instance.Settings.speed_range = Convert.ToInt32(item.Values[3]);
            instance.Settings.basic_damage = Convert.ToInt32(item.Values[4]);
            instance.Settings.external_damage = Convert.ToInt32(item.Values[5]);
        });

        init();
    }
}