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
    /// Classe padrão para cordas no jogo
    /// </summary>
    [Serializable]
    public class Corda : IXmlSerializable
    {
        #region Variaveis
        protected Vector2 posicao = Vector2.Zero;
        protected Vector2 posicaoAtual;
        protected float angulo = 0f;
        protected int tamanho = 1;

        protected Texture2D textura;
        protected String nomeTextura;
        protected Color cor = Color.White;
        protected Rectangle retangulo;

        protected int largura = 1;
        protected int altura = 1;

        protected Item origem;
        protected Item destino;

        protected Rectangle hitTest = new Rectangle();

        protected int posX;
        protected int posY;
        #endregion


        #region Propriedades
        public Vector2 Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public String NomeTextura
        {
            get { return nomeTextura; }
            set { nomeTextura = value; }
        }

        [XmlIgnore]
        public Texture2D Textura
        {
            get { return textura; }
            set { textura = value; }
        }

        [XmlIgnore]
        public Item Origem
        {
            get { return origem; }
            set { origem = value; }
        }

        [XmlIgnore]
        public Item Destino
        {
            get { return destino; }
            set { destino = value; }
        }

        public float Angulo
        {
            get { return angulo; }
            set { angulo = value; }
        }

        public int Tamanho
        {
            get { return tamanho; }
            set
            {
                tamanho = value;
                largura = (int)(tamanho * (textura.Width * (float)Math.Cos(angulo * 3.14f/180)));
                altura = (int)(tamanho * textura.Width * (float)Math.Sin(angulo * 3.14f/180));
            }
        }

        public Color Cor
        {
            get { return cor; }
            set { cor = value; }
        }

        [XmlIgnore]
        public Rectangle HitTest
        {
            get
            {
                hitTest.X = (largura < 0) ? (int)(posicao.X + largura) : (int)posicao.X;
                hitTest.Y = (altura > 0) ? (int)(posicao.Y - altura) : (int)posicao.Y;

                hitTest.Width = Math.Abs((int)(tamanho * (textura.Width * (float)Math.Cos(angulo * 3.14f / 180))));
                hitTest.Height = Math.Abs((int)(tamanho * (textura.Width * (float)Math.Sin(angulo * 3.14f / 180))));

                return hitTest;
            }
        }
        #endregion


        #region Contrutor
        public Corda() { }

        public Corda(String nomeTextura, Vector2 posicao, int tamanho)
        {
            this.nomeTextura = nomeTextura;
            this.posicao = posicao;
            this.tamanho = tamanho;
        }

        public Corda(String nomeTextura, Vector2 posicao, int tamanho, float angulo)
            : this(nomeTextura, posicao, tamanho)
        {
            this.angulo = angulo;
        }

        public Corda(String nomeTextura, Vector2 origem, Vector2 destino)
            : this(nomeTextura, origem, (int)Vector2.Distance(origem, destino), (-(float)Math.Atan2((double)(destino.Y - origem.Y), (double)(destino.X - origem.X))) * 180/3.14f)
        { }
        #endregion


        #region Metodos Padrao
        public void LoadContent(ContentManager Content)
        {
            textura = Content.Load<Texture2D>(nomeTextura);

            retangulo = new Rectangle(0, 0, textura.Width, textura.Height);

            largura = (int)(tamanho * (textura.Width * (float)Math.Cos(angulo * 3.14f/180)));
            altura = (int)(tamanho * (textura.Width * (float)Math.Sin(angulo * 3.14f/180)));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            posicaoAtual = posicao;
            for (int i = 0; i < tamanho; i++)
            {
                spriteBatch.Draw(textura, posicaoAtual, retangulo, cor, -(angulo * 3.14f/180), new Vector2(0, (textura.Height/2)), 1, SpriteEffects.None, 0);
                posicaoAtual.X += textura.Width * (float)Math.Cos(angulo * 3.14f/180);
                posicaoAtual.Y -= textura.Width * (float)Math.Sin(angulo * 3.14f/180);
            }
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
                        this.posicao.X = reader.ReadElementContentAsFloat();
                        this.posicao.Y = reader.ReadElementContentAsFloat();
                        reader.MoveToContent();

                        reader.ReadToNextSibling("Angulo");
                        this.Angulo = float.Parse(reader.ReadElementContentAsString());

                        this.tamanho = reader.ReadElementContentAsInt();

                        this.NomeTextura = reader.ReadElementContentAsString();
 
                        sr = new XmlSerializer(typeof(Color));
                        this.Cor = (Color)sr.Deserialize(reader);
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Posicao");
            writer.WriteElementString("X", String.Format("{0}", Posicao.X));
            writer.WriteElementString("Y", String.Format("{0}", Posicao.Y));
            writer.WriteEndElement();

            writer.WriteElementString("Angulo", String.Format("{0}", Angulo));

            writer.WriteElementString("Tamanho", String.Format("{0}", Tamanho));

            writer.WriteElementString("NomeTextura", String.Format("{0}", NomeTextura));

            XmlSerializer sr = new XmlSerializer(typeof(Color));
            sr.Serialize(writer, Cor);
        }
        #endregion


        #region Metodos
        public void Recalcular()
        {
            if (origem != null && destino != null)
            {
                posicao.X = origem.Posicao.X * Tile.Dimensoes.X;
                posicao.Y = origem.Posicao.Y * Tile.Dimensoes.Y;
                angulo = (-(float)Math.Atan2((destino.Posicao.Y - origem.Posicao.Y), (destino.Posicao.X - origem.Posicao.X))) * 180 / 3.14f;
                tamanho = (int)Vector2.Distance(origem.Posicao, destino.Posicao);
            }
            else
            {
                angulo = 0;
                tamanho = 0;
                posicao.X = 0;
                posicao.Y = 0;
            }
        }
        #endregion
    }
}
