using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XlsWork;
using XlsWork.WeaponsXls;

[Serializable]
public class WeaponSettings
{
    public int id;
    public string name;
    public int durable;
    public int speed_range;
    public int basic_damage;
    public int external_damage;
}

public class WeaponsInfo : MonoBehaviour
{
    public WeaponSettings Settings;

    [Header("配表内ID")]
    public int InitFromID;

}