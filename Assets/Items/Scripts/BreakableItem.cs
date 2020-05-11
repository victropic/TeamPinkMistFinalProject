using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BreakableItem", menuName = "ScriptableObjects/BreakableItem", order = 1)]
public class BreakableItem : Item
{
    public float health;

    public override Item Clone() {
        BreakableItem clone = (BreakableItem)ScriptableObject.CreateInstance(typeof(BreakableItem));

        clone.itemInfo = itemInfo;
        clone.stackAmount = stackAmount;
        clone.health = health;

        return clone;
    }
}
