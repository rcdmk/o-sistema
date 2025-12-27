using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Jogo.Componentes
{
    /// <summary>
    /// Classe base para telas de vídeo com skip por teclado ou controle
    /// </summary>
    public class TelaVideo : Tela, IDisposable
    {
        #region Variáveis
        protected string caminhoVideo;
        protected VideoPlayer videoPlayer;
        protected Tela proximaTela;
        protected int tempoEspera = 1;
        protected float tempo;
        protected bool inicio;
        #endregion


        #region Propriedades
        public string CaminhoVideo
        {
            get { return caminhoVideo; }
            set { caminhoVideo = value; }
        }

        public VideoPlayer VideoPlayer
        {
            get { return VideoPlayer; }
        }

        #endregion

        #region Construtor
        public TelaVideo(Principal _principal, string _caminhoVideo, Tela _proximaTela)
            : base(_principal)
        {
            this.caminhoVideo = _caminhoVideo;
            this.proximaTela = _proximaTela;

            videoPlayer = new VideoPlayer(_principal);
        }
        #endregion


        #region Metodos Padrao
        public override void Initialize()
        {
            base.Initialize();
            LoadContent();
        }

        public virtual new void LoadContent()
        {
            videoPlayer.LoadMedia(caminhoVideo);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!inicio)
            {
                tempo += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (tempo >= tempoEspera)
                {
                    tempo = 0;
                    inicio = true;
                    videoPlayer.Play();
                    videoPlayer.IsMuted = Principal.Mudo;
                }
            }
            else
            {
                if (videoPlayer.State == MediaState.Playing && videoPlayer.PlayPosition >= 0.99f)
                {
                    Principal.mostrarTela(proximaTela);
                }

                //Skip por teclado ou controle
                KeyboardState teclado = Keyboard.GetState();
                GamePadState controle = GamePad.GetState(PlayerIndex.One);
                if (teclado.IsKeyDown(Keys.Enter) || teclado.IsKeyDown(Keys.Space) ||
                    controle.Buttons.A == ButtonState.Pressed || controle.Buttons.Start == ButtonState.Pressed)
                {
                    Principal.mostrarTela(proximaTela);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Render extracted video frame to game window
            if (videoPlayer.State == MediaState.Playing)
            {
                Texture2D currentFrame = videoPlayer.GetCurrentFrame(principal.GraphicsDevice);
                if (currentFrame != null)
                {
                    principal.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                    principal.SpriteBatch.Draw(currentFrame,
                        new Rectangle(0, 0, principal.GraphicsDevice.Viewport.Width, principal.GraphicsDevice.Viewport.Height),
                        Color.White);
                    principal.SpriteBatch.End();
                    currentFrame.Dispose();
                }
            }

            base.Draw(gameTime);
        }

        #endregion

        #region Metodos
        public override void Mostrar()
        {
            base.Mostrar();
            inicio = false;
            tempo = 0;
        }

        public override void Esconder()
        {
            videoPlayer?.Stop();
            base.Esconder();
        }


        public new void Dispose()
        {
            videoPlayer.Dispose();

            base.Dispose();
        }
        #endregion
    }
}
