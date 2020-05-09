using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jogo.Componentes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ImagemCentral : DrawableGameComponent
    {
        #region variaveis
        //Enums
        public enum Modo
        {
            Centralizado = 1,
            Esticado,
        }

        //Variaveis
        protected readonly Texture2D textura;
        protected readonly Modo modo;
        protected readonly Rectangle tamanho;
        protected readonly SpriteBatch spriteBatch = null;
        #endregion


        #region Construtor
        public ImagemCentral(Principal _principal, Texture2D _textura, Modo _modo) : base(_principal)
        {
            this.textura = _textura;
            this.modo = _modo;
            spriteBatch = _principal.SpriteBatch;

            switch (_modo)
            {
                case Modo.Centralizado:
                    if (textura.Width > Game.Window.ClientBounds.Width)
                    {
                        tamanho = new Rectangle(0, (Game.Window.ClientBounds.Height - (int)(((float)textura.Height / (float)textura.Width) * (float)Game.Window.ClientBounds.Width)) / 2, Game.Window.ClientBounds.Width, (int)(((float)textura.Height / (float)textura.Width) * (float)Game.Window.ClientBounds.Width));
                    }
                    else if (textura.Height > Game.Window.ClientBounds.Height)
                    {
                        tamanho = new Rectangle((Game.Window.ClientBounds.Width - ((textura.Width / textura.Height) * Game.Window.ClientBounds.Height)) / 2, 0, (textura.Width / textura.Height) * Game.Window.ClientBounds.Height, Game.Window.ClientBounds.Height);
                    }
                    else
                    {
                        tamanho = new Rectangle((Game.Window.ClientBounds.Width - textura.Width) / 2, (Game.Window.ClientBounds.Height - textura.Height) / 2, textura.Width, textura.Height);
                    }
                    break;
                case Modo.Esticado:
                    tamanho = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
                    break;
            }
        }
        #endregion


        #region Metodos Padrao
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

            spriteBatch.Draw(textura, tamanho, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
    }
}