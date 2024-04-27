using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUi : MonoBehaviour
{
    [SerializeField] private GameObject PauseUiCanvas;

    private bool _activeUi = false;
    public bool ActiveUi 
    { 
        get 
        { return _activeUi; }
        set 
        {
            _activeUi = value;
            if(value)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
        } 
    }
    private void OnEnable()
    {
        Pause();
    }
    public void Pause()
    {
        GameManager.Instance.PauseGame();
        ActiveUi = true;
    }
    public void Resume()
    {
        GameManager.Instance.ResumeGame();
        UiManager.Instance.ExitUiMode();
        ActiveUi = false;
    }
    public void Restart()
    {
        GameManager.Instance.ResumeGame();
        SceneLoader.Instance.LoadScene("MainGame");
        ActiveUi = false;
    }
    public void Quit()
    {
        GameManager.Instance.ResumeGame();
        GameManager.Instance.QuitGame();
    }
}
