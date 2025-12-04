using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private BoardManager boardManager;
    [SerializeField] private CardSelectionController selectionController;

    public static GameManager Instance { get; internal set; }

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        selectionController.OnAllPairsMatched.AddListener(HandleAllPairsMatched);
    }

    private void HandleAllPairsMatched()
    {
        UIManager.Instance.ShowGameOverScreen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
