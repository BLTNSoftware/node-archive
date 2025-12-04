using DG.Tweening;
using TMPro;
using UnityEngine;

public class ComboUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private bool hideWhenZero = true;

    public void OnScoreChanged(int score, int combo)
    {
        if (hideWhenZero && combo <= 1)
        {
            comboText.text = "";

            return;
        }
        comboText.rectTransform.DOPunchScale(
Vector3.one * 0.25f,
0.25f,
10,
0.6f);

        comboText.DOColor(Color.yellow, 0.1f).OnComplete(() =>
        {
            comboText.DOColor(Color.white, 0.3f);
        });
        comboText.text = $"Combo x{combo}";
    }
}
