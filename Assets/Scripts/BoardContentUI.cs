using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardContentUI : MonoBehaviour
{

    private void OnRectTransformDimensionsChange()
    {
        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.RecalculateLayout();
        }
            
    }
}
