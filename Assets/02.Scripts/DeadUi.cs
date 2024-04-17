using System;
using UnityEngine;

public class DeadUi : MonoBehaviour
{
    private Animator animator;
    public GameObject canvas;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void DeadUiOn()
    {
        canvas.SetActive(true);
        animator.SetTrigger("On");
    }
    public void DeadUiOff()
    {
        canvas.SetActive(false);
        animator.SetTrigger("Off");
    }

}
