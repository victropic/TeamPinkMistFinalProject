﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfo", menuName = "ScriptableObjects/ItemInfo", order = 1)]
public class ItemInfo : ScriptableObject
{
    public string itemName;
    public string description;

    public Sprite icon;
    public float value;
    public int maxStackAmount;
}
