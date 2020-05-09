using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Jogo.Componentes
{
    /// <summary>
    /// Classe base para telas de vídeo com skip por teclado ou controle
    /// </summary>
    public class TelaVideo : Tela
    {
        #region Variáveis
        protected string caminhoVideo;
        protected Video video;
        protected VideoPlayer videoPlayer;
        protected Texture2D texturaVideo;
        protected Rectangle retangulo = new Rectangle(0, 0, 64, 64);
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

        public Video Video
        {
            get { return video; }
            set { video = value; }
        }
        #endregion


        #region Construtor
        public TelaVideo(Principal _principal, string _caminhoVideo, Tela _proximaTela)
            : base(_principal)
        {
            this.caminhoVideo = _caminhoVideo;
            this.proximaTela = _proximaTela;

            videoPlayer = new VideoPlayer();
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
            video = principal.Content.Load<Video>(caminhoVideo);

            retangulo.Width = GraphicsDevice.Viewport.Width;
            retangulo.Height = GraphicsDevice.Viewport.Height;

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
                    videoPlayer.Play(video);
                    videoPlayer.IsMuted = Principal.Mudo;
                }
            }

            teclado = Keyboard.GetState();
            controle = GamePad.GetState(PlayerIndex.One);

            //se pressionar alguma tecla, sai do vídeo
            if ((tecladoAnterior.IsKeyDown(Keys.Enter) && teclado.IsKeyUp(Keys.Enter)) || (tecladoAnterior.IsKeyDown(Keys.Escape) && teclado.IsKeyUp(Keys.Escape)) || (controleAnterior.Buttons.Back == ButtonState.Pressed && controle.Buttons.Back == ButtonState.Released) || (controleAnterior.Buttons.A == ButtonState.Pressed && controle.Buttons.A == ButtonState.Released) || (controleAnterior.Buttons.B == ButtonState.Pressed && controle.Buttons.B == ButtonState.Released) || (controleAnterior.Buttons.X == ButtonState.Pressed && controle.Buttons.X == ButtonState.Released) || (controleAnterior.Buttons.Y == ButtonState.Pressed && controle.Buttons.Y == ButtonState.Released) || (controleAnterior.Buttons.Start == ButtonState.Pressed && controle.Buttons.Start == ButtonState.Released))
            {
                if (!Principal.Mudo) Sons.MenuOK.Play();
                Principal.mostrarTela(proximaTela);
            }
            else if (video != null)
            {
                if (videoPlayer.PlayPosition >= video.Duration) Principal.mostrarTela(proximaTela);
            }

            tecladoAnterior = teclado;
            controleAnterior = controle;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (videoPlayer.State == MediaState.Playing)
            {
                principal.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

                texturaVideo = videoPlayer.GetTexture();

                principal.SpriteBatch.Draw(texturaVideo, retangulo, Color.White);

                principal.SpriteBatch.End();
            }

            base.Draw(gameTime);
        }
        #endregion


        #region Metodos
        public override void Mostrar()
        {
            base.Mostrar();
            inicio = false;

            if (video != null)
            {
                //videoPlayer.Play(video);
            }
        }

        public override void Esconder()
        {
            if (videoPlayer != null)
            {
                if (videoPlayer.State != MediaState.Stopped) videoPlayer.Stop();
            }

            base.Esconder();
        }
        #endregion
    }
}
