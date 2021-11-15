using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Vector2Int boardSize = new Vector2Int(11, 11);
    [SerializeField]
    private GameBoard board = default;

    [SerializeField] private GameTileContentFactory tileContentFactory = default;

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
        board.Init(boardSize);
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
}
