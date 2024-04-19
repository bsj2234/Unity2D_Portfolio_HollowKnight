using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private CanvasGroup _sceneLoaderCanvasGroup;
    [SerializeField] private Image progressBar;
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
        progressBar.fillAmount = 0f;
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while(!op.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            if(op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if(progressBar.fillAmount >= op.progress)
                {
                    timer = 0.0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if(progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.name == _loadSceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }
    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0.0f;

        while(timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            _sceneLoaderCanvasGroup.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
        }

        if(!isFadeIn)
        {
            gameObject.SetActive(false);
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
