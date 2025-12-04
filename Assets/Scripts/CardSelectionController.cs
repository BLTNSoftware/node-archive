using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles gameplay logic for card selection and matching.
/// </summary>
public class CardSelectionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardManager boardManager;

    [Tooltip("How long to keep both cards revealed before resolving.")]
    [SerializeField] private float revealDelay = 0.35f;

    [System.Serializable]
    public class PairResolvedEvent : UnityEvent<CardView, CardView, bool> { }


    [Header("Events")]
    [Tooltip("Invoked when a pair is resolved: (firstCard, secondCard, isMatch)")]
    public PairResolvedEvent OnPairResolved;


    [Tooltip("Invoked when all pairs are matched.")]
    public UnityEvent OnAllPairsMatched;

    // Cards that are currently face-up and not yet resolved
    private readonly List<CardView> _pendingCards = new List<CardView>();

    // Cards currently in a comparison coroutine
    private readonly HashSet<CardView> _inComparison = new HashSet<CardView>();

    private void Start()
    {
        StartCoroutine(DelayedSubscribe());
    }

    private IEnumerator DelayedSubscribe()
    {
        // Wait a frame to ensure BoardManager has initialized its cards
        yield return new WaitForEndOfFrame();
        SubscribeToCards();
    }

    private void OnDisable()
    {
        UnsubscribeFromCards();
    }

    public void SubscribeToCards()
    {
        UnsubscribeFromCards(); // safety

        for (int i = 0; i < boardManager.Cards.Count; i++)
        {
            var card = boardManager.Cards[i];
            if (card == null) continue;
            card.OnCardFlippedUp.AddListener(HandleCardFlippedUp);
        }
    }

    private void UnsubscribeFromCards()
    {

        for(int i = 0; i < _pendingCards.Count; i++)
        {
            var card = _pendingCards[i];
            if (card == null) continue;
            card.OnCardFlippedUp.RemoveListener(HandleCardFlippedUp);
        }

        _pendingCards.Clear();
        _inComparison.Clear();
    }

    /// <summary>
    /// Called by CardView when its flip up animation is complete, and the card is now face up.
    /// </summary>
    private void HandleCardFlippedUp(CardView card)
    {
        Debug.Log($"CardSelectionController: Card flipped up: ID={card.CardId}");
        if (card == null)
            return;

          if (card.IsMatched)
            return;

        if (_inComparison.Contains(card))
            return;

        if (_pendingCards.Contains(card))
            return;

        _pendingCards.Add(card);
        StartNextComparison();
    }

    /// <summary>
    /// Attempts to start comparisons for the earliest two pending cards that are not already being compared.
    /// </summary>
    private void StartNextComparison()
    {
        while (true)
        {
            CardView first = null;
            CardView second = null;

            // Find the first card not in comparison and not matched
            for (int i = 0; i < _pendingCards.Count; i++)
            {
                var currentFirstCard = _pendingCards[i];
                if (currentFirstCard == null) continue;
                if (currentFirstCard.IsMatched) continue;
                if (_inComparison.Contains(currentFirstCard)) continue;

                first = currentFirstCard;
                break;
            }

            if (first == null)
                return;

            // Find the matching card if any!
            for (int i = 0; i < _pendingCards.Count; i++)
            {
                var currentSecondCard = _pendingCards[i];
                if (currentSecondCard == null) continue;
                if (currentSecondCard == first) continue; // skip self
                if (currentSecondCard.IsMatched) continue;
                if (_inComparison.Contains(currentSecondCard)) continue;

                second = currentSecondCard;
                break;
            }

            if (second == null)
                return;

            // At this point we have two valid cards; Let's compare them
            StartCoroutine(ComparePairRoutine(first, second));

            // and now let's loop again to see if there are more pairs we can schedule for comparison.
        }
    }

    /// <summary>
    /// Coroutine that waits briefly, then resolves match/mismatch
    /// for the given pair of cards.
    /// </summary>
    private IEnumerator ComparePairRoutine(CardView first, CardView second)
    {
        _inComparison.Add(first);
        _inComparison.Add(second);

        // Delay so the player can see both cards
        if (revealDelay > 0f)
            yield return new WaitForSeconds(revealDelay);

        bool isMatch = first.CardId == second.CardId;

        if (isMatch)
        {
            first.SetMatched();
            second.SetMatched();
        }
        else
        {
            // Flip them back down.
            first.FlipDown();
            second.FlipDown();
        }

        OnPairResolved?.Invoke(first, second, isMatch);

        // Remove from pending list and comparison set
        _pendingCards.Remove(first);
        _pendingCards.Remove(second);
        _inComparison.Remove(first);
        _inComparison.Remove(second);

        // After a successful match, check if the board is fully solved
        if (isMatch)
            CheckForCompletion();

        // See if we can resolve another pair with remaining pending cards
        StartNextComparison();
    }

    /// <summary>
    /// Checks if all cards in the current board are matched.
    /// If yes, fires OnAllPairsMatched.
    /// </summary>
    private void CheckForCompletion()
    {
        if (boardManager == null || boardManager.Cards == null)
            return;

        for (int i = 0; i < boardManager.Cards.Count; i++)
        {
            var card = boardManager.Cards[i];
            if (card == null) continue;
            if (!card.IsMatched)
                return; // still cards to solve
        }

        OnAllPairsMatched?.Invoke();
    }
}
