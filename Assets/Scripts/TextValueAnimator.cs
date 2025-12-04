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

    void AnimateFromToValue(int startValue, int endValue)
    {
        
        DOTween.To(() => startValue,
                   x =>
                   {
                       startValue = x;
                       scoreText.text = Mathf.RoundToInt(startValue).ToString();
                   },
                   endValue,
                   AnimationTime).OnComplete(() => 
                   {
                       scoreText.text = endValue.ToString(); //just making sure bro!
                   });
    }

}
