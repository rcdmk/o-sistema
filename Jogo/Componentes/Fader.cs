using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jogo.Componentes
{
    /// <summary>
    /// Classe base para crir transições de tela com fade-in ou fade-out
    /// </summary>
    public class Fader : GameComponent
    {
        #region Variaveis
        protected Vector2 posicao = Vector2.Zero;
        protected Color cor = Color.Black;
        protected AnimacaoDeSprites animacao;
        protected Texture2D texturaIn;
        protected Texture2D texturaOut;
        protected String nomeTexturaIn;
        protected String nomeTexturaOut;
        protected bool visivel;
        #endregion


        #region Propriedades
        public bool Visivel
        {
            get { return visivel; }
            set { visivel = value; }
        }

        public Vector2 Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public Color Cor
        {
            get { return cor; }
            set { cor = value; }
        }

        public AnimacaoDeSprites Animacao
        {
            get { return animacao; }
        }
        #endregion


        #region Contrutor
        public Fader(Principal _principal, String _nomeTexturaIn, String _nomeTexturaOut)
            : base(_principal)
        {
            this.nomeTexturaIn = _nomeTexturaIn;
            this.nomeTexturaOut = _nomeTexturaOut;

            //Animação
            animacao = new AnimacaoDeSprites();

            Animacao fadeIn = new Animacao(1024, 154, 5, 10, false, false, 0, 0);
            Animacao fadeOut = new Animacao(1024, 154, 5, 10, false, false, 0, 0);

            animacao.Animacoes.Add("fadeIn", fadeIn);
            animacao.Animacoes.Add("fadeOut", fadeOut);

            LoadContent(_principal.Content);
        }
        #endregion


        #region Metodos Padrao
        protected void LoadContent(ContentManager Content)
        {
            texturaIn = Content.Load<Texture2D>(nomeTexturaIn);
            texturaOut = Content.Load<Texture2D>(nomeTexturaOut);
        }

        public override void Update(GameTime gameTime)
        {
            animacao.Update(gameTime);

            if (visivel)
            {
                if (animacao.AnimacaoAtual == "fadeIn" && animacao.Animacoes["fadeIn"].QuadroAtual == animacao.Animacoes["fadeIn"].Quadros.Length - 1)
                {
                    Esconder();
                }
                else if (animacao.AnimacaoAtual == "fadeOut" && animacao.Animacoes["fadeOut"].QuadroAtual == animacao.Animacoes["fadeOut"].Quadros.Length - 1)
                {
                    Pausar();
                }
            }

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (visivel)
            {
                Rectangle destino = new Rectangle((int)posicao.X, (int)posicao.Y, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
                if (animacao.AnimacaoAtual == "fadeIn")
                {
                    spriteBatch.Draw(texturaIn, destino, animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual, cor);
                }
                else
                {
                    spriteBatch.Draw(texturaOut, destino, animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual, cor);
                }
            }
        }
        #endregion


        #region Metodos
        public void Esconder()
        {
            visivel = false;
        }

        public void FadeIn()
        {
            animacao.AnimacaoAtual = "fadeIn";
            animacao.Animacoes["fadeIn"].Reset();
            animacao.iniciarAnimacao();
            visivel = true;
        }

        public void FadeOut()
        {
            animacao.AnimacaoAtual = "fadeOut";
            animacao.Animacoes[animacao.AnimacaoAtual].Reset();
            animacao.iniciarAnimacao();
            visivel = true;
        }

        public void Pausar()
        {
            animacao.pararAnimacao();
        }
        #endregion
    }
}
