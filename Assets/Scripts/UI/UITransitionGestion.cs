using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UITransitionGestion : MonoBehaviour
{
    public Canvas canvas;
    public LevelTransitionManager levelTransitionManager;
    
    [Header("NextLevel")]
    public RectTransform nextLevelUi;

    public RectTransform skipLevelUi;
    public float topPosition;
    public float bottoomPosition;
    public float timeShowUp;
    public float timeWaitUp;
    
    [Header("RestartLevel")]
    public RectTransform restartLevelUi;
    public float timeWaitRestart = 0.2f;
    public float timeGetUpRestart = 0.4f;

    private bool _routineInProgress;
    private InputGestion _inputGestion;
    private bool waitStart;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        canvas.worldCamera = FindObjectOfType<Camera>();
        if (_routineInProgress)
        {
            _inputGestion = FindObjectOfType<InputGestion>();
            _inputGestion.SetListenState(false);
            waitStart = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextLevelTransition()
    {
        if (_routineInProgress)
            return;
        _routineInProgress = true;
        StartCoroutine(INextLevelTransition(nextLevelUi));
    }

    public void SkipLevelUi()
    {
        if (_routineInProgress)
            return;
        _routineInProgress = true;
        StartCoroutine(INextLevelTransition(skipLevelUi));
    }
 
    private void ChangePosition(RectTransform rectTransform, float addY)
    {
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, rectTransform.offsetMax.y - addY);
    }

    private void ResetPosition(RectTransform rectTransform)
    {
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -bottoomPosition);
    }
    IEnumerator INextLevelTransition(RectTransform rectTransform)
    {
        float changePerSecond = Mathf.Abs(topPosition - bottoomPosition) / timeShowUp;
        
        Debug.Log(changePerSecond);

        float timeCounter = 0;
        while (timeCounter <= timeShowUp)
        {
            ChangePosition(rectTransform, -changePerSecond * Time.deltaTime);
            yield return null;
            timeCounter += Time.deltaTime;
        }
        
        yield return new WaitForSeconds(timeWaitUp);
        
        // Load next Level
        levelTransitionManager.ApplyNextLevel();

        yield return null;

        timeCounter = 0;
        while (timeCounter <= timeShowUp)
        {
            ChangePosition(rectTransform, changePerSecond * Time.deltaTime);
            yield return null;
            timeCounter += Time.deltaTime;
        }
        ResetPosition(nextLevelUi);
        _routineInProgress = false;
        if (waitStart)
        {
            _inputGestion.SetListenState(true);
        }
    }

    public void PlayRestardTransition()
    {
        if (_routineInProgress)
            return;
        _routineInProgress = true;
        StartCoroutine(IRestardTransition());
    }
    
    IEnumerator IRestardTransition()
    {
        float changePerSecond = Mathf.Abs(topPosition - bottoomPosition) / timeGetUpRestart;
        

        float timeCounter = 0;
        while (timeCounter <= timeGetUpRestart)
        {
            ChangePosition(restartLevelUi, -changePerSecond * Time.deltaTime);
            yield return null;
            timeCounter += Time.deltaTime;
        }
        
        yield return new WaitForSeconds(timeWaitRestart);
        
        
        timeCounter = 0;
        while (timeCounter <= timeGetUpRestart)
        {
            ChangePosition(restartLevelUi, changePerSecond * Time.deltaTime);
            yield return null;
            timeCounter += Time.deltaTime;
        }
        ResetPosition(restartLevelUi);
        _routineInProgress = false;
    }
}
