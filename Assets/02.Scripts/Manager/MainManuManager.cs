using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManuManager : MonoBehaviour
{

    public void LoadScene()
    {
        SceneLoader.Instance.LoadScene("MainGame");
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
