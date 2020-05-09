using Microsoft.Xna.Framework;

namespace Jogo.Componentes
{
    /// <summary>
    /// Classe padrão para itens que não são desenhados, usados apenas para colisões
    /// </summary>
    class ItemInvisivel : Item
    {
        #region Propriedades
        public new Rectangle HitTest
        {
            get
            {
                retangulo.X = (int)posicao.X;
                retangulo.Y = (int)posicao.Y;
                return retangulo;
            }
        }
        #endregion


        #region Construtor
        public ItemInvisivel(Principal _principal, Vector2 _posicao, int _largura, int _altura)
            : base(_principal, _posicao, "")
        {
            retangulo.X = (int)_posicao.X;
            retangulo.Y = (int)_posicao.Y;
            retangulo.Width = _largura;
            retangulo.Height = _altura;
        }
        #endregion


        #region Metodos Padrao
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content) { }

        public override void Update(GameTime gameTime) { }

        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch) { }
        #endregion
    }
}
