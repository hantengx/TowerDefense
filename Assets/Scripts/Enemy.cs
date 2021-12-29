﻿using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] 
    private Transform model = default;
    
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

        if (directionChange != DirectionChange.None)
        {
            float angle = Mathf.Lerp(angleFrom, angleTo, progress);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(posFrom, posTo, progress);
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
        // angleTo = direction.GetAngle();
        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
        // Debug.Log($"direction: {direction}, angleFrom: {angleFrom}, angleTo: {angleTo}");
    }

    private void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        angleTo = direction.GetAngle();
        model.localPosition = Vector3.zero;
    }

    private void PrepareTurnRight()
    {
        angleTo = angleFrom + 90f;
        model.localPosition = new Vector3(-0.5f, 0f);
        transform.localPosition = posFrom + direction.GetHalfVector();
        //avoid wrapping angles
        //angleTo = direction.GetAngle();
    }

    private void PrepareTurnLeft()
    {
        angleTo = angleFrom - 90f;
        model.localPosition = new Vector3(0.5f, 0f);
        transform.localPosition = posFrom + direction.GetHalfVector();
    }

    private void PrepareTurnAround()
    {
        angleTo = angleFrom + 180f;
        model.localPosition = Vector3.zero;
        transform.localPosition = posFrom;
    }
}
