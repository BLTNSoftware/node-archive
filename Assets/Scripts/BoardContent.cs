using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardContent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnRectTransformDimensionsChange()
    {
        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.RecalculateLayout();
        }
            
    }
}
