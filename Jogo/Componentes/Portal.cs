#region using
using System;
using Jogo.Engine;
using Jogo.Personagens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Jogo.Componentes
{
    /// <summary>
    /// Classe base para portais (portas, elevadores, etc.)
    /// </summary>
    public class Portal : GameComponent
    {
        #region Variaveis
        protected Principal principal;

        protected Vector2 posicao = Vector2.Zero;
        protected Vector2 saida = Vector2.Zero;
        protected String tela;

        protected Texture2D textura;
        protected String nomeTextura;

        protected AnimacaoDeSprites animacao;

        protected Personagem personagem;

        protected String tipo;

        protected Rectangle retangulo = new Rectangle(0, 0, 64, 64);
        #endregion


        #region Propriedades
        public Vector2 Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public Vector2 Saida
        {
            get { return saida; }
            set { saida = value; }
        }

        public Personagem Personagem
        {
            get { return personagem; }
            set { personagem = value; }
        }

        public String Tela
        {
            get { return tela; }
            set { tela = value; }
        }

        public Texture2D Textura
        {
            get { return textura; }
            set { textura = value; }
        }

        public String Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        public AnimacaoDeSprites Animacao
        {
            get { return animacao; }
        }

        public Rectangle HitTest
        {
            get
            {
                retangulo = animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual;
                retangulo.X = (int)posicao.X * (int)Tile.Dimensoes.X;
                retangulo.Y = (int)posicao.Y * (int)Tile.Dimensoes.Y;
                return retangulo;
            }
        }
        #endregion


        #region Contrutor
        /// <summary>
        /// Cria um portal para teleportar o personagem
        /// </summary>
        /// <param name="_principal">O jogo</param>
        /// <param name="_posicao">Posição do item no mapa</param>
        /// <param name="_saida">Posição do personagem após teleporte</param>
        /// <param name="_tela">A tela a qual o personagem será levado</param>
        /// <param name="_nomeTextura">O sprite do portal</param>
        public Portal(Principal _principal, Vector2 _posicao, Vector2 _saida, String _tela, String _nomeTextura) : base(_principal)
        {
            this.principal = _principal;
            this.posicao = _posicao;
            this.saida = _saida;
            this.tela = _tela;
            this.nomeTextura = _nomeTextura;

            animacao = new AnimacaoDeSprites();

            LoadContent(_principal.Content);
        }
        #endregion


        #region Metodos Padrao
        public void LoadContent(ContentManager Content)
        {
            textura = Content.Load<Texture2D>(nomeTextura);
        }

        public override void Update(GameTime gameTime)
        {
            animacao.Update(gameTime);

            if (animacao.AnimacaoAtual != null)
            {
                if (animacao.AnimacaoAtual == "abrindo" && animacao.Animacoes["abrindo"].QuadroAtual == animacao.Animacoes["abrindo"].Quadros.Length - 1 && personagem.Passando)
                {
                    animacao.AnimacaoAtual = "fechando";
                    animacao.iniciarAnimacao();

                    principal.telaJogo.Fader.FadeOut();

                    if (tela == "fim")
                    {
                        Principal.mostrarTela(principal.telaCreditos);
                    }
                    else if (principal.telaJogo.mapa.Caminho != String.Concat("mapas\\", tela) && principal.telaJogo.mapa.Caminho != String.Concat("Mapas\\", tela))
                    {
                        principal.telaJogo.mapa.Caminho = String.Concat("mapas\\", tela);
                        principal.telaJogo.Initialize();
                        principal.telaJogo.mapa.LerXML();
                        principal.telaJogo.mapa.criarItens();
                        principal.telaJogo.mapa.criarInimigos();

                        personagem.ObjetivosCompletos = 0;
                        principal.telaJogo.HUD.limpar();
                        personagem.Posicao = new Vector2(principal.telaJogo.mapa.Inicio.X * Tile.Dimensoes.X, principal.telaJogo.mapa.Inicio.Y * Tile.Dimensoes.Y - 1);
                    }
                    else
                    {
                        personagem.Posicao = new Vector2(saida.X * Tile.Dimensoes.X, saida.Y * Tile.Dimensoes.Y - 1);
                    }

                    principal.telaJogo.Fader.FadeIn();
                    personagem.Passando = false;
                }
            }

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle destino = new Rectangle((int)posicao.X * (int)Tile.Dimensoes.X, (int)posicao.Y * (int)Tile.Dimensoes.Y, animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual.Width, animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual.Height);
            if (Mapa.Camera.Intersects(destino))
            {
                spriteBatch.Draw(textura, destino, animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual, Color.White);
            }
        }
        #endregion
    }
}
