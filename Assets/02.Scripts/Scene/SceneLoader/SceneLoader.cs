using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private CanvasGroup _sceneLoaderCanvasGroup;
    [SerializeField] private Image _progressBar;
    [SerializeField] private CanvasGroup _gameLoadCompletedUi;
    private bool _complete = false;
    //게임켜면 미리 준비하고 있는 로딩 나중에 만들어 보자
    private AsyncOperation _ao;



    private string _loadSceneName;

    private void Awake()
    {
        if(Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += LoadSceneEnd;
        _loadSceneName = sceneName;
        StartCoroutine(Load(sceneName));
    }

    private IEnumerator Load(string sceneName)
    {
        _progressBar.fillAmount = 0f;
        //이 코루틴이 끝날때 까지 대기
        yield return StartCoroutine(Fade(true,_sceneLoaderCanvasGroup));

        _ao = SceneManager.LoadSceneAsync(sceneName);
        _ao.allowSceneActivation = false;

        float timer = 0.0f;
        while(!_ao.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            if(_ao.progress < 0.9f)
            {
                _progressBar.fillAmount = Mathf.Lerp(_progressBar.fillAmount, _ao.progress, timer);
                if(_progressBar.fillAmount >= _ao.progress)
                {
                    timer = 0.0f;
                }
            }
            else
            {
                _progressBar.fillAmount = Mathf.Lerp(_progressBar.fillAmount, 1f, timer);
                if(_progressBar.fillAmount == 1.0f)
                {
                    yield return StartCoroutine(Fade(true, _gameLoadCompletedUi));
                    _complete = true;
                    yield break;
                }
            }
        }
    }

    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.name == _loadSceneName)
        {
            StartCoroutine(Fade(false, _sceneLoaderCanvasGroup));
            StartCoroutine(Fade(false, _gameLoadCompletedUi));
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }
    private IEnumerator Fade(bool isFadeIn, CanvasGroup target)
    {
        float timer = 0.0f;

        while(timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            target.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
        }

        if(!isFadeIn)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnButtonClick()
    {
        if(_complete)
        {
            _ao.allowSceneActivation = true;
        }
    }

    #region asyncSceneLoad
    //Custom by siko 
    private IEnumerator PrepareScene()
    {
        //잠시 기다려줘야 한다 어웨이크에서 호출시 오류 발생
        yield return new WaitForSeconds(.1f);

        _ao = SceneManager.LoadSceneAsync("01.Scenes/MainGame");
        _ao.allowSceneActivation = false;

    }
    private void LoadWhenSceneIsPrepared()
    {
        _ao.allowSceneActivation = true;
    }
    #endregion
}
