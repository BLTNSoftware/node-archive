using UnityEngine;
using UnityEngine.Events;

public class ScoreSystem : MonoBehaviour
{
    [Header("Scoring Rules")]
    public int baseMatchPoints = 100;
    public int mismatchPenalty = 10;

    
    [System.Serializable]
    public class ScoreChangedEvent : UnityEvent<int, int> { }    // Arguments: (newScore, comboMultiplier)

    [Header("Events")]
    public ScoreChangedEvent OnScoreChanged;

    public int score = 0;
    public int combo = 0;

    public int Score => score;
    public int Combo => combo;

    public static ScoreSystem Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ResetScore()
    {
        score = 0;
        combo = 0;
        OnScoreChanged?.Invoke(score, combo);
    }

    public void SetFromSave(int savedScore, int savedCombo)
    {
        score = Mathf.Max(0, savedScore);
        combo = Mathf.Max(0, savedCombo);
        OnScoreChanged?.Invoke(score, combo);   // update UI
    }

    public void OnMatch()
    {
        combo += 1;
        score += baseMatchPoints * combo;

        OnScoreChanged?.Invoke(score, combo);
    }

    public void OnMismatch()
    {
        combo = 0;
        score -= mismatchPenalty;
        if (score < 0) score = 0;

        OnScoreChanged?.Invoke(score, combo);
    }
}
