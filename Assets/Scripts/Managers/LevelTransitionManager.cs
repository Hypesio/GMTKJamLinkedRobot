using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionManager : MonoBehaviour
{
    static LevelTransitionManager _instance;
    public float timeCelebration = 2f;
    public AudioSource audioSource; 
    
    public UITransitionGestion uiTransitionGestion;
    private int _actualLevel;
    private void Awake()
    {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }

        
    }


    public void NextLevel()
    {
        _actualLevel += 1;
        StartCoroutine(IChangeLevelEffect());
    }

    public void ApplyNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void EndOfGame()
    {
        Debug.Log("Finish game");
    }

    public void SkipLevel()
    {
        uiTransitionGestion.SkipLevelUi();
    }

    public void PlayRestartTransition()
    {
        uiTransitionGestion.PlayRestardTransition();
    }

    IEnumerator IChangeLevelEffect()
    {
        audioSource.Play();
        yield return new WaitForSeconds(timeCelebration);
        uiTransitionGestion.NextLevelTransition();
    }
}
