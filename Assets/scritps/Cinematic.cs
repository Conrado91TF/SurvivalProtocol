using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class Cinematic : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public string nivel2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playableDirector.stopped += OnTimelineFinished;
    }

    void OnTimelineFinished(PlayableDirector pd)
    {
        SceneManager.LoadScene(nivel2);
        
    }
     
}
