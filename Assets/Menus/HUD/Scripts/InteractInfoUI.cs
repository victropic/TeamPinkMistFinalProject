using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractInfoUI : MonoBehaviour
{

    CanvasGroup canvasGroup;
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        text = GetComponentInChildren<Text>();
    }

    public void Show() {
        canvasGroup.alpha = 1f;
    }

    public void Hide() {
        canvasGroup.alpha = 0f;
    }
}
