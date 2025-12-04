using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatButton : MonoBehaviour
{
    public void OnClicked()
    {
        GameManager.Instance.SwitchCheatMode();
    }
}
