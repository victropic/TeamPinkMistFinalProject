using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void InventoryChangeHandler();

public class InventoryController : MonoBehaviour
{

    public string inventoryName;
    public Inventory inventoryValues;
    private Inventory inventory;
    
    public InventoryChangeHandler inventoryChangeDelegate;

    void Awake()
    {
        RefreshInventory();
    }

    public Inventory InventoryValues {
        get {
            return inventoryValues;
        }
        set {
            inventoryValues = value;
            RefreshInventory();
        }
    }

    public int Length {
        get {
            if(inventory != null && inventory.items != null) {
                return inventory.items.Count; 
            } else
                return 0;
        }
    }

    private void RefreshInventory() {
        inventory = ScriptableObject.CreateInstance("Inventory") as Inventory;
        inventory.items = new List<Item>();

        for(int i = 0; i < inventoryValues.items.Count; i++) {
            
            if(inventoryValues.items[i] != null) {
                Item newItem = inventoryValues.items[i].Clone();

                inventory.items.Add(newItem);
            } else {
                inventory.items.Add(null);
            }
        }
        if(inventoryChangeDelegate != null)
            inventoryChangeDelegate();
    }

    public Item GetItem(int n) {
        return inventory.items[n];
    }

    public void Move(int a, int b) {
        Item tmp = inventory.items[a];
        inventory.items[a] = inventory.items[b];
        inventory.items[b] = tmp;

        if(inventoryChangeDelegate != null)
            inventoryChangeDelegate();
    }

    public void Insert(Item item, int n) {
        inventory.items[n] = item;
        if(inventoryChangeDelegate != null)
            inventoryChangeDelegate();
    }

    public void Remove(int n) {
        inventory.items[n] = null;
        if(inventoryChangeDelegate != null)
            inventoryChangeDelegate();
    }

    public int SearchAndRemove(ItemInfo itemInfo, int max) {
        if(itemInfo == null)
            return 0;

        int amountFound = 0;
        for(int i = 0; i < inventory.items.Count; i++) {

            if(inventory.items[i] != null && inventory.items[i].itemInfo.itemName == itemInfo.itemName) {
                amountFound += inventory.items[i].stackAmount;

                if(max - amountFound < 0) {
                    inventory.items[i].stackAmount = amountFound - max;
                    return amountFound;
                } else {
                    Remove(i);
                }
            }
        }

        return amountFound;
    }

    public void ChangeCapacity(int capacity) {
        if(capacity < 0)
            return;
        
        if(capacity < Length)
            inventory.items.RemoveRange(capacity, Length - capacity);

        if(capacity > Length) {
            for(int i = 0; i < capacity - Length; i++) {
                inventory.items.Insert(Length, null);
            }
        }
        if(inventoryChangeDelegate != null)
            inventoryChangeDelegate();
    }

    public Inventory GetInventory() {
        return inventory;
    }

    public void SetInventory(Inventory newInventory) {
        inventory = newInventory;
    } 

}
