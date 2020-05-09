#region using
using Microsoft.Xna.Framework;
#endregion

namespace Jogo.Engine
{
    /// <summary>
    /// Classes padrão para tiles de escada
    /// </summary>
    class TileEscada : Tile
    {
        public TileEscada(Vector2 _posicao, int _origem) : base(_posicao, _origem, Vector2.Zero, true, true, false, false, false) { }
    }
}
