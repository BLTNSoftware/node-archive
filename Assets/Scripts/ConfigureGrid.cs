using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigureGrid : MonoBehaviour
{

    RectTransform boardPanel;
    GridLayoutGroup grid;
    [SerializeField] int rows = 3;
    [SerializeField] int columns = 3;

    private void Awake()
    {
        boardPanel = transform.GetComponent<RectTransform>();
        grid = GetComponent<GridLayoutGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        

        ConfigureGridLayout(rows, columns);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ConfigureGridLayout(int rows, int columns)
    {
        var rt = boardPanel.GetComponent<RectTransform>();
        var size = rt.rect.size;

        float totalHorizontalSpacing = grid.spacing.x * (columns - 1);
        float totalVerticalSpacing = grid.spacing.y * (rows - 1);

        float widthAvailable = size.x - grid.padding.left - grid.padding.right - totalHorizontalSpacing;
        float heightAvailable = size.y - grid.padding.top - grid.padding.bottom - totalVerticalSpacing;

        float cellWidth = widthAvailable / columns;
        float cellHeight = heightAvailable / rows;

        float cell = Mathf.Min(cellWidth, cellHeight);
        grid.cellSize = new Vector2(cell, cell);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
    }
}
