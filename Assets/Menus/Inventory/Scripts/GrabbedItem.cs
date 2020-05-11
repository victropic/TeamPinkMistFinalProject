using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrabbedItem : MonoBehaviour
{
    RectTransform rectTransform;
    Image panel;
    Image icon;
    Text stackAmount;

    Sprite emptyCellSprite;
    Color emptyCellColor;

    Item item;

    private bool total;
    private bool itemSlotWasClicked;
    private int nReported;

    public GameObject discardBoxPref;

    // Start is called before the first frame update
    void Start()
    {

        rectTransform = GetComponent<RectTransform>();
        panel = GetComponent<Image>();
        icon = transform.GetChild(0).GetComponent<Image>();
        stackAmount = transform.GetChild(1).GetComponent<Text>();

        emptyCellSprite = icon.sprite;
        emptyCellColor = icon.color;

        AllowClickThrough();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
            itemSlotWasClicked = false;
            nReported = 0;
        }
    }

    public Item CurrentItem {
        get {
            return item;
        }
        set {
            item = value;
            if(item != null) {
                icon.sprite = item.itemInfo.icon;
                icon.color = Color.white;
                if(item.itemInfo.GetType() != typeof(Equipable))
                    stackAmount.text = "" + item.stackAmount;
                else
                    stackAmount.text = "";
            } else {
                icon.sprite = emptyCellSprite;
                icon.color = emptyCellColor;
                stackAmount.text = "";
            }
        }
    }

    public void AllowClickThrough() {
        panel.raycastTarget = false;
        icon.raycastTarget = false;
        stackAmount.raycastTarget = false;
    }

    public void Hide() {
        panel.enabled = false;
        icon.enabled = false;
        stackAmount.enabled = false;
    }

    public void Show() {
        panel.enabled = true;
        icon.enabled = true;
        stackAmount.enabled = true;
    }

    public void ReportSlotClicked() {
        itemSlotWasClicked = true;
        nReported++;
    }

    public void ReportSlotNotClicked() {
        nReported++;
        int total = GetTotalInventorySpace();
        if(nReported == total && !itemSlotWasClicked) {
            //Debug.Log("Discard");
            Discard();
        }
    }

    private int GetTotalInventorySpace() {
        int total = 0;
        GameObject[] invObjs = GameObject.FindGameObjectsWithTag("InventoryUI");
        
        foreach(GameObject invObj in invObjs) {
            InventoryUI invUI = invObj.GetComponent<InventoryUI>();
            total += invUI.GetInventorySpace();
        }
        return total;
    }

    public void Discard() {
        if(CurrentItem != null) {

            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            GameObject discardBox = Instantiate(discardBoxPref, player.position + player.forward * 2f, Quaternion.identity);
            InventoryController boxIC = discardBox.GetComponent<InventoryController>();
            boxIC.ChangeCapacity(1);
            boxIC.Insert(CurrentItem, 0);
        }

        CurrentItem = null;
        Hide();
    }
}
