using UnityEngine;
using static Game;

namespace Assets.Scripts
{
    public class GameTileContent : MonoBehaviour
    {
        [SerializeField]
        private GameTileContentType type = default;

        public GameTileContentType Type => type;
    }
}