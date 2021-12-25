using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Vector2Int boardSize = new Vector2Int(11, 11);
    [SerializeField]
    private GameBoard board = default;
    [SerializeField] 
    private GameTileContentFactory tileContentFactory = default;
    
    [SerializeField] 
    private EnemyFactory enemyFactory = default;
    
    [SerializeField, Range(0.1f, 10f)]
    private float spawnSpeed = 1f;

    private float spawnProgress;
    private EnemyCollection enemyCollection = new EnemyCollection();
    private Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    
    public enum GameTileContentType
    {
        Empty,
        Destination,
        Wall,
        SpawnPoint,
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

        spawnProgress += spawnSpeed * Time.deltaTime;
        while (spawnProgress >= 1f)
        {
            spawnProgress -= 1f;
            SpawnEnemy();
        }
        enemyCollection.GameUpdate();
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

        if (Input.GetKey(KeyCode.LeftShift))
        {
            board.ToggleDestination(tile);
        }
        else
        {
            board.ToggleSpawnPoint(tile);
        }
    }

    private void SpawnEnemy()
    {
        var index = UnityEngine.Random.Range(0, board.SpawnPointCount);
        var spawnPoint = board.GetSpawnPoint(index);
        var enemy = enemyFactory.Get();
        enemy.SpawnOn(spawnPoint);
        enemyCollection.Add(enemy);
    }
}
