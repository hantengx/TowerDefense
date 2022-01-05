﻿using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] 
    private Transform model = default;
    
    private GameTile tileFrom, tileTo;
    private Vector3 posFrom, posTo;
    private float progress, progressFactor;
    private Direction direction;
    private DirectionChange directionChange;
    private float angleFrom, angleTo;
    private float pathOffset;
    
    public EnemyFactory OriginFactory { get; set; }


    public void Initialize(float scale, float pathOffset)
    {
        model.localScale = Vector3.one * scale;
        this.pathOffset = pathOffset;
    }
    
    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!");
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        progress = 0f;
        PrepareIntro();
    }

    public bool GameUpdate()
    {
        progress += Time.deltaTime * progressFactor;
        while (progress >= 1f)
        {
            // tileFrom = tileTo;
            // tileTo = tileFrom.NextTileOnPath;
            if (tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            progress = (progress - 1f) / progressFactor;
            // progress -= 1f;
            PrepareNextState();
            progress *= progressFactor;
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
        tileFrom = tileTo;
        tileTo = tileFrom.NextTileOnPath;
        posFrom = posTo;
        if (tileTo == null)
        {
            PrepareOutro();
            return;
        }
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
        model.localPosition = new Vector3(pathOffset, 0f);
        progressFactor = 1f;
    }

    private void PrepareTurnRight()
    {
        angleTo = angleFrom + 90f;
        model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
        transform.localPosition = posFrom + direction.GetHalfVector();
        //avoid wrapping angles
        //angleTo = direction.GetAngle();
        progressFactor = 1f / (Mathf.PI / 2f * (0.5f - pathOffset));
    }

    private void PrepareTurnLeft()
    {
        angleTo = angleFrom - 90f;
        model.localPosition = new Vector3(pathOffset + 0.5f, 0f);
        transform.localPosition = posFrom + direction.GetHalfVector();
        progressFactor = 1f / (Mathf.PI / 2f * (0.5f + pathOffset));
    }

    private void PrepareTurnAround()
    {
        angleTo = angleFrom + (pathOffset < 0f ? 180f : -180f);
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localPosition = posFrom;
        progressFactor = 1f / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
    }
    
    //登场
    private void PrepareIntro()
    {
        posFrom = tileFrom.transform.localPosition;
        posTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        angleFrom = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f;
    }

    //谢幕
    private void PrepareOutro()
    {
        posTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        angleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f;
    }
}
