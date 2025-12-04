using UnityEngine;

[CreateAssetMenu(fileName = "BoardConfig", menuName = "Game/BoardConfig")]
public class BoardConfig : ScriptableObject
{
    public int rows;
    public int columns;
    public string id;
    public string description;
}