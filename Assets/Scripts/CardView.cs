using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;

public class CardView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform cardContainer;

    [Header("Flip settings")]
    [SerializeField] private float flipUpTime = 0.2f;
    [SerializeField] private float flipDownTime = 0.3f;

    // Tiny offsets on Y to force a consistent rotation direction
    private const float FaceDownY = 0.001f;
    private const float FaceUpY = -179.99f;

    [Header("Feedback effect params")]
    [SerializeField] private float punchScaleTime = 0.2f;
    [SerializeField] private float punchScaleSizeFactor = 0.15f;
    [SerializeField] private int punchVibrato = 5;
    [SerializeField] private float punchElasticity = 0.6f;

    [Header("State (Debug)")]
    [SerializeField] private int cardId = 0;
    [SerializeField] private TextMeshProUGUI debugIdText;
    public int CardId => cardId;

    public bool IsAnimating { get; private set; }
    public bool IsMatched { get; private set; }
    public bool IsFaceUp { get; private set; }

    [System.Serializable]
    public class CardFlipEvent : UnityEvent<CardView> { }

    [Tooltip("Invoked when the card finishes flipping face-up.")]
    public CardFlipEvent OnCardFlippedUp;

    private Tween flipTween;

    //private void Reset()
    //{
    //    // Auto-assign if dropped on the prefab
    //    if (cardContainer == null)
    //        cardContainer = GetComponent<RectTransform>();
    //}

    private void Start()
    {
        // Ensure we start face-down in case Init wasn't called yet
        SetToFaceDownImmediate();
        IsMatched = false;
        IsAnimating = false;
    }

    /// <summary>
    /// Called by BoardManager after instantiation to assign the logical ID.
    /// </summary>
    public void Init(int id)
    {
        cardId = id;
        debugIdText.text = id.ToString();
        IsMatched = false;
        SetToFaceDownImmediate();
    }

    /// <summary>
    /// Called from the UI Button on the card.
    /// Only allows flipping UP, logic decides when to flip down.
    /// </summary>
    public void OnClicked()
    {
        if (IsAnimating || IsMatched)
            return;

        // If already face up, ignore.
        if (!IsFaceUp)
            FlipUp();
        
    }

    public void FlipUp(bool callback = true)
    {
        if (IsAnimating || IsMatched || IsFaceUp)
            return;

        IsAnimating = true;
        IsFaceUp = true;
        flipTween?.Kill();

        flipTween = cardContainer
            .DORotate(new Vector3(0f, FaceUpY, 0f), flipUpTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                IsAnimating = false;
                if (callback)
                {
                    OnCardFlippedUp?.Invoke(this);
                }
                //Debug.Log($"Card {cardId} flipped up.");
            });
    }

    public void FlipDown()
    {
        if (IsAnimating || !IsFaceUp)
            return;

        IsAnimating = true;
        IsFaceUp = false;
        flipTween?.Kill();

        flipTween = cardContainer
            .DORotate(new Vector3(0f, FaceDownY, 0f), flipDownTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                IsAnimating = false;
            });
    }

    public void SetMatched()
    {
        IsMatched = true;
        IsFaceUp = true;

        // Feedback punch effect when card is matched.
        cardContainer.DOPunchScale(Vector3.one * punchScaleSizeFactor, punchScaleTime, punchVibrato, punchElasticity);
    }

    public void SetToFaceDownImmediate()
    {
        IsFaceUp = false;
        cardContainer.localRotation = Quaternion.Euler(0f, FaceDownY, 0f);
    }
    public void SetToFaceUpImmediate()
    {
        IsFaceUp = true;
        cardContainer.localRotation = Quaternion.Euler(0f, FaceUpY, 0f);
    }
}
