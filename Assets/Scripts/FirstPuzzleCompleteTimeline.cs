using UnityEngine;
using UnityEngine.Playables;

public class FirstPuzzleCompleteTimeline : MonoBehaviour
{
    public PlayableDirector playableDirector;

    private void Awake()
    {
        if (!playableDirector)
        {
            playableDirector = GetComponent<PlayableDirector>();
        }
    }
    
    public void StartTimeline()
    {
        if (playableDirector) playableDirector.Play();
    }
}
