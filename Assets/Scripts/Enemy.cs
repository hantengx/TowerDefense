using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameTile tileFrom, tileTo;
    private Vector3 posFrom, posTo;
    private float progress;
    private Direction direction;
    private DirectionChange directionChange;
    private float angleFrom, angleTo;
    
    public EnemyFactory OriginFactory { get; set; }

    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!");
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        progress = 0f;
        PrepareIntro();
    }

    private void PrepareIntro()
    {
        posFrom = tileFrom.transform.localPosition;
        posTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        angleFrom = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
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
            progress -= 1f;
            PrepareNextState();
        }

        transform.localPosition = Vector3.Lerp(posFrom, posTo, progress);
        if (directionChange != DirectionChange.None)
        {
            float angle = Mathf.Lerp(angleFrom, angleTo, progress);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
        return true;
    }

    private void PrepareNextState()
    {
        posFrom = posTo;
        posTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
        direction = tileFrom.PathDirection;
        angleFrom = angleTo;
        //why? different
        //angleTo = direction.GetAngle();
        //Debug.Log("angleTo: " + angleTo);
        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
        //Debug.Log("angleTo: " + angleTo);
    }

    private void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        angleTo = direction.GetAngle();
    }

    void PrepareTurnRight()
    {
        angleTo = angleFrom + 90f;
        //avoid wrapping angles
        //angleTo = direction.GetAngle();
    }

    void PrepareTurnLeft()
    {
        angleTo = angleFrom - 90f;
    }

    void PrepareTurnAround()
    {
        angleTo = angleFrom + 180f;
    }
}
