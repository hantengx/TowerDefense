using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameTile tileFrom, tileTo;
    private Vector3 posFrom, posTo;
    private float progress;
    
    public EnemyFactory OriginFactory { get; set; }

    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!");
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        posFrom = tileFrom.transform.localPosition;
        posTo = tileTo.transform.localPosition;
        progress = 0f;
    }

    public bool GameUpdate()
    {
        progress += Time.deltaTime;
        while (progress >= 1f)
        {
            tileFrom = tileTo;
            tileTo = tileFrom.NextTileOnPath;
            if (tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }
            posFrom = tileFrom.transform.localPosition;
            posTo = tileTo.transform.localPosition;
            progress -= 1f;
        }

        transform.localPosition = Vector3.Lerp(posFrom, posTo, progress);
        return true;
    }
}
