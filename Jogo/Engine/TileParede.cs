#region using
using Microsoft.Xna.Framework;

#endregion

namespace Jogo.Engine
{
    /// <summary>
    /// Classe padr�o para paredes (obst�culos)
    /// </summary>
    class TileParede : Tile
    {
        public TileParede(Vector2 _posicao, int _origem) : base(_posicao, _origem, Vector2.Zero, false, false, false, false, false) { }
    }
}