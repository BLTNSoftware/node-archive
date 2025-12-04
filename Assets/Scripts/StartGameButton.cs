using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameButton : MonoBehaviour
{

    //[SerializeField] private BoardConfig boardConfig;
    public void OnClicked()
    {
        UIManager.Instance.ShowGameOverScreen();
    }


}
