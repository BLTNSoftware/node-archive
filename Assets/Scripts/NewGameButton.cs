using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameButton : MonoBehaviour
{

    [SerializeField] private BoardConfig boardConfig;
    public void OnClicked()
    {
        GameManager.Instance.StartNewGame(boardConfig);
    }


}
