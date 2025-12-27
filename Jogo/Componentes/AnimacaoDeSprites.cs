using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jogo.Componentes
{
    /// <summary>
    /// Classe base para anima��es com controle de framerate
    /// (Retirada de www.ziggyware.com e modificada por mim RCDMK)
    /// </summary>
    [Serializable]
    public class Animacao
    {
        #region Variaveis
        int largura;
        int altura;
        int offsetX;
        int offsetY;
        int numQuadros;

        Rectangle[] quadros;
        float duracaoQuadros = 1f / 5f;
        float timer = 0f;
        int quadroAtual = 0;
        bool loop = false;
        bool reverso = false;
        bool vertical = false;
        #endregion


        #region Propriedades
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

        public int OffsetX
        {
            get { return offsetX; }
            set { offsetX = value; }
        }

        public int OffsetY
        {
            get { return offsetY; }
            set { offsetY = value; }
        }

        public int NumQuadros
        {
            get { return numQuadros; }
            set { numQuadros = value; }
        }

        public bool Loop
        {
            get { return loop; }
            set { loop = value; }
        }

        public bool Reverso
        {
            get { return reverso; }
            set { reverso = value; }
        }

        public bool Vertical
        {
            get { return vertical; }
            set { vertical = value; }
        }

        public Rectangle[] Quadros
        {
            get { return quadros; }
            set
            {
                GerarRetangulos();
            }
        }

        [XmlIgnore]
        public Rectangle RetanguloQuadroAtual
        {
            get { return quadros[quadroAtual]; }
        }

        [XmlIgnore]
        public int QuadrosPorSegundo
        {
            get { return (int)(1f / duracaoQuadros); }
            set { duracaoQuadros = 1f / (float)value; }
        }

        [XmlIgnore]
        public int QuadroAtual
        {
            get { return quadroAtual; }
            set
            {
                if (value <= 0)
                {
                    quadroAtual = 0;
                }
                else if (value >= quadros.Length - 1)
                {
                    quadroAtual = quadros.Length - 1;
                }
                else
                {
                    quadroAtual = value;
                }
            }
        }
        #endregion


        #region Construtor
        /// <summary>
        /// Cria uma anima��o.
        /// Construtor vazio para carregar dados posteriormente
        /// </summary>
        public Animacao() { }

        /// <summary>
        /// Cria uma nova anima��o
        /// </summary>
        /// <param name="largura">A largura total da anima��o</param>
        /// <param name="altura">A altura total da anima��o</param>
        /// <param name="numQuadros">O n�mero de quadros da anima��o</param>
        /// <param name="OffsetX">O offset no eixo x de onde come�a a anima��o na spritesheet</param>
        /// <param name="OffsetY">O offset no eixo Y de onde come�a a anima��o na spritesheet</param>
        public Animacao(int largura, int altura, int numQuadros, int OffsetX, int OffsetY)
        {
            this.largura = largura;
            this.altura = altura;
            this.numQuadros = numQuadros;
            this.offsetX = OffsetX;
            this.offsetY = OffsetY;

            GerarRetangulos();
        }

        /// <summary>
        /// Cria uma nova anima��o com defini��o de quadros por segundo
        /// </summary>
        /// <param name="largura">A largura total da anima��o</param>
        /// <param name="altura">A altura total da anima��o</param>
        /// <param name="numQuadros">O n�mero de quadros da anima��o</param>
        /// <param name="quadrosPorSegundo">N�mero de quadros por segundo da anima��o (velocidade)</param>
        /// <param name="loop">Flag para anima��o passar continuamente (em loop) ou n�o</param>
        /// <param name="vertical">Flag para anima��o ser gerada verticalemente na spritesheet ou n�o</param>
        /// <param name="OffsetX">O offset no eixo x de onde come�a a anima��o na spritesheet</param>
        /// <param name="OffsetY">O offset no eixo Y de onde come�a a anima��o na spritesheet</param>
        public Animacao(int largura, int altura, int numQuadros, int quadrosPorSegundo, bool loop, bool vertical, int OffsetX, int OffsetY)
        {
            this.largura = largura;
            this.altura = altura;
            this.numQuadros = numQuadros;
            this.offsetX = OffsetX;
            this.offsetY = OffsetY;
            this.loop = loop;
            this.vertical = vertical;
            this.duracaoQuadros = 1f / (float)quadrosPorSegundo;

            GerarRetangulos();
        }

        /// <summary>
        /// Cria uma nova anima��o com reverso
        /// </summary>
        /// <param name="largura">A largura total da anima��o</param>
        /// <param name="altura">A altura total da anima��o</param>
        /// <param name="numQuadros">O n�mero de quadros da anima��o</param>
        /// <param name="quadrosPorSegundo"></param>
        /// <param name="loop">Flag para anima��o passar continuamente (em loop) ou n�o</param>
        /// <param name="vertical">Flag para anima��o ser gerada verticalemente na spritesheet ou n�o</param>
        /// <param name="reverso">Flag para anima��o ser gerada invertida na spritesheet ou n�o</param>
        /// <param name="OffsetX">O offset no eixo x de onde come�a a anima��o na spritesheet</param>
        /// <param name="OffsetY">O offset no eixo Y de onde come�a a anima��o na spritesheet</param>
        public Animacao(int largura, int altura, int numQuadros, int quadrosPorSegundo, bool loop, bool vertical, bool reverso, int OffsetX, int OffsetY)
        {
            this.largura = largura;
            this.altura = altura;
            this.numQuadros = numQuadros;
            this.offsetX = OffsetX;
            this.offsetY = OffsetY;
            this.loop = loop;
            this.vertical = vertical;
            this.reverso = reverso;
            this.duracaoQuadros = 1f / (float)quadrosPorSegundo;

            GerarRetangulos();
        }
        #endregion


        #region Metodos Padrao
        /// <summary>
        /// Atualiza a anima��o
        /// </summary>
        /// <param name="gameTime">O tempo de jogo atual</param>
        public void Update(GameTime gameTime)
        {
            //incrementa o timer com os segundos passados
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //se o timer alcan�ar a duara��o dos quadros
            if (timer >= duracaoQuadros)
            {
                //reseta o timer
                timer = 0f;


                if (reverso)
                {
                    //incrementa o quadro atual e limita � quantidade de quadros
                    if (loop)
                    {
                        quadroAtual -= 1;
                        if (quadroAtual < 0) quadroAtual = quadros.Length - 1;
                    }
                    else
                    {
                        if (quadroAtual > 0) quadroAtual -= 1;
                    }
                }
                else
                {
                    //incrementa o quadro atual e limita � quantidade de quadros
                    if (loop)
                    {
                        quadroAtual = (quadroAtual + 1) % quadros.Length;
                    }
                    else
                    {
                        if (quadroAtual < quadros.Length - 1) quadroAtual += 1;
                    }
                }
            }
        }
        #endregion


        #region Metodos
        /// <summary>
        /// Reseta o quadro atual e o timer
        /// </summary>
        public void Reset()
        {
            //reseta a anima��o a 0.
            quadroAtual = 0;
            timer = 0f;
        }

        /// <summary>
        /// Gera os ret�ngulos dos quadros da anima��o
        /// </summary>
        public void GerarRetangulos()
        {
            //Criar a lista de quaros
            quadros = new Rectangle[numQuadros];

            //determina a largura e altura de cada quadro
            int larguraQuadro = largura / numQuadros;
            int alturaQuadro = altura / numQuadros;

            if (vertical)
            {
                //cria os retangulos
                for (int i = 0; i < numQuadros; i++)
                {
                    //Cria cada ret�ngulo a partir dos offsets com a largura e a altura de cada quadro
                    quadros[i] = new Rectangle(OffsetX, OffsetY + (alturaQuadro * i), largura, alturaQuadro);
                }
            }
            else
            {
                //cria os retangulos
                for (int i = 0; i < numQuadros; i++)
                {
                    //Cria cada ret�ngulo a partir dos offsets com a largura e a altura de cada quadro
                    quadros[i] = new Rectangle(OffsetX + (larguraQuadro * i), OffsetY, larguraQuadro, altura);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Classe para dicionário de animações serializável
    /// </summary>
    /// <typeparam name="Chave">Chave única do item</typeparam>
    /// <typeparam name="Valor">Item</typeparam>
    [Serializable]
    public class Dicionario<Chave, Valor> : Dictionary<string, Animacao>, IXmlSerializable
    {
        #region Variaveis
        XmlSerializer _XmlSerializer;
        #endregion


        #region Construtor
        public Dicionario() { }
        #endregion


        #region Propriedades
        public XmlSerializer Serializer
        {
            get
            {
                if (_XmlSerializer == null) _XmlSerializer = new XmlSerializer(typeof(Animacao));

                return _XmlSerializer;
            }
        }
        #endregion


        #region Metodos
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            try
            {
                reader.Read();

                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Animacoes"))
                {
                    string key;
                    Animacao animacao;

                    reader.ReadToDescendant("Nome");

                    key = reader.ReadElementContentAsString();

                    animacao = (Animacao)Serializer.Deserialize(reader);

                    reader.ReadToNextSibling("AnimacaoDeSprites");

                    this.Add(key, animacao);
                }
                reader.Read();
            }
            catch (Exception)
            {
                // ignora erros
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            try
            {
                foreach (string key in this.Keys)
                {
                    writer.WriteStartElement("AnimacaoDeSprites");

                    writer.WriteElementString("Nome", key);
                    Serializer.Serialize(writer, this[key]);

                    writer.WriteEndElement();
                }
            }
            catch (Exception)
            {
                // ignora erros
            }
        }
        #endregion
    }

    /// <summary>
    /// Classe base para a anima��o de sprites (Retirada de www.ziggyware.com e modigicada por mim RCDMK)
    /// </summary>
    [Serializable]
    public class AnimacaoDeSprites
    {
        #region Variaveis
        /// <summary>
        /// Posicao atual do sprite
        /// </summary>
        public Vector2 Posicao = Vector2.Zero;

        /// <summary>
        /// Cole��o de anima��es do sprite
        /// </summary>
        public Dicionario<string, Animacao> Animacoes = new Dicionario<string, Animacao>();

        //O spritesheet
        Texture2D textura;

        //a anima��o atual
        string animacaoAtual;

        //a flag de atualiza��o da anima��o
        public bool atualizarAnimacao = true;
        #endregion


        #region Propriedades
        /// <summary>
        /// Retorna ou seta a spritesheet
        /// </summary>
        [XmlIgnore]
        public Texture2D Textura
        {
            get { return textura; }
            set { textura = value; }
        }

        /// <summary>
        /// Retorna ou seta a anima��o atual
        /// </summary>
        [ContentSerializer]
        public string AnimacaoAtual
        {
            get { return animacaoAtual; }
            set
            {
                //se o valor passado existir como anima��o
                if (value != null)
                {
                    if (Animacoes.ContainsKey(value))
                    {
                        //muda a anima��o somente se a anima�ao atual for diferente da informada
                        if (animacaoAtual == null || !animacaoAtual.Equals(value))
                        {
                            //seta a anima��o atual com o valor informado
                            animacaoAtual = value;

                            //reseta a anima��o
                            Animacoes[animacaoAtual].Reset();
                        }
                    }
                }
            }
        }
        #endregion


        #region contrutor
        public AnimacaoDeSprites() { }
        #endregion


        #region Metodos
        /// <summary>
        /// Inicia a anima��o se ela ainda n�o estiver iniciada
        /// </summary>
        public void iniciarAnimacao()
        {
            atualizarAnimacao = true;
        }

        /// <summary>
        /// Para a anima��o
        /// </summary>
        public void pararAnimacao()
        {
            atualizarAnimacao = false;
        }
        #endregion


        #region metodos Padao
        /// <summary>
        /// Atualiza o sprite
        /// </summary>
        /// <param name="gameTime">O tempo de jogo atual</param>
        public void Update(GameTime gameTime)
        {
            //Se tiver uma anima��o v�lida
            if (animacaoAtual == null)
            {
                //Se n�o tiver chaves, n�o tem anima��o, ent�o sai
                if (Animacoes.Keys.Count == 0) return;

                //pega uma lista de anima��es v�lidas na lista de anima��es
                string[] keys = new string[Animacoes.Keys.Count];

                //copia a primeira chave da lista para a lista de anima��es
                Animacoes.Keys.CopyTo(keys, 0);

                //seta a anima��o atual para a primeira anima��o
                animacaoAtual = keys[0];
            }

            //se a flag de atualiza��o for verdadeira, atualiza a anima��o
            if (atualizarAnimacao) Animacoes[animacaoAtual].Update(gameTime);
        }

        /// <summary>
        /// Desenha o sprite
        /// </summary>
        /// <param name="batch">O SpriteBatch para desenhar o sprite</param>
        public void Draw(SpriteBatch batch)
        {
            //usando o  Animacoes[animacaoAtual].QuadroAtual para pegar o ret�ngulo de origem do sprite na spritesheet
            if (animacaoAtual != null)
            {
                batch.Draw(textura, Posicao, Animacoes[animacaoAtual].RetanguloQuadroAtual, Color.White);
            }
        }
        #endregion
    }
}
