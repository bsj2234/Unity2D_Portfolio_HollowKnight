using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class StatusUi : MonoBehaviour
{
    [SerializeField] private FalseKnight _character;
    [SerializeField] private TMP_Text _textMesh;

    private void Awake()
    {

        if (GameManager.Instance.StatueDebug == false)
        {
            gameObject.SetActive(false);
            return;
        }
        Assert.IsNotNull(_character);

        _character.OnStatusChange += UpdateUi;
        _character.OnFlip += Flip;
    }

    private void Start()
    {
        UpdateUi(_character);
    }

    private void UpdateUi(FalseKnight character)
    {
        StringBuilder result = new StringBuilder();
        result.Append("HP: ");
        result.AppendLine(character.GetHp().ToString());
        result.Append("Phase: ");
        result.AppendLine(character.GetPhase());

        _textMesh.text = result.ToString();
    }
    private void Flip()
    {
        transform.rotation = Quaternion.Euler(0f,0f,0f);
    }





}
