using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Card : MonoBehaviour
{


    [SerializeField] RectTransform cardContainer;

    [SerializeField] float flipUpTime = 0.2f;
    [SerializeField] float flipDownTime = 0.3f;
    [SerializeField] float punchScaleTime = 0.2f;
    [SerializeField] float punchScaleSizeFactor = 0.15f;
    [SerializeField] int punchVibrato = 5;
    [SerializeField] float punchElasticity = 0.6f;

    public int CardId = 0;
    public bool IsAnimating { get; private set; }

    public bool IsMatched = false;


    public bool IsFaceUp = false;

    Tween flipTween;

    private void Start()
    {
        Init(CardId);
    }

    public void Init(int id)
    {
        CardId = id;
        ShowBackImmediate();
    }

    public void OnClicked()
    {
        if (IsFaceUp)
        {
            FlipDown();

        }
        else
        {

            FlipUp();
        }
    }

 

    public void FlipUp()
    {
        IsAnimating = true;
        IsFaceUp = true;
        flipTween?.Kill();

        flipTween = cardContainer
            .DORotate(new Vector3(0, -179.99f, 0), flipUpTime)// forcing a tiny offset to avoid gimbal lock and ensure correct rotation direction
            .OnComplete(() =>
            {
                IsAnimating = false;

            });



    }

    public void FlipDown()
    {
        IsFaceUp = false;
        IsAnimating = true;
        flipTween?.Kill();

        flipTween = cardContainer
            .DORotate(new Vector3(0, 0.001f, 0), flipDownTime)// forcing a tiny offset to avoid gimbal lock and ensure correct rotation direction
            .OnComplete(() =>
            {
                IsAnimating = false;

            });

    }

    public void SetMatched()
    {
        IsMatched = true;

        cardContainer.DOPunchScale(Vector3.one * punchScaleSizeFactor, punchScaleTime, punchVibrato, punchElasticity);
    }

    void ShowFrontImmediate()
    {

        IsFaceUp = true;
        cardContainer.localRotation = Quaternion.Euler(0, -179.99f, 0);
    }

    void ShowBackImmediate()
    {
        IsFaceUp = false;
        cardContainer.localRotation = Quaternion.Euler(0, 0.001f, 0);
    }
}