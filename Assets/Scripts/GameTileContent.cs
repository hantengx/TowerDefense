using UnityEngine;
using static Game;

public class GameTileContent : MonoBehaviour
{
    [SerializeField]
    private GameTileContentType type = default;

    public GameTileContentType Type => type;
    
    private GameTileContentFactory originalFactory;

    public GameTileContentFactory OriginalFactory
    {
        get => originalFactory;
        set
        {
            Debug.Assert(originalFactory == null, "Redefined origin factory!");
            originalFactory = value;
        }
    }

    public void Recycle()
    {
        originalFactory.Reclaim(this);
    }
}