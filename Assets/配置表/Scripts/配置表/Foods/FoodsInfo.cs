using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XlsWork;
using XlsWork.FoodsXls;

[Serializable]
public class FoodSettings
{
    public int id;
    public string name;
    public int is_buff;
    public int volume;
    public int speed;
    public int buff_time;
}

public class FoodsInfo : MonoBehaviour
{
    public FoodSettings Settings;

    [Header("配表内ID")]
    public int InitFromID;

}