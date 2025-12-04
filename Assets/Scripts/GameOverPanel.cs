using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{

    public void OnTryAgainClicked()
    {
        GameManager.Instance.RestartGame();
    }
}
