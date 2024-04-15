using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManuSceneManager : MonoBehaviour
{

    //�����Ѹ� �̸� �غ��ϰ� �ִ� �ε� �ε����� ���߿� ����� ����
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
