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

    public bool CheatMode = false;

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

    public void SwitchCheatMode()
    {
        CheatMode = !CheatMode;

        BoardManager.Instance.SetCheatMode(CheatMode);
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
        CheatMode = false;

        boardManager.GenerateBoard(config, null);
        cardSelectionController.UnsubscribeFromCards();
        cardSelectionController.SubscribeToCards();
        ScoreSystem.Instance.ResetScore();
        StartCoroutine(PreviewSequence());

        UIManager.Instance.HideGameOverScreen();
    }


    private void HandlePairResolved(CardView cardA, CardView cardB, bool isMatch)
    {
        if (isMatch)
        {
            ScoreSystem.Instance.OnMatch();
            AudioManager.Instance.PlayMatch();
        }
        else
        {
            ScoreSystem.Instance.OnMismatch();
            AudioManager.Instance.PlayMismatch();
        }

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
                Debug.LogWarning($"{gameObject.name}: Saved boardConfigId '{saveData.boardConfigId}' not found. Using default.");
                config = defaultBoardConfig;
            }

            boardManager.GenerateBoard(config, saveData);

            cardSelectionController.UnsubscribeFromCards();
            // Ensure selection controller is hooked AFTER board is created
            cardSelectionController.SubscribeToCards();

            cardSelectionController.InitializePendingFromBoardState();

            ScoreSystem.Instance.SetFromSave(saveData.score, saveData.combo);

            // Check if this saved game is already completed
            if (IsBoardComplete())
            {
                Debug.Log($"{gameObject.name}: Saved game is already completed. Starting a fresh game.");

                PlayerPrefs.DeleteKey(SaveKey);
                PlayerPrefs.Save();

                boardManager.GenerateBoard(config, null);
                cardSelectionController.UnsubscribeFromCards();
                cardSelectionController.SubscribeToCards();

                return false; // treat as NEW game, so preview can run!
            }

            return true; // valid loaded game
        }
        else
        {
            boardManager.GenerateBoard(defaultBoardConfig, null);
            cardSelectionController.UnsubscribeFromCards();
            cardSelectionController.SubscribeToCards();

            ScoreSystem.Instance.ResetScore();

            return false;
        }

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

        data.score = ScoreSystem.Instance.score;
        data.combo = ScoreSystem.Instance.Combo;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log($"[GameManager] Saved game: {json}");
    }

    private void HandleAllPairsMatched()
    {
        UIManager.Instance.ShowGameOverScreen();
        AudioManager.Instance.PlayGameOver();
        ScoreSystem.Instance.OnGameComplete();
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

