using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{

    ItemInfoUI infoPanel;
    InventoryUI inventoryUI;
    Image image;
    Text stackAmount;
    
    public Sprite emptyCellSprite;
    public Color emptyCellColor;

    public bool hovering;
    
    private Item item;
    public int index;

    Player player;

    void Awake() {
        image = transform.GetChild(0).GetComponent<Image>();
        stackAmount = transform.GetChild(1).GetComponent<Text>();
        
        emptyCellSprite = image.sprite;
        emptyCellColor = image.color;
    }

    void Start() {
        infoPanel = GameObject.FindGameObjectWithTag("ItemInfo").GetComponent<ItemInfoUI>();
        inventoryUI = transform.parent.GetComponent<InventoryUI>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    /* Because this is run in late update,
    the InventoryUI can set itemClickedSlot to false*/
    void LateUpdate() {
        bool useItem = Input.GetKey(KeyCode.LeftShift);

        if(hovering) {
            
            if(Input.GetMouseButtonDown(0)) {
                if(!useItem)
                    OnClick(0);
                else
                    UseItem();

                inventoryUI.ReportSlotClicked();
            }
            else if(Input.GetMouseButtonDown(1)) {
                OnClick(1);
                inventoryUI.ReportSlotClicked();
            }
        } else {
            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
                inventoryUI.ReportSlotNotClicked();
            }
        }
    }

    public void OnPointerEnter() {
        infoPanel.CurrentItem = item;
        infoPanel.Show();

        hovering = true;
    }

    public void OnPointerExit() {
        infoPanel.Hide();
        hovering = false;
    }

    public Item CurrentItem {
        get {
            return item;
        }
        set {
            item = value;
            if(item != null) {
                image.sprite = item.itemInfo.icon;
                image.color = Color.white;
                stackAmount.enabled = true;
                System.Type type = item.itemInfo.GetType();
                if(type != typeof(Equipable)) {
                    stackAmount.text = "" + item.stackAmount;
                }
                else {
                    stackAmount.text = "";
                }
                
                if(type == typeof(Refillable)) {
                    RefillableItem refItem = (RefillableItem)item;
                    stackAmount.text = "" + refItem.ammo;
                }

                if(type == typeof(Breakable)) {
                    stackAmount.text = "";
                    Breakable breakable = (Breakable)item.itemInfo;
                    BreakableItem breakableItem = (BreakableItem)item;
                    image.color = Color.Lerp(Color.red, Color.white, breakableItem.health/breakable.maxDurability);
                }

                
            } else {
                image.sprite = emptyCellSprite;
                image.color = emptyCellColor;
                stackAmount.enabled = false;
            }
        }
    }

    public void OnClick(int mouseNumber) {
        if(mouseNumber == 0)
            inventoryUI.Grab(index, item);
        else
            inventoryUI.GrabHalf(index, item);
    }

    public void UseItem() {
        if(item != null && item.itemInfo.GetType() == typeof(Consumable)) {
            Consumable consumable = (Consumable)item.itemInfo;
            player.AddStatusEffect(consumable.statusEffect);
            inventoryUI.Consume(index, item);
        }
    }
}
