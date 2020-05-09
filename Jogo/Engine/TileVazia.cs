#region using
using Microsoft.Xna.Framework;

#endregion

namespace Jogo.Engine
{
    /// <summary>
    /// Classe padrão para tiles vazias
    /// </summary>
    class TileVazia : Tile
    {
        public TileVazia(Vector2 _posicao, int _origem) : base(_posicao, _origem, Vector2.Zero, true, false, false, false, false) { }
    }
}