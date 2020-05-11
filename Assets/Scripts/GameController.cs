using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    Player player;
    GameObject currentInteractive;

    /* Input */
    bool openInventory;
    bool exit;
    bool pause;
    bool interact;

    /* UI */
    public InventoryUI handInventory;
    public InventoryUI bagInventory;
    public InventoryUI lootingInventory;
    public CanvasGroup pauseMenu;
    public InteractInfoUI interactUI;
    public CanvasGroup deathScreen;

    bool uiMode;
    
    enum UIType {
        Inventory,
        LootingInventory,
        Pause
    }
    UIType uiType;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        LoadGame();
    }

    // Update is called once per frame
    void Update()
    {

        openInventory = Input.GetButtonDown("Inventory");
        exit = Input.GetButtonDown("Cancel");
        pause = Input.GetButtonDown("Pause");
        interact = Input.GetButtonDown("Interact");

        if(!uiMode) {
            /* Inventory */
            if(openInventory) {
                handInventory.Toggle();
                bagInventory.Toggle();
                EnterUIMode(UIType.Inventory);
            }

            /* Pause */
            if(pause) {
                pauseMenu.alpha = 1;
                pauseMenu.blocksRaycasts = true;
                pauseMenu.interactable = true;

                Time.timeScale = 0f;
                EnterUIMode(UIType.Pause);
            }

            /* Interact */
            if(interact) {
                Interact();
            }

        } else {
            if(exit) {
                LeaveUIMode();
            }
        }
    }

    private void EnterUIMode(UIType uiType) {
        this.uiType = uiType;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        uiMode = true;
    }

    private void LeaveUIMode() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        uiMode = false;

        switch(uiType) {
            case UIType.Inventory:
                handInventory.Toggle();
                bagInventory.Toggle();
                break;

            case UIType.LootingInventory:
                handInventory.Toggle();
                bagInventory.Toggle();
                lootingInventory.Toggle();
                break;

            case UIType.Pause:
                pauseMenu.alpha = 0;
                pauseMenu.blocksRaycasts = false;
                pauseMenu.interactable = false;

                Time.timeScale = 1f;
                break;
        }
    }

    private void Interact() {
        if(currentInteractive != null) {
            if(currentInteractive.GetComponent<InventoryController>()) {
                lootingInventory.inventoryController = currentInteractive.GetComponent<InventoryController>();
                lootingInventory.inventoryName = currentInteractive.GetComponent<InventoryController>().inventoryName;
                handInventory.Toggle();
                bagInventory.Toggle();
                lootingInventory.Toggle();
                EnterUIMode(UIType.LootingInventory);
            } else if(currentInteractive.GetComponent<Interactable>()) {
                Interactable interactable = currentInteractive.GetComponent<Interactable>();
                interactable.Interact();
            }
        }
    }

    public bool UIMode {
        get {
            return uiMode;
        }
    }

    public void SaveGame()
    {
        GameState gameState = new GameState();
        gameState.position = player.transform.position;
        gameState.rotation = player.transform.rotation;
        string json = JsonUtility.ToJson(gameState);
        File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "save.json", json);
    }

    public void LoadGame()
    {
        string fullPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "save.json";
        if(File.Exists(fullPath)) {
            string json = File.ReadAllText(fullPath);

            GameState gameState = new GameState();
            JsonUtility.FromJsonOverwrite(json, gameState);
            Debug.Log(gameState.position.ToString());

            StartCoroutine(player.GetComponent<FPSController>().FreezeForShortTime());
            player.transform.position = gameState.position;
            player.transform.rotation = gameState.rotation;
        }
    }

    public GameObject CurrentInteractive {
        get {
            return currentInteractive;
        }
        set {
            currentInteractive = value;
        }
    }

    public void ShowDeathSequence() {
        StartCoroutine(FadeInDeathScreen());
    }

    private IEnumerator FadeInDeathScreen() {
        int numFrames = 10;

        for(int i = 1; i <= numFrames; i++) {
            yield return new WaitForSeconds(0.05f);
            deathScreen.alpha = ((float)i)/numFrames;
        }

        yield return new WaitForSeconds(1f);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0, LoadSceneMode.Single);

    }
}
