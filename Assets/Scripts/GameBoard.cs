using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    private Transform ground = default;
    [SerializeField]
    private GameTile tilePrefab = default;

    private GameTile[] tiles;
    private Vector2Int size;
    private Queue<GameTile> searchFrontier = new Queue<GameTile>();
    private GameTileContentFactory contentFactory;

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

            searchFrontier.Enqueue(tile.GrowPathNorth());
            searchFrontier.Enqueue(tile.GrowPathEast());
            searchFrontier.Enqueue(tile.GrowPathSouth());
            searchFrontier.Enqueue(tile.GrowPathWest());
        }

        foreach (var tile in tiles)
        {
            tile.ShowPath();
        }

        return true;
    }

    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
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
        else
        {
            tile.Content = contentFactory.Get(Game.GameTileContentType.Destination);
            FindPaths();
        }
    }
}
