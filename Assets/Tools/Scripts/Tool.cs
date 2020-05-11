using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    protected Item item;

    public virtual void Use() {

    }

    public virtual void Refill(int amount, InventoryController ic) {
        
    }

    public Item Item {
        get {
            return item;
        }
        set {
            item = value;
        }
    }
}
