using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Vector2Int boardSize = new Vector2Int(11, 11);
    [SerializeField]
    private GameBoard board = default;
    [SerializeField] 
    private GameTileContentFactory tileContentFactory = default;

    private Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    
    public enum GameTileContentType
    {
        Empty,
        Destination,
        Wall,
        Water,
        Rock,
        Lava
    }

    private void Awake()
    {
        board.Init(boardSize, tileContentFactory);
        board.ShowGrid = true;
    }

    private void OnValidate()
    {
        if (boardSize.x < 2)
        {
            boardSize.x = 2;
        }

        if (boardSize.y < 2)
        {
            boardSize.y = 2;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("click position: " + Input.mousePosition);
            HandleTouch();
        } 
        else if (Input.GetMouseButtonDown(1)) 
        {
            HandleAlternativeTouch();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            board.ShowPaths = !board.ShowPaths;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            board.ShowGrid = !board.ShowGrid;
        }
    }

    private void HandleTouch()
    {
        var tile = board.GetTile(TouchRay);
        if (tile == null)
        {
            return;
        }

        board.ToggleWall(tile);
    }

    private void HandleAlternativeTouch()
    {
        var tile = board.GetTile(TouchRay);
        if (tile == null)
        {
            return;
        }
        
        board.ToggleDestination(tile);
    }
}
