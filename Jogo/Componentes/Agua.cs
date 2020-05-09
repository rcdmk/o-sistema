using System;
using Jogo.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jogo.Componentes
{
    /// <summary>
    /// Classe base para as tiles de água
    /// </summary>
    public class Agua
    {
        #region Variaveis
        protected Vector2 posicao = Vector2.Zero;
        protected AnimacaoDeSprites animacao;
        protected Texture2D textura;
        protected String nomeTextura;
        protected Rectangle retangulo = new Rectangle(0, 0, 64, 64);
        public static float Densidade = 0.95f;
        #endregion


        #region Propriedades
        public Vector2 Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public Texture2D Textura
        {
            get { return textura; }
            set { textura = value; }
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
                retangulo.X = (int)posicao.X;
                retangulo.Y = (int)posicao.Y;

                return retangulo;
            }
        }
        #endregion


        #region Contrutor
        public Agua()
        {
            animacao = new AnimacaoDeSprites();

            animacao.Animacoes.Add("vazio", new Animacao(64, 64, 1, 1, false, false, 0, 0));
            animacao.Animacoes.Add("metade", new Animacao(64, 64, 1, 1, false, false, 64, 0));
            animacao.Animacoes.Add("cheio", new Animacao(64, 64, 1, 1, false, false, 128, 0));

            animacao.AnimacaoAtual = "cheio";
            animacao.pararAnimacao();
        }

        public Agua(String nomeTextura)
            : this()
        {
            this.nomeTextura = nomeTextura;
        }

        public Agua(Vector2 posicao, String nomeTextura)
            : this(nomeTextura)
        {
            this.posicao = posicao;
        }
        #endregion


        #region Metodos Padrao
        public virtual void LoadContent(ContentManager Content)
        {
            textura = Content.Load<Texture2D>(nomeTextura);
        }

        public virtual void Update(GameTime gameTime)
        {
            animacao.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (animacao.AnimacaoAtual != "vazio") spriteBatch.Draw(textura, posicao, animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual, Color.White);
        }
        #endregion
    }


    /// <summary>
    /// Classe base para o bloco de água
    /// </summary>
    public class Enchente
    {
        #region Variaveis
        private Vector2 posicao;
        private int largura = 1;
        private int altura = 1;
        private String ultimaFileira;
        private ListaItens<Agua> agua;
        private Rectangle retangulo = new Rectangle(0, 0, 64, 64);
        #endregion

        
        #region Propriedades
        public Vector2 Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public int Largura
        {
            get { return largura; }
            set { largura = value; }
        }

        public int Altura
        {
            get { return altura; }
            set { altura = value; }
        }

        public String UltimaFileira
        {
            get { return ultimaFileira; }
            set
            {
                AlterarNivel(value);
            }
        }

        public ListaItens<Agua> Agua
        {
            get { return agua; }
            set { agua = value; }
        }

        public Rectangle HitTest
        {
            get
            {
                retangulo.X = (int)posicao.X;
                retangulo.Y = (int)posicao.Y;
                retangulo.Width = largura * (int)Tile.Dimensoes.X;
                retangulo.Height = altura * (int)Tile.Dimensoes.Y;

                if (ultimaFileira == "vazio")
                {
                    retangulo.Y += (int)Tile.Dimensoes.Y;
                    retangulo.Height -= (int)Tile.Dimensoes.Y;
                }

                return retangulo;
            }
        }
        #endregion


        #region Construtor
        public Enchente()
        {
            agua = new ListaItens<Agua>();
        }

        public Enchente(Vector2 posicao, int largura, int altura)
        {
            this.posicao = posicao;
            this.largura = largura;
            this.altura = altura;

            agua = new ListaItens<Agua>();
        }
        #endregion


        #region Metodos padrão
        public virtual void LoadContent(ContentManager Content)
        {
            MontarAgua(Content);
            AlterarNivel("vazio");
        }

        public virtual void Update(GameTime gameTime)
        {
            for (int i = 0; i < agua.Count; i++)
            {
                agua[i].Update(gameTime);
            }
        }
        
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < agua.Count; i++)
            {
                agua[i].Draw(gameTime, spriteBatch);
            }
        }
        #endregion


        #region Metodos
        public virtual void MontarAgua(ContentManager Content)
        {
            agua.Clear();

            for (int y = 0; y < altura; y++)
            {
                for (int x = 0; x < largura; x++)
                {
                    Agua novaAgua = new Agua(new Vector2(posicao.X + (x * Tile.Dimensoes.X), posicao.Y + (y * Tile.Dimensoes.Y)), "Sprites\\Agua\\TILE_AGUA_SUBINDO");
                    novaAgua.LoadContent(Content);
                    agua.Add(novaAgua);
                }
            }
        }

        public virtual void AlterarNivel(String nivel)
        {
            ultimaFileira = nivel;

            for (int i = 0; i < largura; i++)
            {
                agua[i].Animacao.AnimacaoAtual = nivel;
            }
        }
        #endregion
    }
}
