using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private BoardManager boardManager;
    [SerializeField] private CardSelectionController cardSelectionController;
    [SerializeField] private float previewDuration = 5f;

    [SerializeField] private BoardConfig defaultBoardConfig;
    [SerializeField] private BoardConfig[] allBoardConfigs;

    private const string SaveKey = "CARD_MATCH_SAVE";
    public static GameManager Instance { get; internal set; }

    private void Awake()
    {
        Instance = this;
        bool loadedFromSave = LoadOrStartNewGame();

        // Only run preview for new games
        if (!loadedFromSave)
        {
            StartCoroutine(PreviewSequence());
        }
    }

    private void OnEnable()
    {

        cardSelectionController.OnPairResolved.AddListener(HandlePairResolved);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnDisable()
    {
        cardSelectionController.OnPairResolved.RemoveListener(HandlePairResolved);
    }

    [ContextMenu("Clear Saved Game")]
    public void ClearSavedGame()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }

    public void StartNewGame(BoardConfig config)
    {
        ClearSavedGame();

        boardManager.GenerateBoard(config, null);
        cardSelectionController.UnsubscribeFromCards();
        cardSelectionController.SubscribeToCards();
        StartCoroutine(PreviewSequence());

        UIManager.Instance.HideGameOverScreen();
    }


    private void HandlePairResolved(CardView a, CardView b, bool isMatch)
    {
        // Later: scoring logic goes here.
        SaveGame();
    }
    // Start is called before the first frame update
    void Start()
    {
        cardSelectionController.OnAllPairsMatched.AddListener(HandleAllPairsMatched);
        //StartCoroutine(PreviewSequence());
    }

    private bool LoadOrStartNewGame()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            var saveData = JsonUtility.FromJson<GameSaveData>(json);

            BoardConfig config = FindBoardConfigById(saveData.boardConfigId);
            if (config == null)
            {
                Debug.LogWarning($"[GameManager] Saved boardConfigId '{saveData.boardConfigId}' not found. Using default.");
                config = defaultBoardConfig;
            }

            boardManager.GenerateBoard(config, saveData);

            cardSelectionController.UnsubscribeFromCards();
            // Ensure selection controller is hooked AFTER board is created
            cardSelectionController.SubscribeToCards();

            cardSelectionController.InitializePendingFromBoardState();

            // Check if this saved game is already completed
            if (IsBoardComplete())
            {
                Debug.Log($"{gameObject.name}: Saved game is already completed. Starting a fresh game.");

                // Clear save so we don't keep loading a finished game
                PlayerPrefs.DeleteKey(SaveKey);
                PlayerPrefs.Save();

                // Start a fresh game with the same config (or default)
                boardManager.GenerateBoard(config, null);
                cardSelectionController.UnsubscribeFromCards();
                cardSelectionController.SubscribeToCards();

                return false; // treat as NEW game (so preview can run)
            }

            return true; // valid loaded game
        }
        else
        {
            boardManager.GenerateBoard(defaultBoardConfig, null);
            cardSelectionController.UnsubscribeFromCards();
            cardSelectionController.SubscribeToCards();

            return false;
        }


        // Start preview routine if you have it
        //StartCoroutine(PreviewSequence());
    }

    private bool IsBoardComplete()
    {
        var cards = boardManager.Cards;
        if (cards == null || cards.Count == 0)
            return false;

        foreach (var card in cards)
        {
            if (card == null)
                continue;

            if (!card.IsMatched)
                return false;
        }

        return true;
    }

    private BoardConfig FindBoardConfigById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        for (int i = 0; i < allBoardConfigs.Length; i++)
        {
            var cfg = allBoardConfigs[i];
            if (cfg != null && cfg.id == id)
                return cfg;
        }
        //foreach (var cfg in allBoardConfigs)
        //{
        //    if (cfg != null && cfg.id == id)
        //        return cfg;
        //}
        return null;
    }

    public void SaveGame()
    {
        Debug.Log($"{gameObject.name}: Saving game...");

        var cards = boardManager.Cards;
        var config = boardManager.Config;

        if (config == null || cards == null || cards.Count == 0)
            return;

        GameSaveData data = new GameSaveData();
        data.boardConfigId = config.id;
        data.rows = config.rows;
        data.columns = config.columns;

        data.cards = new CardSaveData[cards.Count];

        for (int i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            if (card == null)
            {
                data.cards[i] = new CardSaveData();
                continue;
            }

            data.cards[i] = new CardSaveData
            {
                cardId = card.CardId,
                isMatched = card.IsMatched,
                isFaceUp = card.IsFaceUp
            };
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log($"[GameManager] Saved game: {json}");
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
                card.FlipUp(true);
        }

        // Wait for preview time
        yield return new WaitForSeconds(previewDuration);

        for (int i = 0; i < boardManager.Cards.Count; i++)
        {
            var card = boardManager.Cards[i];
            if (card != null)
                card.FlipDown(true);
        }

        //todo: Do we really need to enable/disable the selection controller?
        //selectionController.enabled = true;
    }

}

