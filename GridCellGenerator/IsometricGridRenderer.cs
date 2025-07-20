using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using wildlogicgames;

public class IsometricGridRenderer : MonoBehaviour
{

	//[Header("example")]

	private int _gridSizeX = 10; 
	private int _gridSizeY = 10; 
	private float _cellSize = 2.0f;
    private float _gridLineThickness = 0.025f;//0.05f;

    private Color _gridColor = Color.green;
    private Material _gridMaterial;
    private List<GameObject> _gridLines = new List<GameObject>();


    [Header("Cell Material")]
    public Material _cellMaterial;
    private GameObject[,] _gridCells;

    private void Awake() => Setup();
	private void Start()
	{
        GenerateGrid();
        GenerateCells();
    }
	private void Setup()
	{
        _gridMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    private void GenerateCells()
    {
        ClearCells();

        _gridCells = new GameObject[_gridSizeX, _gridSizeY];

        for (int x = 0; x < _gridSizeX; x++)  // Fixed <= to <
        {
            for (int y = 0; y < _gridSizeY; y++)  // Fixed <= to <
            {
                // Convert grid coordinates to isometric space (adjusting for centering)
                Vector3 cellPosition = GridToIsometric(x, y);

                //cellPosition.x = cellPosition.x + _cellSize / 2;
                //cellPosition.y = cellPosition.y + _cellSize / 2;
                cellPosition.z = cellPosition.z + _cellSize / 4;

                // Adjust position to fit grid properly
                //cellPosition.y -= 0.01f;  // Slight offset to prevent z-fighting

                // Create a cell quad.
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cell.transform.position = cellPosition;

                // Isometric rotation correction
                cell.transform.rotation = Quaternion.Euler(25, 0, 45);

                // Scale quads to fit the grid perfectly
                //cell.transform.localScale = new Vector3(_cellSize/2, _cellSize/2, _cellSize / 4);
                cell.transform.localScale = new Vector3(_cellSize / 1.4f, _cellSize / 1.4f, _cellSize / 8);

                // Apply texture/material
                if (_cellMaterial != null)
                    cell.GetComponent<Renderer>().material = _cellMaterial;

                cell.transform.parent = this.transform;  // Parent for organization
                _gridCells[x, y] = cell;  // Store reference to clear later
            }
        }
    }

    private void GenerateGrid()
    {
        ClearGrid();

        for (int x = 0; x <= _gridSizeX; x++)
        {
            for (int y = 0; y <= _gridSizeY; y++)
            {
                int a = (x == _gridSizeX) ? 0 : 1;  // Prevent right-side overflow
                int b = (y == _gridSizeY) ? 0 : 1;  // Prevent top-side overflow

                // Convert grid coordinates to isometric space
                Vector3 startPos = GridToIsometric(x, y);
                Vector3 endPosX = GridToIsometric(x + a, y);
                Vector3 endPosY = GridToIsometric(x, y + b);

                if (a == 1) // Only draw if within bounds
                    DrawLine(startPos, endPosX);

                if (b == 1) // Only draw if within bounds
                    DrawLine(startPos, endPosY);
            }
        }
    }










    private void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject("GridLine");
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.startWidth = _gridLineThickness;
        lr.endWidth = _gridLineThickness;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        if (_gridMaterial != null)
            lr.material = _gridMaterial;
        else
            lr.material = new Material(Shader.Find("Sprites/Default"));

        lr.useWorldSpace = true;
    }
    private Vector3 GridToIsometric(int x, int y)
    {
        //Convert grid coordinates to isometric world space.
        float isoX = (x - y) * _cellSize * 0.5f;
        float isoZ = (x + y) * _cellSize * 0.25f;
        return new Vector3(isoX, 0, isoZ);
    }
    private void ClearGrid()
    {
        foreach (GameObject line in _gridLines)
        {
            Destroy(line);
        }
        _gridLines.Clear();
    }
    void ClearCells()
    {
        if (_gridCells == null) return;

        foreach (GameObject cell in _gridCells)
        {
            if (cell != null)
                Destroy(cell);
        }
    }
    private void OnDrawGizmosSelected()
    {

        //Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(this.gameObject.transform.position, 10.0f);

        Gizmos.color = _gridColor;

        for (int x = 0; x <= _gridSizeX; x++)
        {
            for (int y = 0; y <= _gridSizeY; y++)
            {
                // Convert grid coordinates to isometric space
                Vector3 startPos = GridToIsometric(x, y);
                Vector3 endPosX = GridToIsometric(x + 1, y);
                Vector3 endPosY = GridToIsometric(x, y + 1);

                // Draw grid lines
                Gizmos.DrawLine(startPos, endPosX);
                Gizmos.DrawLine(startPos, endPosY);
            }
        }
    }
    
}
