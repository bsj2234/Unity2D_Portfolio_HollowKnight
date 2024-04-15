using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManuSceneManager : MonoBehaviour
{

    //게임켜면 미리 준비하고 있는 로딩 로딩차오 나중에 만들어 보자
    AsyncOperation ao;
    private void Awake()
    {
        StartCoroutine(PrepareScene());
    }


    private IEnumerator PrepareScene()
    {
        yield return new WaitForSeconds(.1f);

        ao = SceneManager.LoadSceneAsync("01.Scenes/MainGame");
        ao.allowSceneActivation = false;
    }
    public void LoadScene()
    {
        ao.allowSceneActivation = true;
    }
}
