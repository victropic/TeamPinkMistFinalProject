using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsButtons : MonoBehaviour
{
    public CanvasGroup mainPanel;
    public CanvasGroup controlsPanel;

    public void ShowMain() {
        mainPanel.interactable = true;
        mainPanel.blocksRaycasts = true;
        mainPanel.alpha = 1;

        controlsPanel.interactable = false;
        controlsPanel.blocksRaycasts = false;
        controlsPanel.alpha = 0;
    }

    public void ShowControls() {
        mainPanel.interactable = false;
        mainPanel.blocksRaycasts = false;
        mainPanel.alpha = 0;

        controlsPanel.interactable = true;
        controlsPanel.blocksRaycasts = true;
        controlsPanel.alpha = 1;
    }
}
