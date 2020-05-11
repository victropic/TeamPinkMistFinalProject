using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RefillableItem", menuName = "ScriptableObjects/RefillableItem", order = 1)]
public class RefillableItem : Item
{
    public int ammo;

    public override Item Clone() {
        RefillableItem clone = (RefillableItem)ScriptableObject.CreateInstance(typeof(RefillableItem));

        clone.itemInfo = itemInfo;
        clone.stackAmount = stackAmount;
        clone.ammo = ammo;

        return clone;
    }
}
