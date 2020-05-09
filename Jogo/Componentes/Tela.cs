using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Jogo.Componentes
{
    public class Tela : DrawableGameComponent
    {
        #region Variaveis
        protected Principal principal;
        private readonly List<GameComponent> componentes;

        protected KeyboardState teclado;
        protected GamePadState controle;
        protected KeyboardState tecladoAnterior;
        protected GamePadState controleAnterior;
        #endregion


        #region propriedades
        public List<GameComponent> Componentes
        {
            get { return componentes; }
        }
        #endregion


        #region Construtor
        public Tela(Principal _principal) : base(_principal)
        {
            this.principal = _principal;

            componentes = new List<GameComponent>();

            Esconder();
        }
        #endregion


        #region metodos
        public virtual void Mostrar()
        {
            tecladoAnterior = teclado;
            controleAnterior = controle;

            Enabled = true;
            Visible = true;
        }

        public virtual void Esconder()
        {
            Enabled = false;
            Visible = false;
        }
        #endregion


        #region metodosPadrao
        public override void Initialize()
        {
            for (int i = 0; i < componentes.Count; i++)
            {
                GameComponent gc = componentes[i];
                gc.Initialize();
            }
            
            base.Initialize();
        }
        
        public override void Update(GameTime gameTime)
        {
            if (MediaPlayer.IsMuted != Principal.Mudo) MediaPlayer.IsMuted = Principal.Mudo;

            for (int i = 0; i < componentes.Count; i++)
            {
                if (componentes[i].Enabled)
                {
                    componentes[i].Update(gameTime);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < componentes.Count; i++)
            {
                GameComponent gc = componentes[i];
                if (gc is DrawableGameComponent && ((DrawableGameComponent)gc).Visible)
                {
                    ((DrawableGameComponent)gc).Draw(gameTime);
                }
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}
