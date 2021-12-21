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
            Debug.Log("click position: " + Input.mousePosition);
            HandleTouch();
        }
    }

    private void HandleTouch()
    {
        var tile = board.GetTile(TouchRay);
        if (tile == null)
        {
            return;
        }

        // tile.Content = tileContentFactory.Get(GameTileContentType.Destination);
        board.ToggleDestination(tile);
    }
}
