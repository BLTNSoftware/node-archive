using System;
using UnityEngine;

[Serializable]
public class CardSaveData
{
    public int cardId;
    public bool isMatched;
    public bool isFaceUp;
}

[Serializable]
public class GameSaveData
{
    public string boardConfigId;
    public int rows;
    public int columns;

    public CardSaveData[] cards;
}
