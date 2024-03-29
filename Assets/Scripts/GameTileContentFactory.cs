﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Game;

[CreateAssetMenu]
public class GameTileContentFactory : GameObjectFactory
{
    [SerializeField]
    private GameTileContent[] contentPrefabs = default;
    
    // public GameTileContent[] tileContentPrefabs;

    public void Reclaim(GameTileContent content)
    {
        Debug.Assert(content.OriginalFactory == this, "Wrong factory reclaimed!");
        Destroy(content.gameObject);
    }

    private GameTileContent Get(GameTileContent prefab)
    {
        var content = CreateGameObjectInstance(prefab);
        content.OriginalFactory = this;
        return content;
    }

    public GameTileContent Get(GameTileContentType type)
    {
        var prefab = contentPrefabs.FirstOrDefault(x => x.Type == type);
        if (prefab != null)
        {
            return Get(prefab);
        }
        Debug.Assert(false, "Unsupported type: " + type);
        return null;
    }
}
