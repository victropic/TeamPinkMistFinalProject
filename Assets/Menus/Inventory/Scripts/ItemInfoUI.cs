using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{

    RectTransform rectTransform;
    Image panel;
    Text nameText;
    Text descriptionText;

    Item item;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        panel = GetComponent<Image>();
        nameText = transform.GetChild(0).GetComponent<Text>();
        descriptionText = transform.GetChild(1).GetComponent<Text>();

        AllowClickThrough();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition = new Vector2(Input.mousePosition.x + 20f, Input.mousePosition.y);
        
        if(Input.mousePosition.y < Screen.height/2f) {
            rectTransform.pivot = new Vector2(0f, 0f);
        } else {
            rectTransform.pivot = new Vector2(0f, 1f);
        }
    }

    public Item CurrentItem {
        get {
            return item;
        }
        set {
            item = value;
            if(item != null) {
                nameText.text = item.itemInfo.itemName;
                descriptionText.text = item.itemInfo.description;
                if(item.itemInfo.GetType() == typeof(Consumable)) {
                    descriptionText.text += "(Shift-Click to use)";
                }

            } else {
                nameText.text = "";
                descriptionText.text = "";
            }
        }
    }

    public void Show() {
        panel.enabled = true;
        nameText.enabled = true;
        descriptionText.enabled = true;
    }

    public void Hide() {
        panel.enabled = false;
        nameText.enabled = false;
        descriptionText.enabled = false;
    }

    public void AllowClickThrough() {
        panel.raycastTarget = false;
        nameText.raycastTarget = false;
        descriptionText.raycastTarget = false;
    }
}
