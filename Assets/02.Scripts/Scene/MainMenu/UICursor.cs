using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICursor : MonoBehaviour
{
    [SerializeField] private RectTransform _cursorL;
    [SerializeField] private RectTransform _cursorR;

    public float CursorSpeed = 10f;
    public float CursorOffset = 15f;

    private System.Action UpdataPointerUi;
    private RectTransform _cachingTransform;

    private void Update()
    {
        if(UpdataPointerUi != null)
        {
            UpdataPointerUi.Invoke();
        }
    }
    public void OnPointCursor(RectTransform rect)
    {
        if(UpdataPointerUi == null)
        {
            UpdataPointerUi += UpdatePointer;
        }
        _cachingTransform = rect;
    }

    private void UpdatePointer()
    {
        Vector2 posL = new Vector2(_cachingTransform.anchoredPosition.x - (_cachingTransform.rect.width / 2f + CursorOffset), _cachingTransform.anchoredPosition.y);
        Vector2 posR = new Vector2(_cachingTransform.anchoredPosition.x + (_cachingTransform.rect.width / 2f + CursorOffset), _cachingTransform.anchoredPosition.y);

        _cursorL.anchoredPosition = Vector2.Lerp(_cursorL.anchoredPosition, posL, CursorSpeed * Time.deltaTime);
        _cursorR.anchoredPosition = Vector2.Lerp(_cursorR.anchoredPosition, posR, CursorSpeed * Time.deltaTime);
        float distance = (_cursorL.anchoredPosition - _cachingTransform.anchoredPosition).magnitude;
        if (distance < .01f)
        {
            UpdataPointerUi -= UpdatePointer;
            _cachingTransform = null;
        }
    }
}
