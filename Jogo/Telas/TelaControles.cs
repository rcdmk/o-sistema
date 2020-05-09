using Jogo.Componentes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jogo.Telas
{
    public class TelaControles :Tela
    {
        #region Construtor
        public TelaControles(Principal _principal, Texture2D _fundo, Texture2D _textura):base(_principal)
        {
            this.principal = _principal;

            Componentes.Add(new ImagemCentral(_principal, _fundo, ImagemCentral.Modo.Esticado));
            Componentes.Add(new ImagemCentral(_principal, _textura , ImagemCentral.Modo.Centralizado));
        }
        #endregion


        #region Metodos Padrao
        public override void Update(GameTime gameTime)
        {
            teclado = Keyboard.GetState();
            controle = GamePad.GetState(PlayerIndex.One);

            if ((tecladoAnterior.IsKeyDown(Keys.Enter) && teclado.IsKeyUp(Keys.Enter)) || (tecladoAnterior.IsKeyDown(Keys.Escape) && teclado.IsKeyUp(Keys.Escape)) || (controleAnterior.Buttons.Back == ButtonState.Pressed && controle.Buttons.Back == ButtonState.Released) || (controleAnterior.Buttons.A == ButtonState.Pressed && controle.Buttons.A == ButtonState.Released) || (controleAnterior.Buttons.B == ButtonState.Pressed && controle.Buttons.B == ButtonState.Released) || (controleAnterior.Buttons.X == ButtonState.Pressed && controle.Buttons.X == ButtonState.Released) || (controleAnterior.Buttons.Y == ButtonState.Pressed && controle.Buttons.Y == ButtonState.Released) || (controleAnterior.Buttons.Start == ButtonState.Pressed && controle.Buttons.Start == ButtonState.Released))
            {
                if (!Principal.Mudo) Sons.MenuOK.Play();

                if (Principal.telaAnterior == principal.telaJogo)
                {
                    Esconder();
                    principal.telaJogo.Enabled = true;
                    principal.telaJogo.Visible = true;
                    Principal.telaAtual = principal.telaJogo;
                    Principal.telaAnterior = this;
                }
                else
                {
                    Principal.mostrarTela(Principal.telaAnterior);
                }
            }

            tecladoAnterior = teclado;
            controleAnterior = controle;
            base.Update(gameTime);
        }
        #endregion
    }
}
