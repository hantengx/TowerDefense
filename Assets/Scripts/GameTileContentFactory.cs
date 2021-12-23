using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Game;

[CreateAssetMenu]
public class GameTileContentFactory : ScriptableObject
{
    [SerializeField]
    private GameTileContent[] contentPrefabs = default;
    
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
        var prefab = contentPrefabs.FirstOrDefault(x => x.Type == type);
        if (prefab != null)
        {
            return Get(prefab);
        }
        Debug.Assert(false, "Unsupported type: " + type);
        return null;
    }
}
