using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Michsky.UI.Reach;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private bool isMenuOpen = false;
    [SerializeField] private TextMeshProUGUI dungeonFloorText;

    [Header("Health UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpSliderText;

    [Header("Message UI")]
    [SerializeField] private int sameMessageCount = 1;
    [SerializeField] private string lastMessage;
    [SerializeField] private bool isMessageHistoryOpen = false;
    [SerializeField] private GameObject messageHistory;
    [SerializeField] private GameObject messageHistoryContent;
    [SerializeField] private GameObject lastFiveMessages;
    [SerializeField] private GameObject lastFiveMessagesContent;

    [Header("Inventory UI")]
    [SerializeField] private bool isInventoryOpen = false;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject inventoryContent;

    [Header("Drop Menu UI")]
    [SerializeField] private bool isDropMenuOpen = false;
    [SerializeField] private GameObject dropMenu;
    [SerializeField] private GameObject dropMenuContent;

    [Header("Instructions UI")]
    [SerializeField] private GameObject instructions;

    [Header("Escape Menu UI")]
    [SerializeField] private bool isEscapeMenuOpen = false; //Read-only
    [SerializeField] private GameObject escapeMenu;

    [Header("Reborn Menu UI")]
    [SerializeField] private bool isRebornMenuOpen = false; //Read-only
    [SerializeField] private GameObject rebornMenu;

    [Header("Character Information Menu UI")]
    [SerializeField] private bool isCharacterInformationMenuOpen = false; //Read-only
    [SerializeField] private GameObject characterInformationMenu;

    [Header("Level Up Menu UI")]
    [SerializeField] private bool isLevelUpMenuOpen = false; //Read-only
    [SerializeField] private GameObject levelUpMenu;
    [SerializeField] private GameObject levelUpMenuContent;

    public bool IsMenuOpen { get => isMenuOpen; }
    public bool IsMessageHistoryOpen { get => isMessageHistoryOpen; }
    public bool IsInventoryOpen { get => isInventoryOpen; }
    public bool IsDropMenuOpen { get => isDropMenuOpen; }
    public bool IsEscapeMenuOpen { get => isEscapeMenuOpen; }
    public bool IsCharacterInformationMenuOpen { get => isCharacterInformationMenuOpen; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetDungeonFloorText(SaveManager.instance.CurrentFloor);

        if (SaveManager.instance.Save.SavedFloor is 0)
        {
            AddMessage("Hello and welcome to the dungeon! Move to activate your sonar.", "#0da2ff"); //Light blue
        }
        else
        {
            AddMessage("Welcome back, adventurer!", "#0da2ff"); //Light blue
        }
    }

    public void SetHealthMax(int maxHp)
    {
        hpSlider.maxValue = maxHp;
    }

    public void SetHealth(int hp, int maxHp)
    {
        hpSlider.value = hp;
        hpSliderText.text = $"HEALTH {hp}/{maxHp}";
    }

    public void SetDungeonFloorText(int floor)
    {
        dungeonFloorText.text = $"DUNGEON FLOOR: {floor}";
    }

    public void ToggleMenu()
    {
        if (isMenuOpen)
        {
            isMenuOpen = !isMenuOpen;

            switch (true)
            {
                case bool _ when isMessageHistoryOpen:
                    ToggleMessageHistory();
                    break;
                case bool _ when isInventoryOpen:
                    ToggleInventory();
                    break;
                case bool _ when isDropMenuOpen:
                    ToggleDropMenu();
                    break;
                case bool _ when isEscapeMenuOpen:
                    ToggleEscapeMenu();
                    break;
                case bool _ when isRebornMenuOpen:
                    ToggleRebornMenu();
                    break;
                case bool _ when isCharacterInformationMenuOpen:
                    ToggleCharacterInformationMenu();
                    break;
                default:
                    break;
            }
        }
    }

    public void ToggleMessageHistory()
    {
        isMessageHistoryOpen = !messageHistory.activeSelf;
        messageHistory.SetActive(isMessageHistoryOpen);
        lastFiveMessages.SetActive(!isMessageHistoryOpen);
    }

    public void ToggleInventory(Actor actor = null)
    {
        inventory.SetActive(!inventory.activeSelf);
        instructions.SetActive(!instructions.activeSelf);
        isMenuOpen = inventory.activeSelf;
        isInventoryOpen = inventory.activeSelf;

        if (isMenuOpen)
        {
            UpdateMenu(actor, inventoryContent);
        }
    }

    public void ToggleDropMenu(Actor actor = null)
    {
        dropMenu.SetActive(!dropMenu.activeSelf);
        instructions.SetActive(!instructions.activeSelf);
        isMenuOpen = dropMenu.activeSelf;
        isDropMenuOpen = dropMenu.activeSelf;

        if (isMenuOpen)
        {
            UpdateMenu(actor, dropMenuContent);
        }
    }

    public void ToggleEscapeMenu()
    {
        escapeMenu.SetActive(!escapeMenu.activeSelf);
        isMenuOpen = escapeMenu.activeSelf;
        isEscapeMenuOpen = escapeMenu.activeSelf;

        eventSystem.SetSelectedGameObject(escapeMenu.transform.GetChild(0).gameObject);
    }
    public void ToggleRebornMenu()
    {
        rebornMenu.SetActive(!rebornMenu.activeSelf);
        isMenuOpen = rebornMenu.activeSelf;
        isRebornMenuOpen = rebornMenu.activeSelf;

        eventSystem.SetSelectedGameObject(rebornMenu.transform.GetChild(0).gameObject);
    }

    public void ToggleLevelUpMenu(Actor actor)
    {
        isLevelUpMenuOpen = !isLevelUpMenuOpen;
        SetBooleans(levelUpMenu, isLevelUpMenuOpen);

        GameObject constitutionButton = levelUpMenuContent.transform.GetChild(0).gameObject;
        GameObject strengthButton = levelUpMenuContent.transform.GetChild(1).gameObject;
        GameObject agilityButton = levelUpMenuContent.transform.GetChild(2).gameObject;
        GameObject sonarButton = levelUpMenuContent.transform.GetChild(3).gameObject;

        constitutionButton.GetComponent<ButtonManager>().buttonText = $"+ 20 HP";
        strengthButton.GetComponent<ButtonManager>().buttonText = $"+ 1 attack";
        agilityButton.GetComponent<ButtonManager>().buttonText = $"+ 1 defense";
        sonarButton.GetComponent<ButtonManager>().buttonText = $"+ 5 sonar Radius";

        foreach (Transform child in levelUpMenuContent.transform)
        {
            child.GetComponent<ButtonManager>().onClick.RemoveAllListeners();

            child.GetComponent<ButtonManager>().onClick.AddListener(() => {
                if (constitutionButton == child.gameObject)
                {
                    actor.GetComponent<Level>().IncreaseMaxHp();
                }
                else if (strengthButton == child.gameObject)
                {
                    actor.GetComponent<Level>().IncreasePower();
                }
                else if (agilityButton == child.gameObject)
                {
                    actor.GetComponent<Level>().IncreaseDefense();
                }
                else if (sonarButton == child.gameObject)
                {
                    actor.GetComponent<Level>().IncreaseSonar();
                }
                else
                {
                    actor.GetComponent<Level>().IncreaseSonar();
                    Debug.LogError("No button found!");
                }
                ToggleLevelUpMenu(actor);
            });
        }

        GameManager.instance.Actors[0].GetComponent<Player>().StartCoroutine(GameManager.instance.Actors[0].GetComponent<Player>().WaitToBorn(0.2f));
        eventSystem.SetSelectedGameObject(levelUpMenuContent.transform.GetChild(0).gameObject);
    }

    public void ToggleCharacterInformationMenu(Actor actor = null)
    {
        isCharacterInformationMenuOpen = !isCharacterInformationMenuOpen;
        SetBooleans(characterInformationMenu, isCharacterInformationMenuOpen);

        if (actor is not null)
        {
            characterInformationMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Level: {actor.GetComponent<Level>().CurrentLevel}";
            characterInformationMenu.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"XP: {actor.GetComponent<Level>().CurrentXp}";
            characterInformationMenu.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"XP for next level: {actor.GetComponent<Level>().XpToNextLevel}";
            characterInformationMenu.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"Attack: {actor.GetComponent<Fighter>().Power}";
            characterInformationMenu.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = $"Defense: {actor.GetComponent<Fighter>().Defense}";
        }
    }

    private void SetBooleans(GameObject menu, bool menuBool)
    {
        isMenuOpen = menuBool;
        menu.SetActive(menuBool);
    }

    public void Save()
    {
        SaveManager.instance.SaveGame(false);
        AddMessage("Information collected.", "#0da2ff");
    }

    public void Load()
    {
        SaveManager.instance.LoadGame();
        AddMessage("Successfully loaded.", "#0da2ff");
        ToggleMenu();
    }

    public void Reborn()
    {
        SaveManager.instance.SaveGame();

        if (SaveManager.instance.HasSaveAvailable())
        {
            SaveManager.instance.DeleteSave();
        }

        SaveManager.instance.CurrentFloor = 1;

        GameManager.instance.Reset(true);
        MapManager.instance.GenerateDungeon(true);

        UIManager.instance.AddMessage("Reborn.", "#0da2ff");
        UIManager.instance.SetDungeonFloorText(SaveManager.instance.CurrentFloor);

        ToggleMenu();

        SonarManager.instance.SonarDownGrade?.Invoke();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void AddMessage(string newMessage, string colorHex)
    {
        if (lastMessage == newMessage)
        {
            TextMeshProUGUI messageHistoryLastChild = messageHistoryContent.transform.GetChild(messageHistoryContent.transform.childCount - 1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lastFiveMessagesLastChild = lastFiveMessagesContent.transform.GetChild(lastFiveMessagesContent.transform.childCount - 1).GetComponent<TextMeshProUGUI>();
            sameMessageCount += 1;
            messageHistoryLastChild.text = $"{newMessage} (x{sameMessageCount+1})";
            lastFiveMessagesLastChild.text = $"{newMessage} (x{sameMessageCount+1})";
            return;
        }
        else if (sameMessageCount > 0)
        {
            sameMessageCount = 0;
        }

        lastMessage = newMessage;

        TextMeshProUGUI messagePrefab = Instantiate(Resources.Load<TextMeshProUGUI>("UI/Message")) as TextMeshProUGUI;
        messagePrefab.text = newMessage;
        messagePrefab.color = GetColorFromHex(colorHex);
        messagePrefab.transform.SetParent(messageHistoryContent.transform, false);

        for (int i = 0; i < lastFiveMessagesContent.transform.childCount; i++)
        {
            if (messageHistoryContent.transform.childCount - 1 < i)
            {
                return;
            }

            TextMeshProUGUI messageHistoryChild = messageHistoryContent.transform.GetChild(messageHistoryContent.transform.childCount - 1 - i).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lastFiveMessagesChild = lastFiveMessagesContent.transform.GetChild(lastFiveMessagesContent.transform.childCount - 1 - i).GetComponent<TextMeshProUGUI>();
            lastFiveMessagesChild.text = messageHistoryChild.text;
            lastFiveMessagesChild.color = messageHistoryChild.color;
        }
    }

    private Color GetColorFromHex(string v)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(v, out color))
        {
            return color;
        }
        else
        {
            Debug.Log("GetColorFromHex: Could not parse color from string.");
            return Color.white;
        }
    }

    private void UpdateMenu(Actor actor, GameObject menuContent)
    {
        for (int resetNum = 0; resetNum < menuContent.transform.childCount; resetNum++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(resetNum).gameObject;
            menuContentChild.GetComponent<ButtonManager>().buttonText = "";
            menuContentChild.GetComponent<ButtonManager>().onClick.RemoveAllListeners();
            menuContentChild.SetActive(false);
        }

        for (int itemNum = 0; itemNum < actor.Inventory.Items.Count; itemNum++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(itemNum).gameObject;
            Item item = actor.Inventory.Items[itemNum];
            menuContentChild.GetComponent<ButtonManager>().buttonText = $"({itemNum+1}) {item.name}";
            menuContentChild.GetComponent<ButtonManager>().onClick.AddListener(() =>
            {
                if (menuContent == inventoryContent)
                {
                    Action.UseAction(actor, item);
                }
                else if (menuContent == dropMenuContent)
                {
                    Action.DropAction(actor, actor.Inventory.Items[itemNum - 1]);
                }
                UpdateMenu(actor, menuContent);
            });
            menuContentChild.SetActive(true);
        }
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(menuContent.transform.GetChild(0).gameObject);
    }
}
