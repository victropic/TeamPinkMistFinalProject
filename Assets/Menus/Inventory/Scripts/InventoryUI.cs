using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public string inventoryName;
    public int nItemsX;
    public InventoryController inventoryController;
    public GrabbedItem grabbedItem;
    public float itemSlotWidth = 60f;

    RectTransform mainPanel;
    GameObject itemButton;
    GameObject[] slots;
    GameObject infoPanel;
    System.Random random;
    Text nameText;

    bool active;

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();

        //inventoryController = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>();
        mainPanel = GetComponent<RectTransform>();
        itemButton = Resources.Load("Prefabs/UI/ItemButton") as GameObject;
        infoPanel = GameObject.FindGameObjectWithTag("ItemInfo") as GameObject;
        nameText = transform.GetChild(0).GetComponent<Text>();
        nameText.enabled = false;
    }

    public void Toggle() {
        if(!active) {
            Generate();

        } else {
            Ripdown();

            infoPanel.GetComponent<ItemInfoUI>().Hide();
        }
    }

    public void Generate() {

        float nameHeight = 20f;
        nameText.text = inventoryName;
        nameText.enabled = true;

        int sizeX = nItemsX;
        int sizeY = (int)Mathf.Ceil(inventoryController.Length/(float)sizeX);

        active = true;
        mainPanel.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Horizontal, sizeX * itemSlotWidth + 10f);
        mainPanel.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Vertical, sizeY * itemSlotWidth + 10f + nameHeight);

        slots = new GameObject[inventoryController.Length];

        for(int i = 0; i < inventoryController.Length; i++) {
                slots[i] = Instantiate(itemButton, Vector3.zero, Quaternion.identity);
                slots[i].transform.SetParent(transform, false);

                RectTransform objRect = slots[i].GetComponent<RectTransform>();
                objRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 5f + itemSlotWidth * Mathf.Floor(i%sizeX), itemSlotWidth);
                objRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 5f + itemSlotWidth * Mathf.Floor(i/sizeX) + nameHeight, itemSlotWidth);

                ItemSlot itemSlot = slots[i].GetComponent<ItemSlot>();
                itemSlot.CurrentItem = inventoryController.GetItem(i);
                itemSlot.index = i;
        }
    }

    public void Ripdown() {
        active = false;
        for(int i = 0; i < slots.GetLength(0); i++) {
                Destroy(slots[i]);
        }

        grabbedItem.Hide();

        mainPanel.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Horizontal, 0f);
        mainPanel.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Vertical, 0f);

        nameText.enabled = false;
    }

    public void Grab(int index, Item item) {
        if(item != null && grabbedItem.CurrentItem != null && item.itemInfo.itemName == grabbedItem.CurrentItem.itemInfo.itemName) {
            Combine(index, item);
        } else {

            slots[index].GetComponent<ItemSlot>().CurrentItem = grabbedItem.CurrentItem;
            inventoryController.Insert(grabbedItem.CurrentItem, index);

            if(item != null) {
                grabbedItem.CurrentItem = item;
                grabbedItem.Show();
                grabbedItem.CurrentItem = item;
            } else {
                grabbedItem.Hide();
                grabbedItem.CurrentItem = null;
            }
        }
    }

    public void GrabHalf(int index, Item item) {

        /* holding no item and no item in slot,    do nothing */
        if(item == null && grabbedItem.CurrentItem == null)
            return;

        /* Create new items to hold halves */    
        Item first = ScriptableObject.CreateInstance("Item") as Item;
        Item second = ScriptableObject.CreateInstance("Item") as Item;
        
        if(item != null)
            first.itemInfo = item.itemInfo;
        else
            first.itemInfo = grabbedItem.CurrentItem.itemInfo;

        second.itemInfo = first.itemInfo;

        /* Holding item */
        if(grabbedItem.CurrentItem != null) {

            /* Slot has item */
            if(item != null) {

                /* Slot item is the same,   give half of what is held to the slot */
                if(grabbedItem.CurrentItem.itemInfo.itemName == item.itemInfo.itemName) {
                    second.stackAmount = grabbedItem.CurrentItem.stackAmount / 2;
                    first.stackAmount = (grabbedItem.CurrentItem.stackAmount - second.stackAmount) + item.stackAmount;

                    if(first.stackAmount > item.itemInfo.maxStackAmount) {
                        second.stackAmount = second.stackAmount + (first.stackAmount - item.itemInfo.maxStackAmount);
                        first.stackAmount = item.itemInfo.maxStackAmount;
                    }

                }
                /* Slot item is different,    do nothing */
                else {
                    return;
                }
            }
            /* Slot is blank,    give half of what is held to the slot */
            else {
                first.stackAmount = grabbedItem.CurrentItem.stackAmount/2;
                second.stackAmount = grabbedItem.CurrentItem.stackAmount - first.stackAmount;
            }
        }
        /* Not holding item (slot is not empty),   hold half of what's in the slot */
        else {
            first.stackAmount = item.stackAmount/2;
            second.stackAmount = item.stackAmount - first.stackAmount;
        }

        /* assign items to slot and 'hand' */
        if(first.stackAmount > 0) {
            slots[index].GetComponent<ItemSlot>().CurrentItem = first;
            inventoryController.Insert(first, index);
        }
        else {
            slots[index].GetComponent<ItemSlot>().CurrentItem = null;
            inventoryController.Remove(index);
        }

        if(second.stackAmount > 0) {
            grabbedItem.CurrentItem = second;
            grabbedItem.Show();
        } else {
            grabbedItem.Hide();
            grabbedItem.CurrentItem = null;
        }
    }

    public void Combine(int toIndex, Item item) {
        grabbedItem.Hide();

        Item newItem = ScriptableObject.CreateInstance("Item") as Item;

        int amount = item.stackAmount + grabbedItem.CurrentItem.stackAmount;
        int leftover = 0;

        if(amount > item.itemInfo.maxStackAmount) {
            leftover = amount - item.itemInfo.maxStackAmount;
            amount = item.itemInfo.maxStackAmount;
        }

        newItem.itemInfo = item.itemInfo;
        newItem.stackAmount = amount;
        
        slots[toIndex].GetComponent<ItemSlot>().CurrentItem = newItem;
        inventoryController.Insert(newItem, toIndex);
        
        if(leftover > 0) {
            Item newItem2 = ScriptableObject.CreateInstance("Item") as Item;
            newItem2.itemInfo = item.itemInfo;
            newItem2.stackAmount = leftover;

            grabbedItem.CurrentItem = newItem2;
            grabbedItem.Show();

            return;
        }

        grabbedItem.CurrentItem = null;
    }

    public void Consume(int index, Item item) {
        if(item.stackAmount <= 1) {
            slots[index].GetComponent<ItemSlot>().CurrentItem = null;
            inventoryController.Remove(index);

        } else {
            Item newItem = ScriptableObject.CreateInstance("Item") as Item;
            newItem.itemInfo = item.itemInfo;
            newItem.stackAmount = item.stackAmount - 1;

            slots[index].GetComponent<ItemSlot>().CurrentItem = newItem;
            inventoryController.Insert(newItem, index);
        }
    }

    public int GetInventorySpace() {
        if(slots != null)
            return slots.Length;
        else
            return 0;
    }

    public void ReportSlotClicked() {
        grabbedItem.ReportSlotClicked();
    }

    public void ReportSlotNotClicked() {
        grabbedItem.ReportSlotNotClicked();
    }
    
}
