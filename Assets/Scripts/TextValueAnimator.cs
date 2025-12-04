using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextValueAnimator : MonoBehaviour
{

    TextMeshProUGUI scoreText;
    public float AnimationTime = 2f;

    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    [ContextMenu("Test Anim")]
    public void TestAnimation()
    {
        AnimateFromToValue(0, 100);
    }

    public void AnimateFromToValue(int startValue, int endValue)
    {
        if (endValue < 1)
        {
            scoreText.text = "0"; // no animation needed
            return;
        }
        DOTween.To(() => startValue,
                   x =>
                   {
                       startValue = x;
                       scoreText.text = Mathf.RoundToInt(startValue).ToString();
                   },
                   endValue,
                   AnimationTime).OnComplete(() =>
                   {
                       scoreText.text = endValue.ToString(); //just making sure brah!
                   });


        Color color = Color.green;
        if (endValue < startValue)
        {
            color = Color.red;
        }
        scoreText.DOColor(color, 0.1f).OnComplete(() =>
        {
            scoreText.DOColor(Color.white, 0.3f);
        });
    }

}
