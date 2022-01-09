using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    private Transform ground = default;
    [SerializeField]
    private GameTile tilePrefab = default;
    [SerializeField] 
    private Texture2D gridTexture = default;

    private GameTile[] tiles;
    private Vector2Int size;
    private Queue<GameTile> searchFrontier = new Queue<GameTile>();
    private GameTileContentFactory contentFactory;
    private bool showPaths, showGrid;
    private List<GameTile> spawnPoints = new List<GameTile>();
    private List<GameTileContent> updatingContent = new List<GameTileContent>();

    public int SpawnPointCount => spawnPoints.Count;

    public bool ShowPaths
    {
        get => showPaths;
        set
        {
            showPaths = value;
            if (showPaths)
            {
                foreach (var tile in tiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach (var tile in tiles)
                {
                    tile.HidePath();
                }
            }
        }
    }

    public bool ShowGrid
    {
        get => showGrid;
        set
        {
            showGrid = value;
            var m = ground.GetComponent<MeshRenderer>().material;
            if (showGrid)
            {
                m.mainTexture = gridTexture;
                m.SetTextureScale("_MainTex", size);
            }
            else
            {
                m.mainTexture = null;
            }
        }
    }

    public void Init(Vector2Int size, GameTileContentFactory factory)
    {
        this.size = size;
        this.contentFactory = factory;
        ground.localScale = new Vector3(size.x, size.y, 1f);
        tiles = new GameTile[size.x * size.y];

        var offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
        var index = 0;
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, index++)
            {
                var tile = tiles[index] = Instantiate(tilePrefab, transform);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
                tile.Content = contentFactory.Get(Game.GameTileContentType.Empty);
                tile.IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;
                }

                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, tiles[index - 1]);
                }
                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tile, tiles[index - size.x]);
                }
            }
        }

        // FindPaths();
        ToggleDestination(tiles[tiles.Length / 2]);
        ToggleSpawnPoint(tiles[0]);
    }

    private bool FindPaths()
    {
        foreach (var tile in tiles)
        {
            if (tile.Content.Type == Game.GameTileContentType.Destination)
            {
                tile.BecameDestination();
                searchFrontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }

        if (searchFrontier.Count == 0)
        {
            return false;
        }

        // tiles[tiles.Length / 2].BecameDestination();
        // searchFrontier.Enqueue(tiles[tiles.Length / 2]);

        while(searchFrontier.Count > 0)
        {
            var tile = searchFrontier.Dequeue();
            if (tile == null)
            {
                continue;
            }

            if (tile.IsAlternative)
            {
                searchFrontier.Enqueue(tile.GrowPathNorth());
                searchFrontier.Enqueue(tile.GrowPathSouth());
                searchFrontier.Enqueue(tile.GrowPathEast());
                searchFrontier.Enqueue(tile.GrowPathWest());
            }
            else
            {
                searchFrontier.Enqueue(tile.GrowPathWest());
                searchFrontier.Enqueue(tile.GrowPathEast());
                searchFrontier.Enqueue(tile.GrowPathSouth());
                searchFrontier.Enqueue(tile.GrowPathNorth());
            }
        }

        if (tiles.Any(tile => !tile.HasPath))
        {
            return false;
        }

        if (showPaths)
        {
            foreach (var tile in tiles)
            {
                tile.ShowPath();
            }
        }

        return true;
    }

    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1))
        {
            var x = (int) (hit.point.x + size.x * 0.5f);
            var y = (int) (hit.point.z + size.y * 0.5f);
            if (x >= 0 && x < size.x && y >= 0 && y < size.y)
            {
                return tiles[x + y * size.x];
            }
        }
        return null;
    }

    public void ToggleDestination(GameTile tile)
    {
        if (tile.Content.Type == Game.GameTileContentType.Destination)
        {
            tile.Content = contentFactory.Get(Game.GameTileContentType.Empty);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(Game.GameTileContentType.Destination);
                FindPaths();
            }
        }
        else if (tile.Content.Type == Game.GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(Game.GameTileContentType.Destination);
            FindPaths();
        }
    }

    public void ToggleWall(GameTile tile)
    {
        if (tile.Content.Type == Game.GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(Game.GameTileContentType.Empty);
            FindPaths();
        }
        else if(tile.Content.Type == Game.GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(Game.GameTileContentType.Wall);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(Game.GameTileContentType.Empty);
                FindPaths();
            }
        }
    }

    public void ToggleSpawnPoint(GameTile tile)
    {
        if (tile.Content.Type == Game.GameTileContentType.SpawnPoint)
        {
            if (spawnPoints.Count > 1)
            {
                spawnPoints.Remove(tile);
                tile.Content = contentFactory.Get(Game.GameTileContentType.Empty);
            }
        }
        else if (tile.Content.Type == Game.GameTileContentType.Empty)
        {
            spawnPoints.Add(tile);
            tile.Content = contentFactory.Get(Game.GameTileContentType.SpawnPoint);
        }
    }

    public GameTile GetSpawnPoint(int index)
    {
        if (index < 0 || index >= spawnPoints.Count)
        {
            return null;
        }
        return spawnPoints[index];
    }
    
    public void ToggleTower(GameTile tile)
    {
        if (tile.Content.Type == Game.GameTileContentType.Tower)
        {
            updatingContent.Remove(tile.Content);
            tile.Content = contentFactory.Get(Game.GameTileContentType.Empty);
            FindPaths();
        }
        else if(tile.Content.Type == Game.GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(Game.GameTileContentType.Tower);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(Game.GameTileContentType.Empty);
                FindPaths();
            }
            else
            {
                updatingContent.Add(tile.Content);
            }
        }
        else if (tile.Content.Type == Game.GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(Game.GameTileContentType.Tower);
            updatingContent.Add(tile.Content);
        }
    }

    public void GameUpdate()
    {
        for (var i = 0; i < updatingContent.Count; i++)
        {
            updatingContent[i].GameUpdate();
        }
    }
}
