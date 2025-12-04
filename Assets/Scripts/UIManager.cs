using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;

    public static UIManager Instance { get; internal set; }

    private void Awake()
    {
        Instance = this;
    }

    internal void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }
}
