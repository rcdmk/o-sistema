#region using
using Microsoft.Xna.Framework;

#endregion

namespace Jogo.Engine
{
    /// <summary>
    /// Classes padrão para tiles móveis
    /// </summary>
    class TileMovel : Tile
    {
        public TileMovel(Vector2 _posicao, int _origem) : base(_posicao, _origem, new Vector2(1f, 0f), true, false, true, true, false) { }
    }
}