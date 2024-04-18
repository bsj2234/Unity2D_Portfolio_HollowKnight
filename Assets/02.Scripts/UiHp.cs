using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiHp : MonoBehaviour
{
    [SerializeField] private Image[] _images = new Image[10];
    [SerializeField] private Player _character;


    private void OnValidate()
    {
        _images = GetComponentsInChildren<Image>();
    }

    public void UpdateUi()
    {
        int hp = (int)_character.GetCombatComponent().GetHp();
        int maxHp = (int)(_character).maxHp;
        for (int i = 0; i < _images.Length; i++)
        {
            if (i < hp)
            {
                _images[i].enabled = true;
                _images[i].color = Color.white;
            }
            else if(i < maxHp )
            {
                _images[i].enabled = true;
                _images[i].color = Color.black;
            }
            else
            {
                _images[i].enabled = false;
            }
        }
    }
}
