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
        GameManager.Instance.QuitGame();
    }
}
