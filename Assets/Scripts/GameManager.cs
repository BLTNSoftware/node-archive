using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private BoardManager boardManager;
    [SerializeField] private CardSelectionController selectionController;
    [SerializeField] private float previewDuration = 5f;

    public static GameManager Instance { get; internal set; }

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        selectionController.OnAllPairsMatched.AddListener(HandleAllPairsMatched);
        StartCoroutine(PreviewSequence());
    }

    private void HandleAllPairsMatched()
    {
        UIManager.Instance.ShowGameOverScreen();
    }

    internal void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }




    private IEnumerator PreviewSequence()
    { 
        // wait a few frames to ensure everything is initialized
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        //todo: Do we really need to enable/disable the selection controller?
        //selectionController.enabled = false;

        for (int i = 0; i < boardManager.Cards.Count; i++)
        {
            var card = boardManager.Cards[i];
            if (card != null)
                card.FlipUp(false);
        }

        // Wait for preview time
        yield return new WaitForSeconds(previewDuration);

        for (int i = 0; i < boardManager.Cards.Count; i++)
        {
            var card = boardManager.Cards[i];
            if (card != null)
                card.FlipDown();
        }

        //todo: Do we really need to enable/disable the selection controller?
        //selectionController.enabled = true;
    }

}

