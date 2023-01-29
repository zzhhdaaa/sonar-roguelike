using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Michsky.UI.Reach;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private ButtonManager continueButton;

    private void Start()
    {
        if (!SaveManager.instance.HasSaveAvailable())
        {
            continueButton.isInteractable = false;
        }
        else
        {
            eventSystem.SetSelectedGameObject(continueButton.gameObject);
        }
    }

    public void NewGame()
    {
        if (SaveManager.instance.HasSaveAvailable())
        {
            SaveManager.instance.DeleteSave();
        }

        SaveManager.instance.CurrentFloor = 1;
        SceneManager.LoadScene("Dungeon");
    }

    public void ContinueGame()
    {
        SaveManager.instance.LoadGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}