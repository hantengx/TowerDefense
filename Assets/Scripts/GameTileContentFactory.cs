using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Game;

[CreateAssetMenu]
public class GameTileContentFactory : ScriptableObject
{
    [SerializeField] private GameTileContent destinationPrefab = default;
    [SerializeField] private GameTileContent emptyPrefab = default;
    
    private Scene contentScene;
    // public GameTileContent[] tileContentPrefabs;

    public void Reclaim(GameTileContent content)
    {
        Debug.Assert(content.OriginalFactory == this, "Wrong factory reclaimed!");
        Destroy(content.gameObject);
    }

    private GameTileContent Get(GameTileContent prefab)
    {
        var content = Instantiate(prefab);
        content.OriginalFactory = this;
        MoveToFactoryScene(content.gameObject);
        return content;
    }

    private void MoveToFactoryScene(GameObject o)
    {
        if (!contentScene.isLoaded)
        {
            if (Application.isEditor)
            {
                contentScene = SceneManager.GetSceneByName(name);
                if (!contentScene.isLoaded)
                {
                    contentScene = SceneManager.CreateScene(name);
                }   
            }
            else
            {
                contentScene = SceneManager.CreateScene(name);
            }
        }
        SceneManager.MoveGameObjectToScene(o, contentScene);
    }

    public GameTileContent Get(GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Empty:
                return Get(emptyPrefab);
                break;
            case GameTileContentType.Destination:
                return Get(destinationPrefab);
                break;
        }
        Debug.Assert(false, "Unsupported type: " + type);
        return null;
    }
}
