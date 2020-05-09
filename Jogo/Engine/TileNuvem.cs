#region using
using Microsoft.Xna.Framework;

#endregion

namespace Jogo.Engine
{
    /// <summary>
    /// Classes padr�o para tiles nuvem(tiles que s�o passaveis mas pode-se subir nelas)
    /// </summary>
    class TileNuvem : Tile
    {
        public TileNuvem(Vector2 _posicao, int _origem) : base(_posicao, _origem, Vector2.Zero, true, false, true, false, false) { }
    }
}