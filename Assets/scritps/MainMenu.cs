using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;

    public void OpenOptionsPanel()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OpenMainMenuPanel()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Nivel 1");
    }

}
