using UnityEngine;

[CreateAssetMenu(fileName = "CardTheme", menuName = "Game/CardTheme")]
public class CardTheme : ScriptableObject
{
    [Tooltip("Face sprites for pairs. Index = CardId.")]
    public Texture[] faceSprites;

    [Tooltip("Back sprite used for all cards.")]
    public Texture backSprite;
}