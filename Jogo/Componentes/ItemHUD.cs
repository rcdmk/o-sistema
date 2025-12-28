using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jogo.Componentes
{
    [Serializable]
    public class ItemHUD : GameComponent, IXmlSerializable
    {
        #region Variaveis
        private Principal principal;
        private Vector2 posicao = Vector2.Zero;
        private Vector2 posicaoHUD = Vector2.Zero;
        private Texture2D textura;
        private String nomeTextura;
        private String nome;
        private String tipo;
        private static int espacoItens = 8;
        private Rectangle destino = new Rectangle(0, 0, (int)HUD.tamanhoItens.X, (int)HUD.tamanhoItens.Y);
        #endregion


        #region Propriedades
        public Vector2 Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public Vector2 PosicaoHUD
        {
            get { return posicaoHUD; }
            set { posicaoHUD = value; }
        }

        public String Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        public String Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        [XmlIgnore]
        public Texture2D Textura
        {
            get { return textura; }
            set { textura = value; }
        }

        public String NomeTextura
        {
            get { return nomeTextura; }
            set { nomeTextura = value; }
        }

        public static int EspacoItens
        {
            get { return espacoItens; }
            set { espacoItens = value; }
        }

        #endregion


        #region Contrutor
        public ItemHUD() : base(null) { }

        public ItemHUD(Principal _principal, Vector2 _posicao, String _tipo, String _nomeTextura) : base(_principal)
        {
            this.principal = _principal;
            this.posicao = _posicao;
            this.tipo = _tipo;
            this.nomeTextura = _nomeTextura;
        }
        #endregion


        #region Metodos Padrao
        public override void Initialize()
        {
            LoadContent(principal.Content);
            base.Initialize();
        }

        public void LoadContent(ContentManager Content)
        {
            textura = Content.Load<Texture2D>(nomeTextura);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            destino.X = (int)posicaoHUD.X + (int)posicao.X;
            destino.Y = (int)posicaoHUD.Y + (int)posicao.Y;

            spriteBatch.Draw(textura, destino, Color.White);
        }

        public override string ToString()
        {
            return String.Format("Item: {0}", nome);
        }

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
                        case nameof(this.posicaoHUD):
                            this.posicaoHUD = (Vector2)sr.Deserialize(reader);
                            break;
                        case nameof(this.nome):
                            this.nome = reader.ReadElementContentAsString();
                            break;
                        case nameof(this.tipo):
                            this.tipo = reader.ReadElementContentAsString();
                            break;
                        case nameof(this.nomeTextura):
                            this.nomeTextura = reader.ReadElementContentAsString();
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
            sr.Serialize(writer, this.posicaoHUD);

            writer.WriteElementString(nameof(this.nome), this.nome);
            writer.WriteElementString(nameof(this.tipo), this.tipo);
            writer.WriteElementString(nameof(this.nomeTextura), this.nomeTextura);
        }
        #endregion
    }
}