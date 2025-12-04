using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{

    public static BoardManager Instance { get; private set; }

    [SerializeField] private BoardConfig currentConfig;

    
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    
    [SerializeField] private CardView cardPrefab;

    
    [SerializeField] private Vector2 spacing = new Vector2(10f, 10f);
    [SerializeField] private RectOffset padding;

    // list of all cards on the current board
    private readonly List<CardView> _cards = new List<CardView>();

    public IReadOnlyList<CardView> Cards => _cards;
    public BoardConfig Config => currentConfig;
    [SerializeField] private CardTheme cardTheme;
    private void Awake()
    {
        Instance = this;       
    }


    private void Start()
    {
        //if (currentConfig != null)
        //    GenerateBoard(currentConfig);
        //else
        //    Debug.LogWarning($"{gameObject.name}: No BoardConfig assigned.");
    }

    /// <summary>
    /// Public entry point to build a new board from a ScriptableObject config.
    /// </summary>
    /// <param name="config"></param>
    public void GenerateBoard(BoardConfig config)
    {
        if (config == null)
        {
            Debug.LogError($"{gameObject.name}: Cannot generate board; config is null.");
            return;
        }

        currentConfig = config;

        ClearBoard();
        ConfigureGrid(config.rows, config.columns);
        CreateCards(config.rows, config.columns);
    }

    public void GenerateBoard(BoardConfig config, GameSaveData saveData = null)
    {
        currentConfig = config;

        ClearBoard();
        ConfigureGrid(config.rows, config.columns);

        if (saveData != null && saveData.cards != null && saveData.cards.Length > 0)
        {
            CreateCardsFromSave(saveData);
        }
        else
        {
            CreateCards(config.rows, config.columns);
        }
    }

    private void CreateCardsFromSave(GameSaveData saveData)
    {

        int usableCards = saveData.cards.Length;

        for (int i = 0; i < usableCards; i++)
        {
            var cardInfo = saveData.cards[i];

            CardView card = Instantiate(cardPrefab, gridLayoutGroup.transform);
            card.transform.localScale = Vector3.one;

            Texture face = (cardInfo.cardId >= 0 && cardInfo.cardId < cardTheme.faceSprites.Length)
            ? cardTheme.faceSprites[cardInfo.cardId]
            : null;

            card.Init(cardInfo.cardId, face);

            if (cardInfo.isMatched)
            {
                card.SetMatched();
            }
            else if (cardInfo.isFaceUp)
            {
                card.SetToFaceUpImmediate();
            }
            // else stays face down by Init()

            _cards.Add(card);
        }
    }

    /// <summary>
    /// Clears the current board.
    /// </summary>
    private void ClearBoard()
    {
        _cards.Clear();

        while (gridLayoutGroup.transform.childCount > 0)
        {
            Transform child = gridLayoutGroup.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

//            for (int i = gridLayoutGroup.transform.childCount - 1; i >= 0; i--)
//        {
//            Transform child = gridLayoutGroup.transform.GetChild(i);

//#if UNITY_EDITOR
//            if (Application.isPlaying)
//                Destroy(child.gameObject);
//            else
//                DestroyImmediate(child.gameObject);
//#else
//            Destroy(child.gameObject);
//#endif
//        }


    }

    /// <summary>
    /// Configures cell size to fill the entire RectTransform while keeping cells square.
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="columns"></param>
    private void ConfigureGrid(int rows, int columns)
    {

        gridLayoutGroup.spacing = spacing;
        if (padding != null)
            gridLayoutGroup.padding = padding;

        RectTransform rt = gridLayoutGroup.GetComponent<RectTransform>();

        Vector2 size = rt.rect.size;

        float totalHorizontalSpacing = spacing.x * (columns - 1);
        float totalVerticalSpacing = spacing.y * (rows - 1);

        float widthAvailable =
            size.x - padding.left - padding.right - totalHorizontalSpacing;

        float heightAvailable =
            size.y - padding.top - padding.bottom - totalVerticalSpacing;

        float cell = Mathf.Min(widthAvailable / columns, heightAvailable / rows);

        gridLayoutGroup.cellSize = new Vector2(cell, cell);
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;
    }

    public void RecalculateLayout()
    {
        if (currentConfig == null || gridLayoutGroup == null)
            return;

        ConfigureGrid(currentConfig.rows, currentConfig.columns);
    }

    /// <summary>
    /// Instantiates cards and assigns pair IDs.
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="columns"></param>
    private void CreateCards(int rows, int columns)
    {
        if (cardPrefab == null)
        {
            Debug.LogError($"{gameObject.name}: Card prefab is not assigned.");
            return;
        }

        int totalCards = rows * columns;

        // If odd (e.g. 3x3 = 9), use one less card so all are in pairs.
        int usableCards = totalCards;
        if (totalCards % 2 != 0)
        {
            usableCards = totalCards - 1;
        }

        List<int> cardIds = BuildPairIds(usableCards);
        Shuffle(cardIds);

        for (int i = 0; i < usableCards; i++)
        {
            CardView card = Instantiate(cardPrefab, gridLayoutGroup.transform);
            card.transform.localScale = Vector3.one;

            int id = cardIds[i];

            Texture face = (id >= 0 && id < cardTheme.faceSprites.Length)
            ? cardTheme.faceSprites[id]
            : null; // fallback if missing
            card.Init(id, face);

            _cards.Add(card);
        }

    }


    /// <summary>
    /// Builds a list of pair IDs for totalCards.
    /// </summary>
    /// <param name="totalCards"></param>
    /// <returns></returns>
    private List<int> BuildPairIds(int cardCount)
    {

        List<int> ids = new List<int>(cardCount);
        int pairCount = cardCount / 2;

        for (int id = 0; id < pairCount; id++)
        {
            ids.Add(id);
            ids.Add(id);
        }

        //for (int i = ids.Count - 1; i > 0; i--)
        //{
        //    int j = Random.Range(0, i + 1);
        //    int temp = ids[i];
        //    ids[i] = ids[j];
        //    ids[j] = temp;
        //}

        return ids;
    }

    /// <summary>
    /// Shuffles the contents of a list in-place using the Fisher–Yates algorithm.
    /// </summary>
    /// <param name="list"></param>
    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            // Swap i and j
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    internal void SetCheatMode(bool cheatMode)
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].ShowDebugMode(cheatMode);
        }
    }
}
