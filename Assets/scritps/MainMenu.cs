using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;

    [Header("Música")]
    
    public Slider volumeSlider;      

    private void Start()
    {
        if (volumeSlider != null)
        {
            // Cargar volumen guardado
            volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    public void OnVolumeChanged(float value)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetVolume(value);
    }


    public void OpenOptionsPanel()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);

        //Sincronizar el slider con el volumen actual
        if (volumeSlider != null && MusicManager.Instance != null)
            volumeSlider.value = MusicManager.Instance.GetVolume();
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
