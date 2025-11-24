using System;
using System.Xml.Serialization;
using Jogo.Componentes;
using Jogo.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Xml.Schema;
using System.Xml;

namespace Jogo.Personagens
{
    #region Enums
    public enum IA
    {
        Simples = 1,
        Perseguir,
    }

    public enum Direcao
    {
        Esquerda = -1,
        Direita = 1,
    }
    #endregion

    /// <summary>
    /// Clase base para os inimigos
    /// </summary>
    [Serializable]
    public class Inimigo : Personagem, IXmlSerializable
    {
        #region Variï¿½veis
        //IA
        protected float tempoEspera;
        protected float tempoEsperaMax = 2f;
        protected IA inteligencia = IA.Simples;
        protected bool perseguidor = false;
        protected Direcao direcao = Direcao.Direita;
        protected float tempoParado = 0;
        protected Vector2 posicaoAnterior;
        #endregion


        #region Propriedades
        public new float VelocidadeIncremental
        {
            get { return velocidadeIncremental; }
            set { velocidadeIncremental = value; }
        }

        public Direcao Direcao
        {
            get { return direcao; }
            set { direcao = value; }
        }

        public IA Inteligencia
        {
            get { return inteligencia; }
            set { inteligencia = value; }
        }

        public bool Perseguidor
        {
            get { return perseguidor; }
            set { perseguidor = value; }
        }

        public float Espera
        {
            get { return tempoEspera; }
            set { tempoEspera = value; }
        }

        public float EsperaMax
        {
            get { return tempoEsperaMax; }
            set { tempoEsperaMax = value; }
        }

        [XmlIgnore]
        public Principal Principal
        {
            get { return principal; }
            set { principal = value; }
        }
        #endregion


        #region Construtor
        public Inimigo() : base(null, Vector2.Zero, 0.5f, 1, "") { }

        public Inimigo(Principal _principal, Vector2 _posicao, float _velocidadeIncremental, String _textura)
            : base(_principal, _posicao, _velocidadeIncremental, 1, _textura)
        { }
        #endregion


        #region Metodos Padrao
        public override void Initialize()
        {
            //Animações
            animacao.Animacoes.Clear();

            Animacao parado = new Animacao(106, 256, 1, 0, 0);
            Animacao andando = new Animacao(924, 256, 7, 6, true, false, 0, 256);
            Animacao pulando = new Animacao(1056, 256, 8, 6, false, false, 0, 512);
            Animacao subindo = new Animacao(848, 256, 8, 6, true, false, 0, 768);

            animacao.Animacoes.Add("parado", parado);
            animacao.Animacoes.Add("andando", andando);
            animacao.Animacoes.Add("pulando", pulando);
            animacao.Animacoes.Add("subindo", subindo);

            animacao.AnimacaoAtual = "parado";

            base.Initialize();
            posicaoAnterior = posicao;
        }

        public override void Update(GameTime gameTime)
        {
            if (pulando) subindo = false;

            //Calcular a posicao do inimigo
            float posicaoX = posicao.X;

            if (direcao == Direcao.Esquerda)
            {
                posicaoX = posicao.X + medidas.X / 3;
            }
            else
            {
                posicaoX = Bordas.X - medidas.X / 3;
            }

            tileX = (int)Math.Floor(posicaoX / Tile.Dimensoes.X);
            tileY = (int)Math.Floor(Bordas.Y / Tile.Dimensoes.Y);

            //Se estiver numa altura diferente e não estiver numa escada ou pulando, passa a ter ai simples
            if (Math.Abs(principal.telaJogo.Personagem.Posicao.Y - posicao.Y) > HitTest.Height + 10 && !subindo && !pulando)
            {
                inteligencia = IA.Simples;
            }

            //Se o personagem tiver na tela da água e empurrar 3 ou mais itens na água, os seguranças ficam sempre alertas
            //Caso contrário...
            if (principal.telaJogo.Personagem.ItensUsados < 3 || !principal.telaJogo.mapa.Caminho.EndsWith("Agua"))
            {
                //Se estiver numa altura a 800 px de distância, passa a ter IA simples e deixa de ser perseguidor
                if (Math.Abs(principal.telaJogo.Personagem.Posicao.Y - posicao.Y) > 800 && !subindo && !pulando)
                {
                    inteligencia = IA.Simples;
                    perseguidor = false;
                }
            }

            if (inteligencia == IA.Simples)
            {
                AndarLados(tileX, tileY, gameTime);
            }
            else if (inteligencia == IA.Perseguir)
            {
                if (principal.telaJogo.Personagem.Posicao.X < posicao.X)
                {
                    direcao = Direcao.Esquerda;
                    flip = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    direcao = Direcao.Direita;
                    flip = SpriteEffects.None;
                }

                PerseguirLados(tileX, tileY, gameTime);
                PerseguirAcima(tileX, tileY);
            }

            //Se encostar no persoangem, ele morre
            if (principal.telaJogo.Personagem.Animacao.AnimacaoAtual != "null" && !principal.telaJogo.Personagem.Morto && !principal.telaJogo.Derrota && !principal.telaJogo.Pausado)
            {
                if (HitTest.Intersects(principal.telaJogo.Personagem.HitTest) && !principal.telaJogo.Personagem.Passando)
                {
                    principal.telaJogo.Personagem.morrer();
                }
            }

            //Para de se mover se o jogador perdeu ou se está esperando
            if (principal.telaJogo.Derrota || principal.telaJogo.Personagem.Morto || tempoEspera > 0)
            {
                velocidade.X = 0;
                velocidade.Y = 0;

                if (!subindo && !pulando)
                {
                    animacao.AnimacaoAtual = "parado";
                    animacao.iniciarAnimacao();
                }
                else if (subindo)
                {
                    animacao.pararAnimacao();
                }

                if (principal.telaJogo.Personagem.Morto)
                {
                    inteligencia = IA.Simples;
                    perseguidor = false;
                }
            }
            else
            {
                //caso contrário, se não estiver pulando ou subindo, muda para andar
                if (!pulando && !subindo)
                {
                    animacao.AnimacaoAtual = "andando";
                    animacao.iniciarAnimacao();
                }
            }


            //Ajuste se permanecer parado por um tempo maior que a espera máxima
            if (posicaoAnterior == posicao)
            {
                if (subindo)
                {
                    animacao.pararAnimacao();
                }
                else
                {
                    animacao.AnimacaoAtual = "parado";
                    animacao.iniciarAnimacao();
                }

                tempoParado += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (tempoParado > tempoEsperaMax + 1 || (subindo && tempoParado > tempoEsperaMax/2))
                {
                    pular();
                    tempoParado = 0;
                }
            }
            else
            {
                tempoParado = 0;
            }

            posicaoAnterior = posicao;

            base.Update(gameTime);
        }
        #endregion


        #region Metodos
        public virtual void AndarLados(int tileX, int tileY, GameTime gameTime)
        {
            //Se tiver tempo de espera
            if (tempoEspera > 0)
            {
                //Espera acabar o tempo
                tempoEspera = Math.Max(0f, tempoEspera - (float)gameTime.ElapsedGameTime.TotalSeconds);

                //Se o tempo acabar
                if (tempoEspera <= 0)
                {
                    //Muda de direção do movimento
                    direcao = (Direcao)(-(int)direcao);

                    //Muda a direção do sprite
                    if (direcao == Direcao.Direita)
                    {
                        flip = SpriteEffects.None;
                    }
                    else
                    {
                        flip = SpriteEffects.FlipHorizontally;
                    }
                }
            }
            //Se não, se movimenta na direção atual
            else
            {
                //se não puder passar ou se for cair da plataforma
                if (!caindo && !pulando && !subindo && (!principal.telaJogo.mapa.checaPassavel(new Vector2(tileX + (int)direcao, tileY)) || !principal.telaJogo.mapa.checaPassavel(new Vector2(tileX + (int)direcao, tileY - 1)) || !principal.telaJogo.mapa.checaPassavel(new Vector2(tileX + (int)direcao, tileY - 2)) || (principal.telaJogo.mapa.checaPassavel(new Vector2(tileX + (int)direcao, tileY + 1)) && !principal.telaJogo.mapa.checaNuvem(new Vector2(tileX + (int)direcao, tileY + 1)))))
                {
                    //Começa a esperar
                    tempoEspera = tempoEsperaMax;
                    velocidade.X = 0;
                    velocidade.Y = 0;
                }
                //Se puder passar e não estiver subindo
                else if (!subindo)
                {
                    //movimenta-se na direção atual
                    velocidade.X += velocidadeIncremental * (int)direcao;
                }
            }

            //Se ver o personagem se torna perseguidor
            if (principal.telaJogo.Personagem.Posicao.Y <= Bordas.Y && principal.telaJogo.Personagem.Bordas.Y >= posicao.Y && Math.Abs(principal.telaJogo.Personagem.Posicao.X - posicao.X) <= 800)
            {
                if (perseguidor || (principal.telaJogo.Personagem.Bordas.X <= Bordas.X && direcao == Direcao.Esquerda) || (principal.telaJogo.Personagem.Posicao.X >= posicao.X && direcao == Direcao.Direita))
                {
                    inteligencia = IA.Perseguir;
                    perseguidor = true;
                }
            }

            //Se for perseguidor, persegue pela escada
            if (perseguidor)
            {
                PerseguirAcima(tileX, tileY);
            }
        }

        public virtual void PerseguirLados(int tileX, int tileY, GameTime gameTime)
        {
            if (!subindo && principal.telaJogo.Personagem.Posicao.Y <= Bordas.Y && principal.telaJogo.Personagem.Bordas.Y >= posicao.Y)
            {
                //Se estiver a esquerda
                if (principal.telaJogo.Personagem.Posicao.X <= posicao.X)
                {
                    //Se estiver a esquerda anda para a direita
                    velocidade.X -= velocidadeIncremental;

                    if (!pulando)
                    {
                        animacao.AnimacaoAtual = "andando";
                        animacao.iniciarAnimacao();
                    }
                }
                else if (principal.telaJogo.Personagem.Posicao.X > posicao.X)
                {
                    //Se a direita anda para a direita
                    velocidade.X += velocidadeIncremental;

                    if (!pulando)
                    {
                        animacao.AnimacaoAtual = "andando";
                        animacao.iniciarAnimacao();
                    }
                }

                if (principal.telaJogo.mapa.checaPassavel(new Vector2(tileX + (int)direcao, tileY)) && (principal.telaJogo.mapa.checaPassavel(new Vector2(tileX + (int)direcao, tileY + 1)) && !principal.telaJogo.mapa.checaNuvem(new Vector2(tileX + (int)direcao, tileY + 1))))
                {
                    pular();
                }
            }
            else if (!subindo)
            {
                //inteligencia = IA.Simples;
            }
        }

        public virtual void PerseguirAcima(int tileX, int tileY)
        {
            //Se o personagem estiver mais alto que o inimigo
            if (principal.telaJogo.Personagem.Bordas.Y < Bordas.Y)
            {
                if (!pulando)
                {
                    //Se estiver em uma escada
                    if (principal.telaJogo.mapa.checaEscada(new Vector2(tileX, tileY - 1)))
                    {
                        //Sobe
                        if (!Principal.Mudo)
                        {
                            if (SomEscada.State == SoundState.Stopped) SomEscada.Play();
                        }

                        subindo = true;
                        pulando = false;
                        velocidade.X = 0;
                        velocidade.Y = 0;
                        posicao.Y -= velocidadeIncremental;
                        posicao.X = tileX * Tile.Dimensoes.X;
                        animacao.AnimacaoAtual = "subindo";
                        animacao.iniciarAnimacao();
                    }
                    else if (!principal.telaJogo.mapa.checaEscada(new Vector2(tileX, tileY)))
                    {
                        if (SomEscada.State != SoundState.Stopped) SomEscada.Stop();
                        subindo = false;
                    }
                    else if (subindo)
                    {
                        animacao.pararAnimacao();
                    }
                }
                else
                {
                    subindo = false;
                }
            }
            //Se o personagem estiver abaixo e o inimigo estiver em uma escada
            else if (principal.telaJogo.Personagem.Bordas.Y > Bordas.Y)
            {
                if (!pulando)
                {
                    if (principal.telaJogo.mapa.checaEscada(new Vector2(tileX, tileY + 1)))
                    {
                        //Desce
                        if (SomEscada.State == SoundState.Stopped)
                        {
                            if (!Principal.Mudo) SomEscada.Play();
                        }

                        subindo = true;
                        pulando = false;
                        velocidade.X = 0;
                        velocidade.Y = 0;
                        posicao.Y += velocidadeIncremental;
                        posicao.X = tileX * Tile.Dimensoes.X;
                        animacao.AnimacaoAtual = "subindo";
                        animacao.iniciarAnimacao();
                    }
                    else if (!principal.telaJogo.mapa.checaEscada(new Vector2(tileX, tileY - 1)))
                    {
                        if (SomEscada.State != SoundState.Stopped) SomEscada.Stop();
                        subindo = false;
                    }
                    else if (subindo)
                    {
                        animacao.pararAnimacao();
                    }
                }
                else
                {
                    subindo = false;
                }
            }
        }
        #endregion

        #region XmlSerialize Methods

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer sr = new XmlSerializer(typeof(Vector2));

            reader.ReadStartElement(); // Read the start tag of the element
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.MoveToContent();
                if (reader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case nameof(this.posicao):
                            this.posicao = (Vector2)sr.Deserialize(reader);
                            break;
                        case nameof(this.velocidadeIncremental):
                            this.velocidadeIncremental = reader.ReadElementContentAsFloat();
                            break;
                        case nameof(this.vidas):
                            this.vidas = reader.ReadElementContentAsInt();
                            break;
                        case nameof(this.textura):
                            this.textura = reader.ReadElementContentAsString();
                            break;
                        case nameof(this.direcao):
                            this.direcao = Enum.Parse<Direcao>(reader.ReadElementContentAsString());
                            break;
                        case nameof(this.inteligencia):
                            this.inteligencia = Enum.Parse<IA>(reader.ReadElementContentAsString());
                            break;
                        case nameof(this.perseguidor):
                            this.perseguidor = reader.ReadElementContentAsBoolean();
                            break;
                        case nameof(this.tempoEspera):
                            this.tempoEspera = reader.ReadElementContentAsFloat();
                            break;
                        case nameof(this.tempoEsperaMax):
                            this.tempoEsperaMax = reader.ReadElementContentAsFloat();
                            break;
                        default:
                            reader.Skip(); // Skip unknown elements
                            break;
                    }
                }
                else
                {
                    reader.Read(); // Advance the reader
                }
            }
            reader.ReadEndElement(); // Read the end tag of the element
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer sr = new XmlSerializer(typeof(Vector2));

            sr.Serialize(writer, this.posicao);

            writer.WriteElementString(nameof(this.velocidadeIncremental), this.velocidadeIncremental.ToString());
            writer.WriteElementString(nameof(this.vidas), this.vidas.ToString());
            writer.WriteElementString(nameof(this.textura), this.textura.ToString());

            writer.WriteElementString(nameof(this.direcao), this.direcao.ToString());
            writer.WriteElementString(nameof(this.inteligencia), this.inteligencia.ToString());
            writer.WriteElementString(nameof(this.perseguidor), this.perseguidor.ToString().ToLower());
            writer.WriteElementString(nameof(this.tempoEspera), this.tempoEspera.ToString());
            writer.WriteElementString(nameof(this.tempoEsperaMax), this.tempoEsperaMax.ToString());
        }
        #endregion
    }
}