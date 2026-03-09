using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;

    [Header("Música")]
    public AudioSource musicSource;  // arrastra el AudioSource aquí
    public Slider volumeSlider;      // arrastra tu barra de volumen aquí


    private void Start()
    {
        // Cargar volumen guardado o poner 1 por defecto
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (musicSource != null)
        {
            musicSource.volume = savedVolume;
            musicSource.loop = true;
            musicSource.Play();
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    public void OnVolumeChanged(float value)
    {
        if (musicSource != null)
            musicSource.volume = value;

        // Guardar el volumen para que se recuerde
        PlayerPrefs.SetFloat("MusicVolume", value);
    }


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
