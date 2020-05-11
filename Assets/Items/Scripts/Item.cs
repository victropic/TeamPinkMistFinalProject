using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{
    public ItemInfo itemInfo;
    public int stackAmount;

    public virtual Item Clone() {
        Item clone = (Item)ScriptableObject.CreateInstance(typeof(Item));

        clone.itemInfo = this.itemInfo;
        clone.stackAmount = this.stackAmount;

        return clone;
    }

}
