using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Jogo.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jogo.Componentes
{
    /// <summary>
    /// Classe base para itens do jogo (coletáveis ou não)
    /// </summary>
    [Serializable]
    public class Item : GameComponent, IXmlSerializable
    {
        #region Variaveis
        protected Principal principal;
        protected Vector2 posicao;
        protected Vector2 posicaoReal;
        protected Vector2 velocidade = Vector2.Zero;
        protected Texture2D textura;
        protected String nomeTextura;
        protected String nomeTexturaHUD;
        protected String nomeTexturaCompleta;
        protected Color cor = Color.White;
        protected bool passavel;
        protected bool nuvem;
        protected bool coletavel;
        protected String tipo;
        protected AnimacaoDeSprites animacao;
        protected Rectangle retangulo = new Rectangle(0, 0, 64, 64);
        protected bool ativado = false;
        protected bool objetivo = false;
        protected int peso;
        #endregion


        #region Propriedades
        public Vector2 Posicao
        {
            get { return posicao; }
            set
            {
                posicao = value;
                posicaoReal.X = value.X * Tile.Dimensoes.X;
                posicaoReal.Y = value.Y * Tile.Dimensoes.Y;
            }
        }

        public Vector2 PosicaoReal
        {
            get { return posicaoReal; }
            set
            {
                posicaoReal = value;
                posicao.X = (int)(value.X / Tile.Dimensoes.X);
                posicao.Y = (int)(value.Y / Tile.Dimensoes.Y);
            }
        }

        public Vector2 Velocidade
        {
            get { return velocidade; }
            set { velocidade = value; }
        }

        public String NomeTextura
        {
            get { return nomeTextura; }
            set { nomeTextura = value; }
        }

        public String NomeTexturaHUD
        {
            get { return nomeTexturaHUD; }
            set { nomeTexturaHUD = value; }
        }

        public String NomeTexturaCompleta
        {
            get { return nomeTexturaCompleta; }
            set { nomeTexturaCompleta = value; }
        }

        public int Peso
        {
            get { return peso; }
            set { peso = value; }
        }

        public bool Passavel
        {
            get { return passavel; }
            set { passavel = value; }
        }

        public bool Nuvem
        {
            get { return nuvem; }
            set { nuvem = value; }
        }

        public bool Coletavel
        {
            get { return coletavel; }
            set { coletavel = value; }
        }

        public String Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        public Color Cor
        {
            get { return cor; }
            set { cor = value; }
        }

        public AnimacaoDeSprites Animacao
        {
            get { return animacao; }
            set { animacao = value; }
        }

        public String AnimacaoAtual
        {
            get { return animacao.AnimacaoAtual; }
            set { animacao.AnimacaoAtual = value; }
        }

        public bool Ativado
        {
            get { return ativado; }
            set { ativado = value; }
        }

        public bool Objetivo
        {
            get { return objetivo; }
            set { objetivo = value; }
        }

        [ContentSerializer]
        public int ItemInterativo { get; set; }

        [ContentSerializer]
        public int CordaInterativa { get; set; }

        [ContentSerializer]
        public int CordaDe { get; set; }

        [XmlIgnore]
        public Texture2D Textura
        {
            get { return textura; }
            set { textura = value; }
        }

        [XmlIgnore]
        public Rectangle HitTest
        {
            get
            {
                retangulo = animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual;
                retangulo.X = (int)posicaoReal.X + 1;
                retangulo.Y = (int)posicaoReal.Y + 1;
                retangulo.Width -= 2;
                retangulo.Height -= 2;
                return retangulo;
            }
            set { retangulo = value; }
        }
        #endregion


        #region Construtor
        public Item()
            : base(null)
        {
            animacao = new AnimacaoDeSprites();
        }

        /// <summary>
        /// Cria um item no jogo
        /// </summary>
        /// <param name="_principal">O jogo</param>
        /// <param name="_posicao">A posição do item no mapa</param>
        /// <param name="_passavel">Se o item pode ser atravessado ou não</param>
        /// <param name="_coletavel">Se o item pode ser coletado ou não</param>
        /// <param name="_textura">O sprite do item</param>
        public Item(Principal _principal, Vector2 _posicao, bool _passavel, bool _coletavel, bool _nuvem, String _nomeTextura)
            : base(_principal)
        {
            this.principal = _principal;
            this.posicao = _posicao;
            this.posicaoReal.X = _posicao.X * Tile.Dimensoes.X;
            this.posicaoReal.Y = _posicao.Y * Tile.Dimensoes.Y;
            this.nomeTextura = _nomeTextura;
            this.passavel = _passavel;
            this.nuvem = _nuvem;
            this.coletavel = _coletavel;

            animacao = new AnimacaoDeSprites();
        }

        /// <summary>
        /// Cria um item coletável no jogo
        /// </summary>
        /// <param name="_principal">O jogo</param>
        /// <param name="_posicao">A posição do item no mapa</param>
        /// <param name="_textura">O sprite do item</param>
        public Item(Principal _principal, Vector2 _posicao, String _nomeTextura)
            : this(_principal, _posicao, true, true, false, _nomeTextura) { }
        #endregion


        #region Metodos Padrao
        public override void Initialize()
        {
            ItemInterativo = -1;
            CordaInterativa = -1;
            CordaDe = -1;
            base.Initialize();
            LoadContent(principal.Content);
        }

        public virtual void LoadContent(ContentManager Content)
        {
            textura = Content.Load<Texture2D>(nomeTextura);
        }

        public override void Update(GameTime gameTime)
        {
            animacao.Update(gameTime);
            if (animacao.AnimacaoAtual != null)
            {
                if (animacao.atualizarAnimacao && tipo != "vidro")
                {
                    if (animacao.AnimacaoAtual == "indo" && animacao.Animacoes["indo"].QuadroAtual == animacao.Animacoes["indo"].Quadros.Length - 1)
                    {
                        animacao.AnimacaoAtual = "voltando";
                        animacao.iniciarAnimacao();
                    }
                    else if (animacao.AnimacaoAtual == "voltando" && animacao.Animacoes["voltando"].QuadroAtual == 0)
                    {
                        animacao.AnimacaoAtual = "indo";
                        animacao.iniciarAnimacao();
                    }
                }
            }
            base.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            retangulo = animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual;
            retangulo.X = (int)posicaoReal.X;
            retangulo.Y = (int)posicaoReal.Y;

            if (tipo == "empurravel" || tipo == "roldanaSuporte" || tipo == "save" || nomeTextura.Contains("LAMPADA") || !passavel || coletavel || nuvem)
            {
                cor = Color.White;
            }
            else
            {
                cor.R = 210;
                cor.G = 210;
                cor.B = 210;
            }

            if (Mapa.Camera.Intersects(retangulo))
            {
                spriteBatch.Draw(textura, retangulo, animacao.Animacoes[animacao.AnimacaoAtual].RetanguloQuadroAtual, cor);
            }
        }

        public override string ToString()
        {
            return String.Format("Item: {0}\r\nColetável: {1}\r\nPassável: {2}\r\nNuvem: {3}\r\nPosicao: X {4} x Y {5}\r\nPosicaoReal: X {6} x Y {7}\r\nTextura: {8}", tipo, coletavel, passavel, nuvem, posicao.X, posicao.Y, posicaoReal.X, posicaoReal.Y, nomeTextura);
        }
        #endregion


        #region Metodos IXmlSerializable
        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer sr = new XmlSerializer(typeof(int));
            int depth = reader.Depth;

            if (!reader.IsEmptyElement && reader.Read())
            {
                try
                {
                    if (reader.Depth == depth + 1 && reader.NodeType != XmlNodeType.EndElement)
                    {
                        sr = new XmlSerializer(typeof(float));
                        reader.MoveToContent();
                        reader.Read();
                        this.posicaoReal.X = reader.ReadElementContentAsFloat();
                        this.posicaoReal.Y = reader.ReadElementContentAsFloat();
                        reader.MoveToContent();

                        reader.ReadToNextSibling("Velocidade");

                        reader.MoveToContent();
                        reader.Read();
                        this.velocidade.X = reader.ReadElementContentAsFloat();
                        this.velocidade.Y = reader.ReadElementContentAsFloat();
                        reader.MoveToContent();

                        reader.ReadToNextSibling("NomeTextura");
                        this.nomeTextura = reader.ReadElementContentAsString();
                        this.nomeTexturaCompleta = reader.ReadElementContentAsString();
                        this.nomeTexturaHUD = reader.ReadElementContentAsString();
                        this.tipo = reader.ReadElementContentAsString();

                        Boolean.TryParse((string)reader.ReadElementContentAsString(), out this.nuvem);
                        Boolean.TryParse((string)reader.ReadElementContentAsString(), out this.coletavel);
                        Boolean.TryParse((string)reader.ReadElementContentAsString(), out this.passavel);
                        Boolean.TryParse((string)reader.ReadElementContentAsString(), out this.ativado);
                        Boolean.TryParse((string)reader.ReadElementContentAsString(), out this.objetivo);

                        sr = new XmlSerializer(typeof(AnimacaoDeSprites));
                        this.Animacao = (AnimacaoDeSprites)sr.Deserialize(reader);


                        this.ItemInterativo = reader.ReadElementContentAsInt();
                        this.CordaInterativa = reader.ReadElementContentAsInt();
                        this.CordaDe = reader.ReadElementContentAsInt();
                        this.Peso = reader.ReadElementContentAsInt();

                        sr = new XmlSerializer(typeof(Color));
                        this.Cor = (Color)sr.Deserialize(reader);

                        this.AnimacaoAtual = reader.ReadElementContentAsString();
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                catch (Exception)
                {
                    // ignora erros
                }
            }
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("PosicaoReal");
            writer.WriteElementString("X", String.Format("{0}", PosicaoReal.X));
            writer.WriteElementString("Y", String.Format("{0}", PosicaoReal.Y));
            writer.WriteEndElement();

            writer.WriteStartElement("Velocidade");
            writer.WriteElementString("X", String.Format("{0}", Velocidade.X));
            writer.WriteElementString("Y", String.Format("{0}", Velocidade.Y));
            writer.WriteEndElement();

            writer.WriteElementString("NomeTextura", String.Format("{0}", NomeTextura));
            writer.WriteElementString("NomeTexturaCompleta", String.Format("{0}", NomeTexturaCompleta));
            writer.WriteElementString("NomeTexturaHUD", String.Format("{0}", NomeTexturaHUD));
            writer.WriteElementString("Tipo", String.Format("{0}", Tipo));

            writer.WriteElementString("Nuvem", String.Format("{0}", Nuvem));
            writer.WriteElementString("Coletavel", String.Format("{0}", Coletavel));
            writer.WriteElementString("Passavel", String.Format("{0}", Passavel));
            writer.WriteElementString("Ativado", String.Format("{0}", Ativado));
            writer.WriteElementString("Objetivo", String.Format("{0}", Objetivo));

            XmlSerializer sr = new XmlSerializer(typeof(AnimacaoDeSprites));
            sr.Serialize(writer, Animacao);

            writer.WriteElementString("ItemInterativo", String.Format("{0}", ItemInterativo));
            writer.WriteElementString("CordaInterativa", String.Format("{0}", CordaInterativa));
            writer.WriteElementString("CordaDe", String.Format("{0}", CordaDe));
            writer.WriteElementString("Peso", String.Format("{0}", Peso));

            sr = new XmlSerializer(typeof(Color));
            sr.Serialize(writer, Cor);

            writer.WriteElementString("AnimacaoAtual", String.Format("{0}", AnimacaoAtual));
        }
        #endregion
    }
}
