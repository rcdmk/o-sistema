using System;
using System.Xml.Serialization;
using Jogo.Componentes;
using Jogo.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Jogo.Personagens
{
    /// <summary>
    /// Classe básica para personagens do jogo
    /// </summary>
    public class Personagem : GameComponent
    {
        #region Variaveis
        protected Principal principal;

        //Dados principais
        protected Vector2 posicao = Vector2.Zero;
        protected Vector2 velocidade = Vector2.Zero;
        protected float velocidadeIncremental = 1f;
        protected int vidas = 1;
        protected int hp = 100;
        protected float pulo = 10;
        protected float peso = 10;
        protected float forca = 10;
        protected float friccao = 0.8f;
        protected Vector2 medidas = new Vector2(64f, 192f);

        //Morte
        protected bool morto;
        protected float tempoMorto;

        //Checagens
        protected bool pulando = false;
        protected bool subindo = false;
        protected bool caindo = true;
        protected bool passando = false;
        protected Vector2 posicaoAtual = Vector2.Zero;
        protected Vector2 posicaoAtual2 = Vector2.Zero;
        protected Vector2 posicaoFutura = Vector2.Zero;
        protected Vector2 posicaoFutura2 = Vector2.Zero;
        protected float posicaoX = 0f;
        protected float posicaoY = 0f;
        protected int tileX = 0;
        protected int tileY = 0;
        protected int offsetX;
        protected bool sobreAlgo = false;
        protected Rectangle retangulo = new Rectangle(0,0,64,64);
        protected int objetivosCompletos = 0;
        protected int itensUsados = 0;
        protected bool agua = false;

        //Draw
        protected AnimacaoDeSprites animacao;
        protected Texture2D sprite;
        protected String textura;
        protected Color cor = Color.White;
        public SpriteEffects flip = SpriteEffects.None;
        protected Rectangle destino;

        //Sons
        [XmlIgnore]
        public SoundEffectInstance SomEscada = Sons.Escada.CreateInstance(); 
        #endregion


        #region Construtor
        /// <summary>
        /// Cria um personagem para o jogo
        /// </summary>
        /// <param name="_principal">O jogo</param>
        /// <param name="_posicao">A posição na tela</param>
        /// <param name="_velocidadeIncremental">O incremento de velocidade para a movimentação do personagem</param>
        /// <param name="_vidas">A quantidade de vidas inicial</param>
        /// <param name="_textura">O spritesheet do personagem</param>
        public Personagem(Principal _principal, Vector2 _posicao, float _velocidadeIncremental, int _vidas, String _textura):base(_principal)
        {
            this.principal = _principal;
            this.posicao = _posicao;
            this.velocidadeIncremental = _velocidadeIncremental;
            this.vidas = _vidas;
            this.textura = _textura;

            //Animações
            animacao = new AnimacaoDeSprites();

            Animacao parado = new Animacao(78, 192, 1, 0, 0);
            Animacao ima = new Animacao(312, 192, 4, 78, 0);
            Animacao morrendo = new Animacao(320, 192, 4, 10, true, false, 390, 0);
            Animacao andando = new Animacao(512, 192, 8, 10, true, false, 0, 192);
            Animacao pulando = new Animacao(448, 192, 7, 10, false, false, 0, 384);
            Animacao empurrando = new Animacao(832, 192, 8, 10, true, false, 0, 576);
            Animacao subindo = new Animacao(800, 192, 10, 10, true, false, 0, 768);
            Animacao manivela = new Animacao(588, 192, 7, 10, true, false, 0, 960);

            animacao.Animacoes.Add("parado", parado);
            animacao.Animacoes.Add("ima", ima);
            animacao.Animacoes.Add("morrendo", morrendo);
            animacao.Animacoes.Add("andando", andando);
            animacao.Animacoes.Add("pulando", pulando);
            animacao.Animacoes.Add("empurrando", empurrando);
            animacao.Animacoes.Add("subindo", subindo);
            animacao.Animacoes.Add("manivela", manivela);

            SomEscada.Volume = 0.5f;
        }
        #endregion


        #region Propriedades
        public float Friccao
        {
            get { return friccao; }
            set { friccao = value; }
        }

        public Vector2 Medidas
        {
            get { return medidas; }
            set { medidas = value; }
        }

        public Vector2 Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public Vector2 Velocidade
        {
            get { return velocidade; }
            set { velocidade = value; }
        }

        public int Vidas
        {
            get { return vidas; }
            set { vidas = value; }
        }

        public int HP
        {
            get { return hp; }
            set { hp = value; }
        }

        public float Pulo
        {
            get { return pulo; }
            set { pulo = value; }
        }

        public float Peso
        {
            get { return peso; }
            set { peso = value; }
        }

        public float Forca
        {
            get { return forca; }
            set { forca = value; }
        }

        public bool Morto
        {
            get { return morto; }
            set { morto = value; }
        }

        public bool Pulando
        {
            get { return pulando; }
            set { pulando = value; }
        }

        public bool Subindo
        {
            get { return subindo; }
            set { subindo = value; }
        }

        public bool Caindo
        {
            get { return caindo; }
            set { caindo = true; }
        }

        public bool Passando
        {
            get { return passando; }
            set { passando = value; }
        }

        public Vector2 Bordas
        {
            get { return new Vector2(posicao.X + medidas.X, posicao.Y + medidas.Y); }
        }

        public bool Agua
        {
            get { return agua; }
            set { agua = value; }
        }

        public float VelocidadeIncremental
        {
            get { return velocidadeIncremental; }
        }

        public AnimacaoDeSprites Animacao
        {
            get { return animacao; }
            set { animacao = value; }
        }

        public Rectangle HitTest
        {
            get
            {
                retangulo.X = (int)posicao.X;
                retangulo.Y = (int)posicao.Y;
                if (animacao.AnimacaoAtual != null)
                {
                    retangulo.Height = animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual.Height;
                }
                else
                {
                    retangulo.Height = (int)medidas.Y;
                }
                return retangulo;
            }
        }

        public int ObjetivosCompletos
        {
            get { return objetivosCompletos; }
            set { objetivosCompletos = value; }
        }

        public int ItensUsados
        {
            get { return itensUsados; }
            set { itensUsados = value; }
        }

        public String Textura
        {
            get { return textura; }
            set { textura = value; }
        }
        #endregion


        #region Metodos
        public void pular()
        {
            if (!pulando && velocidade.Y <= 0f)
            {
                velocidade.Y -= pulo;
                pulando = true;
                caindo = true;
                subindo = false;
                animacao.AnimacaoAtual = "pulando";
                animacao.iniciarAnimacao();
            }
        }

        public bool checaChao()
        {
            if (Bordas.Y + velocidade.Y >= principal.telaJogo.mapa.Medidas.Y * Tile.Dimensoes.Y && !morto)
            {
                morrer();
                tempoMorto = 0;
                return false;
            } 
            else
            {
                posicaoAtual = Mapa.pegaIndice(new Vector2(posicao.X + medidas.X / 2, Bordas.Y));
                posicaoAtual2 = Mapa.pegaIndice(new Vector2(Bordas.X - medidas.X / 2, Bordas.Y));
                posicaoFutura = Mapa.pegaIndice(new Vector2(posicao.X + medidas.X / 2, Bordas.Y + velocidade.Y));
                posicaoFutura2 = Mapa.pegaIndice(new Vector2(Bordas.X - medidas.X / 2, Bordas.Y + velocidade.Y));

                if ((!principal.telaJogo.mapa.checaPassavel(posicaoFutura) || !principal.telaJogo.mapa.checaPassavel(posicaoFutura2) || principal.telaJogo.mapa.checaNuvem(posicaoFutura) || principal.telaJogo.mapa.checaNuvem(posicaoFutura2)) && posicaoFutura.Y != posicaoAtual.Y)
                {
                    if (velocidade.Y > 5)
                    {
                        if (velocidade.Y > 18)
                        {
                            if (!Principal.Mudo) Sons.Queda.Play();
                        }
                        else
                        {
                            if (!Principal.Mudo) Sons.Queda.Play(0.3f, 1, 0);
                        }
                    }

                    if (this == principal.telaJogo.Personagem && velocidade.Y > 24 && !morto) morrer();

                    if (subindo)
                    {
                        animacao.AnimacaoAtual = "parado";
                        animacao.iniciarAnimacao();
                    }

                    posicao.Y = posicaoFutura.Y * Tile.Dimensoes.Y - medidas.Y - 0.01f;
                    velocidade.Y = 0f;

                    pulando = false;
                    caindo = false;
                    subindo = false;

                    return true;
                }
                else
                {
                    caindo = true;
                }

                return false;
            }
        }

        private void checaEscada()
        {
            posicaoAtual = Mapa.pegaIndice(new Vector2(posicao.X + medidas.X/2f, Bordas.Y));
            if (principal.telaJogo.mapa.checaEscada(posicaoAtual) && subindo)
            {
                velocidade.Y = 0;
                caindo = false;
            }
            else
            {
                subindo = false;
            }
        }

        private void checaTeto()
        {
            if (velocidade.Y < 0f)
            {
                float posicaoX = posicao.X + medidas.X / 2;
                float posicaoY = posicao.Y + velocidade.Y;
                int tileX = (int)Math.Floor(posicaoX / Tile.Dimensoes.X);
                int tileY = (int)Math.Floor(posicaoY / Tile.Dimensoes.Y + .1f);
                if (!principal.telaJogo.mapa.checaPassavel(new Vector2(tileX, tileY)))
                {
                    velocidade.Y = 0;
                    if (!pulando && !subindo)
                    {
                        animacao.AnimacaoAtual = "parado";
                        animacao.iniciarAnimacao();
                    }
                    else
                    {
                        animacao.pararAnimacao();
                    }
                }
            }
        }

        private void checaDirecao()
        {
            posicaoX = posicao.X - 1;
            if (velocidade.X > 0) posicaoX = Bordas.X + 1;
            tileX = (int)Math.Floor(posicaoX / Tile.Dimensoes.X);
            tileY = (int)Math.Floor(Bordas.Y / Tile.Dimensoes.Y - .1f);
            if (!principal.telaJogo.mapa.checaPassavel(new Vector2(tileX, tileY)) || !principal.telaJogo.mapa.checaPassavel(new Vector2(tileX, tileY - 1)) || !principal.telaJogo.mapa.checaPassavel(new Vector2(tileX, tileY - 2)))
            {
                if (velocidade.X > 0)
                {
                    posicao.X = tileX * Tile.Dimensoes.X - medidas.X;
                }
                else
                {
                    posicao.X = tileX * Tile.Dimensoes.X + medidas.X;
                }
                velocidade.X = 0;
                if (!pulando && !subindo)
                {
                    animacao.AnimacaoAtual = "parado";
                    animacao.iniciarAnimacao();
                }
                else
                {
                    animacao.pararAnimacao();
                }
            }
            else
            {
                if (!pulando) animacao.AnimacaoAtual = "andando";
            }
        }

        public void morrer()
        {
            if (this == principal.telaJogo.Personagem && !Principal.Mudo)
            {
                Sons.Golpe.Play();
                Sons.Morrendo.Play(0.8f, 0, 0);
            }

            vidas--;
            morto = true;
            tempoMorto = 2;
            animacao.AnimacaoAtual = "morrendo";
            animacao.iniciarAnimacao();
        }

        public void inicializar()
        {
            vidas = 3;
            objetivosCompletos = 0;
            itensUsados = 0;
            Resetar();
        }

        public void Resetar()
        {
            velocidade.X = 0;
            velocidade.Y = 0;

            if (this == principal.telaJogo.Personagem)
            {
                posicao.X = principal.telaJogo.mapa.Inicio.X * Tile.Dimensoes.X;
                posicao.Y = principal.telaJogo.mapa.Inicio.Y * Tile.Dimensoes.Y - 1;
            }

            pulando = false;
            subindo = false;
            caindo = false;
            morto = false;

            animacao.AnimacaoAtual = "parado";

            if (this == principal.telaJogo.Personagem)
            {
                principal.telaJogo.Fader.FadeIn();
                principal.telaJogo.mapa.Inimigos.Clear();
                principal.telaJogo.mapa.criarInimigos();
            }
        }
        #endregion


        #region MetodosPadrao
        public override void Initialize()
        {
            base.Initialize();
            LoadContent(principal.Content);
            inicializar();
        }

        public virtual void LoadContent(ContentManager Content)
        {
            sprite = Content.Load<Texture2D>(textura);
        }

        public override void Update(GameTime gameTime)
        {
            if (morto)
            {
                if (tempoMorto > 0)
                {
                    tempoMorto -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    
                    //Sobe aos poucos pra dar impressão de estar voando
                    velocidade = Vector2.Zero;
                    posicao.Y -= 0.4f;

                    animacao.AnimacaoAtual = "morrendo";
                    animacao.iniciarAnimacao();
                }
                else
                {
                    Resetar();
                }
            }
            else
            {
                //Aplicar física
                if (velocidade.X != 0) velocidade.X *= (friccao + Fisica.Friccao) / 2;
                if (caindo) velocidade.Y += Fisica.Gravidade;

                //Melhorar o desempenho se a velocidade estiver muito baixa, arredonda para 0
                if (Math.Abs(velocidade.X) < 0.1f) velocidade.X = 0;
                if (Math.Abs(velocidade.Y) < 0.1f) velocidade.Y = 0;
                if (subindo) checaEscada();

                //Se estiver andando em alguma direção, verifica se há colisão
                if (velocidade.X != 0) checaDirecao();

                if (velocidade.Y < 0)
                {
                    //Se estiver subindo, verifica se pode continuar subindo
                    checaTeto();
                }
                else if (velocidade.Y >= 0 && !subindo)
                {
                    //Se estiver descendo ou caindo,verifica se pode continuar descendo
                    checaChao();
                }

                //Atualiza a posição do personagem
                posicao.X += velocidade.X;
                posicao.Y += velocidade.Y;
            }

            //Atualizar animação
            animacao.Update(gameTime);

            medidas.Y = animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual.Height;

            base.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Mapa.Camera.Intersects(HitTest))
            {
                //Define o destino para desenhar o personagem
                offsetX = animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual.Width - 64;
                destino = new Rectangle((int)(posicao.X - offsetX / 2f), (int)posicao.Y + 1, animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual.Width, animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual.Height);

                //Desenha o personagem
                spriteBatch.Draw(sprite, destino, animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual, cor, 0f, Vector2.Zero, flip, 0f);
            }
        }
        #endregion
    }
}
