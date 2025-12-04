using UnityEngine;

public class ScoreUIController : MonoBehaviour
{
    [SerializeField] private TextValueAnimator animator;
    private int lastScore = 0;

    public void OnScoreChanged(int newScore, int combo)
    {
        animator.AnimateFromToValue(lastScore, newScore);
        lastScore = newScore;
    }
}