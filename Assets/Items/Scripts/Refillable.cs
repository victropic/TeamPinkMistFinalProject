using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Refillable", menuName = "ScriptableObjects/Refillable", order = 1)]
public class Refillable : Equipable
{
    public int maxAmmo;
    public ItemInfo ammoItemInfo;
}
