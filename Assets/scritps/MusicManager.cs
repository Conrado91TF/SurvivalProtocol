using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton que persiste entre escenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

        Debug.Log("MusicManager iniciado | Clip: " + (audioSource.clip != null ? audioSource.clip.name : "NULL") + " | Volumen: " + audioSource.volume);

        // Cargar volumen guardado
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        audioSource.volume = 1f;
        audioSource.loop = true;
        audioSource.Stop();
        audioSource.Play();
        Debug.Log("Reproduciendo música forzado");

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("Reproduciendo música");

        }
    }

    public void SetVolume(float value)
    {
        audioSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public float GetVolume()
    {
        return audioSource.volume;
    }
}