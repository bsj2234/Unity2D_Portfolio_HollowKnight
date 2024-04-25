using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetItemAcquiredText : MonoBehaviour
{
    [SerializeField] private Chest chest;
    [SerializeField] private TMPro.TMP_Text _text;
    private void Awake()
    {

        _text = GetComponent<TMPro.TMP_Text>();
    }

    private void OnEnable()
    {
        _text.text = $"{chest.Item.ItemName} ∏¶ »πµÊ«ﬂΩ¿¥œ¥Ÿ.";
    }
}
