using System;
using System.Collections.Generic;
using System.Xml;
using CustomTextImporter;
using Jogo.Componentes;
using Jogo.Personagens;
using Jogo.Telas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jogo.Engine
{
    /// <summary>
    /// Classe para o mapa que � respons�vel por todas as rotinas referentes a ele, como detec��o de colis�o.
    /// </summary>
    public class Mapa : GameComponent
    {
        #region Variaveis
        protected Principal principal;

        protected Vector2 inicio = Vector2.Zero;
        protected Vector2 offset = Vector2.Zero;
        protected Tela tela;

        //XMLs
        protected XmlDocument XMLdoc;
        protected XmlDocument XMLitens;
        protected XmlDocument XMLinimigos;

        //Mapa
        protected String caminho;
        protected Texture2D tileSet;
        protected Tile[][] mapa;

        //Itens
        protected List<Item> itens;
        protected ListaItens<Item> itens2;
        protected List<Item> itens3;
        protected List<Portal> portais;
        protected ListaItens<Inimigo> inimigos;
        protected ListaItens<Corda> cordas;
        protected ListaItens<Item> coletaveis;
        protected Enchente enchente;

        //Tiles
        protected int esquerda = 0;
        protected int direita;
        protected int cima = 0;
        protected int baixo;
        protected int origemX = 0;
        protected int origemY = 0;

        //Camera
        protected Vector2 posicaoCamera = Vector2.Zero;
        protected static Rectangle camera;
        protected float larguraMargem;
        protected float alturaMargem;
        protected float margemEsquerda;
        protected float margemDireita;
        protected float margemCima;
        protected float margemBaixo;
        protected float movimentoCameraX;
        protected float movimentoCameraY;
        protected float posicaoMaximaCameraX;
        protected float posicaoMaximaCameraY;
        protected const float margem = 0.40f;
        #endregion


        #region Construtor
        public Mapa(Principal _principal, String _caminho, Tela _tela)
            : base(_principal)
        {
            this.principal = _principal;
            this.caminho = _caminho;
            this.tela = _tela;

            camera = new Rectangle(0, 0, principal.GraphicsDevice.Viewport.Width, principal.GraphicsDevice.Viewport.Height);

            itens = new List<Item>();
            itens2 = new ListaItens<Item>();
            itens3 = new List<Item>();
            portais = new List<Portal>();
            inimigos = new ListaItens<Inimigo>();
            cordas = new ListaItens<Corda>();
            coletaveis = new ListaItens<Item>();

            LerXML();
        }
        #endregion


        #region Propriedades
        public String Caminho
        {
            get { return caminho; }
            set { caminho = value; }
        }

        public XmlDocument XMLDOC
        {
            get { return XMLdoc; }
        }

        public XmlDocument XMLItens
        {
            get { return XMLitens; }
        }

        public XmlDocument XMLInimigos
        {
            get { return XMLinimigos; }
        }

        public Vector2 Inicio
        {
            get { return inicio; }
            set { inicio = value; }
        }

        public Vector2 Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public Tile[][] Tiles
        {
            get { return mapa; }
        }

        public Tela Tela
        {
            get { return tela; }
            set { tela = value; }
        }

        public Vector2 Medidas
        {
            get { return new Vector2(mapa[0].Length, mapa.Length); }
        }

        public Vector2 DimensoesTile
        {
            get { return Tile.Dimensoes; }
        }

        public Vector2 Centro
        {
            get { return Medidas/2; }
        }

        public Vector2 PosicaoCamera
        {
            get { return posicaoCamera; }
            set { posicaoCamera = value; }
        }

        public static Rectangle Camera
        {
            get { return camera; }
        }

        public List<Portal> Portais
        {
            get { return portais; }
        }

        public List<Item> Itens
        {
            get { return itens; }
        }

        public ListaItens<Item> Itens2
        {
            get { return itens2; }
        }

        public List<Item> Itens3
        {
            get { return itens3; }
        }

        public ListaItens<Corda> Cordas
        {
            get { return cordas; }
        }

        public ListaItens<Item> Coletaveis
        {
            get { return coletaveis; }
            set { coletaveis = value; }
        }

        public ListaItens<Inimigo> Inimigos
        {
            get { return inimigos; }
        }

        public Enchente Enchente
        {
            get { return enchente; }
            set { enchente = value; }
        }
        #endregion


        #region MetodosPadrao
        public override void Update(GameTime gameTime)
        {
            //Portais
            for (int i = 0; i < portais.Count; i++)
            {
                portais[i].Update(gameTime);
            }

            //Itens - Fundo
            for (int i = 0; i < itens.Count; i++)
            {
                itens[i].Update(gameTime);
            }

            //Colet�veis
            for (int i = 0; i < coletaveis.Count; i++)
            {
                coletaveis[i].Update(gameTime);
            }

            //Inimigos
            for (int i = 0; i < inimigos.Count; i++)
            {
                inimigos[i].Update(gameTime);
            }

            //Itens2 - Segundo plano
            for (int i = 0; i < itens2.Count; i++)
            {
                itens2[i].Update(gameTime);
            }

            //Itens3 - Primeiro plano
            for (int i = 0; i < itens3.Count; i++)
            {
                itens3[i].Update(gameTime);
            }

            //Agua
            if (caminho.EndsWith("Agua")) enchente.Update(gameTime);

            base.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Tiles
            esquerda = (int)Math.Floor(posicaoCamera.X / Tile.Dimensoes.X);
            direita = esquerda + spriteBatch.GraphicsDevice.Viewport.Width / (int)Tile.Dimensoes.X + 2;
            direita = Math.Min(direita, (int)Medidas.X);

            cima = (int)Math.Floor(posicaoCamera.Y / Tile.Dimensoes.Y);
            baixo = cima + spriteBatch.GraphicsDevice.Viewport.Height / (int)Tile.Dimensoes.Y + 2;
            baixo = Math.Min(baixo, (int)Medidas.Y);

            for (int y = cima; y < baixo; y++)
            {
                for (int x = esquerda; x < direita; x++)
                {
                    if (mapa[y][x].Origem != 0)
                    {
                        origemX = mapa[y][x].Origem % (int)(tileSet.Width / Tile.Dimensoes.X);
                        origemY = (int)(mapa[y][x].Origem / (int)(tileSet.Width / Tile.Dimensoes.X));
                        Rectangle origem = new Rectangle(origemX * (int)Tile.Dimensoes.X, origemY * (int)Tile.Dimensoes.Y, (int)Tile.Dimensoes.X, (int)Tile.Dimensoes.Y);
                        Rectangle destino = new Rectangle(x * (int)Tile.Dimensoes.X, y * (int)Tile.Dimensoes.Y, (int)Tile.Dimensoes.X, (int)Tile.Dimensoes.Y);
                        spriteBatch.Draw(tileSet, destino, origem, Color.White);
                    }
                }
            }

            //Portais
            for (int i = 0; i < portais.Count; i++)
            {
                portais[i].Draw(gameTime, spriteBatch);
            }

            //Itens - Fundo
            for (int i = 0; i < itens.Count; i++)
            {
                itens[i].Draw(gameTime, spriteBatch);
            }

            //Cordas
            for (int i = 0; i < cordas.Count; i++)
            {
                if (camera.Intersects(cordas[i].HitTest))
                {
                    cordas[i].Draw(gameTime, spriteBatch);
                }
            }

            //Itens2 - Segundo plano
            for (int i = 0; i < itens2.Count; i++)
            {
                itens2[i].Draw(gameTime, spriteBatch);
            }

            //Itens colet�veis
            for (int i = 0; i < coletaveis.Count; i++)
            {
                coletaveis[i].Draw(gameTime, spriteBatch);
            }

            //Inimigos
            for (int i = 0; i < inimigos.Count; i++)
            {
                inimigos[i].Draw(gameTime, spriteBatch);
            }

            //Personagem
            principal.telaJogo.Personagem.Draw(gameTime, spriteBatch);


            //Agua
            if (caminho.EndsWith("Agua"))
            {
                if (camera.Intersects(enchente.HitTest)) enchente.Draw(gameTime, spriteBatch);
            }

            //Itens3 - Primeiro plano
            for (int i = 0; i < itens3.Count; i++)
            {
                itens3[i].Draw(gameTime, spriteBatch);
            }
        }
        #endregion
        

        #region Metodos
        /// <summary>
        /// Calcula a posicao da camera para fazer o scroll
        /// </summary>
        /// <param name="viewport">O viewport do jogo</param>
        public void ScrollCamera(Viewport viewport)
        {
            // Calculando as bordas da viewport.
            larguraMargem = viewport.Width * margem;
            alturaMargem = viewport.Height * margem;
            margemEsquerda = posicaoCamera.X + larguraMargem;
            margemDireita = posicaoCamera.X + viewport.Width - larguraMargem;
            margemCima = posicaoCamera.Y + larguraMargem;
            margemBaixo = posicaoCamera.Y + viewport.Height - alturaMargem;
            // Calculando o quanto rolar quando o jogador esta proximo as bordas
            movimentoCameraX = 0f;
            movimentoCameraY = 0f;
            
            //Horizontal
            if (principal.telaJogo.Personagem.Posicao.X < margemEsquerda)
            {
                movimentoCameraX = principal.telaJogo.Personagem.Posicao.X - margemEsquerda;
            }
            else if (principal.telaJogo.Personagem.Bordas.X > margemDireita)
            {
                movimentoCameraX = principal.telaJogo.Personagem.Bordas.X - margemDireita;
            }

            //Vertical
            if (principal.telaJogo.Personagem.Bordas.Y < margemCima)
            {
                movimentoCameraY = principal.telaJogo.Personagem.Bordas.Y - margemCima;
            }
            else if (principal.telaJogo.Personagem.Bordas.Y > margemBaixo)
            {
                movimentoCameraY = principal.telaJogo.Personagem.Bordas.Y - margemBaixo;
            }

            // Atualiza a posicao da camera
            posicaoMaximaCameraX = (int)Tile.Dimensoes.X * (int)Medidas.X - viewport.Width;
            posicaoMaximaCameraY = (int)Tile.Dimensoes.Y * (int)Medidas.Y - viewport.Height;
            posicaoCamera = new Vector2(MathHelper.Clamp((int)(posicaoCamera.X + movimentoCameraX), 0.0f, posicaoMaximaCameraX), MathHelper.Clamp((int)(posicaoCamera.Y + movimentoCameraY), 0.0f, posicaoMaximaCameraY));
            
            if (camera.Width != viewport.Width) camera.Width = viewport.Width;
            if (camera.Height != viewport.Height) camera.Height = viewport.Height;
            
            camera.X = (int)posicaoCamera.X;
            camera.Y = (int)posicaoCamera.Y;
            camera.Width = viewport.Width;
            camera.Height = viewport.Height;
        }


        #region Montagem
        /// <summary>
        /// L� o XML carregado e monta o mapa de tiles
        /// </summary>
        public void LerXML()
        {
            XMLdoc = new XmlDocument();
            CustomText xml = principal.Content.Load<CustomText>(caminho);
            XMLdoc.LoadXml(xml.SourceCode);

            ((TelaJogo)tela).Fundo = principal.Content.Load<Texture2D>(XMLdoc.DocumentElement.Attributes["fundo"].Value);
            tileSet = principal.Content.Load<Texture2D>(XMLdoc.DocumentElement.Attributes["tileset"].Value);

            montarMapa();
        }

        /// <summary>
        /// Monta o mapa de tiles
        /// </summary>
        public void montarMapa()
        {
            Tile.Dimensoes = new Vector2(int.Parse(XMLdoc.DocumentElement.Attributes["larguraTile"].Value), int.Parse(XMLdoc.DocumentElement.Attributes["alturaTile"].Value));
            
            mapa = new Tile[XMLdoc.DocumentElement.ChildNodes.Count][];
            inicio.X = int.Parse(XMLdoc.DocumentElement.Attributes["personagemX"].Value);
            inicio.Y = int.Parse(XMLdoc.DocumentElement.Attributes["personagemY"].Value);
            for (int y = 0; y < XMLdoc.DocumentElement.ChildNodes.Count; y++)
            {
                Tile[] linhaAtual = new Tile[XMLdoc.DocumentElement.ChildNodes[y].ChildNodes.Count];
                for (int x = 0; x < XMLdoc.DocumentElement.ChildNodes[y].ChildNodes.Count; x++)
                {
                    linhaAtual[x] = pegaTile(XMLdoc.DocumentElement.ChildNodes[y].ChildNodes[x].Attributes, new Vector2(x, y), int.Parse(XMLdoc.DocumentElement.ChildNodes[y].ChildNodes[x].ChildNodes[0].Value));
                }
                mapa[y] = linhaAtual;
            }

            if (principal.telaJogo != null) principal.telaJogo.Iniciado = false;
        }
        
        /// <summary>
        /// Cria os itens e os posiciona na tela
        /// </summary>
        public void criarItens()
        {
            #region Limpeza
            itens.Clear();
            itens2.Clear();
            itens3.Clear();
            coletaveis.Clear();

            portais.Clear();
            cordas.Clear();
            #endregion


            #region Inicializa��o
            XMLitens = new XmlDocument();
            CustomText xml = principal.Content.Load<CustomText>(caminho + "_Itens");
            XMLitens.LoadXml(xml.SourceCode);
            int saidaX;
            int saidaY;
            int largura;
            int altura;
            int quadros;
            bool passavel;
            bool nuvem;
            String texturaItem;
            String texturaHUD;
            int X, Y;
            #endregion


            #region Cria��o e ajustes
            for (int i = 0; i < XMLitens.DocumentElement.ChildNodes.Count; i++)
            {
                Item novoItem;

                texturaItem = XMLitens.DocumentElement.ChildNodes[i].Attributes["sprite"].Value;
                String tipo = XMLitens.DocumentElement.ChildNodes[i].Attributes["tipo"].Value;
                X = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["x"].Value);
                Y = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["y"].Value);

                switch (tipo)
                {
                    case "roldana":
                    case "cracha":
                    case "engrenagem":
                    case "dica":
                    case "saveItem":
                    case "item":
                    #region Itens colet�veis
                        texturaHUD = XMLitens.DocumentElement.ChildNodes[i].Attributes["spriteHUD"].Value;
                        largura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["largura"].Value);
                        altura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["altura"].Value);
                        quadros = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["quadros"].Value);
                        passavel = XMLitens.DocumentElement.ChildNodes[i].Attributes["passavel"].Value == "s";
                        nuvem = XMLitens.DocumentElement.ChildNodes[i].Attributes["nuvem"].Value == "s";
                        novoItem = new Item(principal, new Vector2(X, Y), passavel, true, nuvem, texturaItem);
                        novoItem.Initialize();
                        novoItem.Tipo = tipo;
                        novoItem.NomeTexturaHUD = XMLitens.DocumentElement.ChildNodes[i].Attributes["spriteHUD"].Value;
                        novoItem.Animacao.Animacoes.Add("parado", new Animacao(largura / quadros, altura, 1, 1, false, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("indo", new Animacao(largura, altura, quadros, 4, false, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("voltando", new Animacao(largura, altura, quadros, 4, false, false, true, 0, 0));
                        novoItem.Animacao.AnimacaoAtual = XMLitens.DocumentElement.ChildNodes[i].Attributes["animacao"].Value;
                        novoItem.Animacao.iniciarAnimacao();
                        coletaveis.Add(novoItem);

                        if (nuvem)
                        {
                            for (int x = 0; x < (largura / quadros) / Tile.Dimensoes.X; x++)
                            {
                                mapa[Y][X + x].Nuvem = true;
                            }
                        }

                        break;
                    #endregion


                    case "roldanaSuporte":
                    #region Suporte das roldanas
                        largura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["largura"].Value);
                        altura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["altura"].Value);
                        quadros = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["quadros"].Value);
                        passavel = XMLitens.DocumentElement.ChildNodes[i].Attributes["passavel"].Value == "s";
                        nuvem = XMLitens.DocumentElement.ChildNodes[i].Attributes["nuvem"].Value == "s";
                        novoItem = new Item(principal, new Vector2(X, Y), passavel, false, nuvem, texturaItem);
                        novoItem.Tipo = tipo;
                        novoItem.NomeTexturaCompleta = XMLitens.DocumentElement.ChildNodes[i].Attributes["spriteCompleto"].Value;

                        novoItem.Initialize();

                        novoItem.CordaDe = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["cordaDe"].Value);
                        novoItem.Animacao.Animacoes.Add("parado", new Animacao(largura / quadros, altura, 1, 1, false, false, 0, 0));
                        novoItem.Animacao.AnimacaoAtual = "parado";
                        novoItem.Animacao.pararAnimacao();
                        itens2.Add(novoItem);

                        if (nuvem)
                        {
                            for (int x = 0; x < (largura / quadros) / Tile.Dimensoes.X; x++)
                            {
                                mapa[Y][X + x].Nuvem = true;
                            }
                        }

                        break;
                    #endregion


                    case "manivela":
                    #region Manivela
                        largura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["largura"].Value);
                        altura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["altura"].Value);
                        quadros = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["quadros"].Value);
                        passavel = XMLitens.DocumentElement.ChildNodes[i].Attributes["passavel"].Value == "s";
                        nuvem = XMLitens.DocumentElement.ChildNodes[i].Attributes["nuvem"].Value == "s";
                        novoItem = new Item(principal, new Vector2(X, Y), passavel, false, nuvem, texturaItem);
                        novoItem.Initialize();
                        novoItem.Tipo = tipo;
                        novoItem.CordaInterativa = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["corda"].Value);
                        novoItem.ItemInterativo = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["item"].Value);
                        novoItem.Animacao.Animacoes.Add("parado", new Animacao(largura / quadros, altura, 1, 1, false, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("indo", new Animacao(largura, altura, quadros, 5, true, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("voltando", new Animacao(largura, altura, quadros, 5, true, false, true, 0, 0));
                        novoItem.Animacao.AnimacaoAtual = XMLitens.DocumentElement.ChildNodes[i].Attributes["animacao"].Value;
                        novoItem.Animacao.iniciarAnimacao();
                        itens2.Add(novoItem);

                        break;
                    #endregion


                    case "empurravel":
                    #region Itens empurr�veis
                        largura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["largura"].Value);
                        altura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["altura"].Value);
                        quadros = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["quadros"].Value);
                        passavel = XMLitens.DocumentElement.ChildNodes[i].Attributes["passavel"].Value == "s";
                        nuvem = XMLitens.DocumentElement.ChildNodes[i].Attributes["nuvem"].Value == "s";
                        novoItem = new Item(principal, new Vector2(X, Y), passavel, false, nuvem, texturaItem);
                        novoItem.Initialize();
                        novoItem.Tipo = tipo;
                        novoItem.Peso = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["peso"].Value);

                        novoItem.Animacao.Animacoes.Add("parado", new Animacao(largura / quadros, altura, 1, 1, false, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("indo", new Animacao(largura, altura, quadros, 5, true, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("voltando", new Animacao(largura, altura, quadros, 5, true, false, true, 0, 0));
                        novoItem.Animacao.AnimacaoAtual = XMLitens.DocumentElement.ChildNodes[i].Attributes["animacao"].Value;
                        novoItem.Animacao.iniciarAnimacao();
                        itens2.Add(novoItem);

                        break;
                    #endregion


                    case "corda":
                    #region Cordas
                        saidaX = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["saidaX"].Value);
                        saidaY = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["saidaY"].Value);
                        Corda corda = new Corda(texturaItem, new Vector2(X * Tile.Dimensoes.X + Tile.Dimensoes.X/2, Y * Tile.Dimensoes.Y + Tile.Dimensoes.Y/2), new Vector2(saidaX * Tile.Dimensoes.X + Tile.Dimensoes.X/2, saidaY * Tile.Dimensoes.Y + Tile.Dimensoes.Y/2));
                        corda.LoadContent(principal.Content);
                        cordas.Add(corda);

                        break;
                    #endregion


                    case "elevador":
                    case "portal":
                    #region Portais
                        saidaX = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["saidaX"].Value);
                        saidaY = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["saidaY"].Value);
                        largura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["largura"].Value);
                        altura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["altura"].Value);
                        quadros = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["quadros"].Value);
                        
                        Portal novoPortal = new Portal(principal, new Vector2(X, Y), new Vector2(saidaX, saidaY), XMLitens.DocumentElement.ChildNodes[i].Attributes["tela"].Value, texturaItem);

                        novoPortal.Tipo = tipo;
                        
                        novoPortal.Animacao.Animacoes.Add("parado", new Animacao(largura / quadros, altura, 1, 1, false, false, 0, 0));
                        novoPortal.Animacao.Animacoes.Add("abrindo", new Animacao(largura, altura, quadros, 8, false, false, 0, 0));
                        novoPortal.Animacao.Animacoes.Add("fechando", new Animacao(largura, altura, quadros, 8, false, false, true, 0, 0));
                        novoPortal.Animacao.AnimacaoAtual = XMLitens.DocumentElement.ChildNodes[i].Attributes["animacao"].Value;
                        novoPortal.Animacao.iniciarAnimacao();
                        portais.Add(novoPortal);

                        break;
                    #endregion


                    case "vidro":
                    case "save":
                    case "cenario2":
                    #region Itens de segundo plano
                        largura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["largura"].Value);
                        altura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["altura"].Value);
                        quadros = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["quadros"].Value);
                        passavel = XMLitens.DocumentElement.ChildNodes[i].Attributes["passavel"].Value == "s";
                        nuvem = XMLitens.DocumentElement.ChildNodes[i].Attributes["nuvem"].Value == "s";
                        novoItem = new Item(principal, new Vector2(X, Y), passavel, false, nuvem, texturaItem);
                        novoItem.Initialize();
                        novoItem.Tipo = tipo;
                        novoItem.Animacao.Animacoes.Add("parado", new Animacao(largura / quadros, altura, 1, 1, false, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("indo", new Animacao(largura, altura, quadros, 5, true, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("voltando", new Animacao(largura, altura, quadros, 5, true, false, true, 0, 0));
                        novoItem.Animacao.AnimacaoAtual = XMLitens.DocumentElement.ChildNodes[i].Attributes["animacao"].Value;
                        if (tipo == "vidro")
                        {
                            novoItem.Animacao.pararAnimacao();
                        }
                        else
                        {
                            novoItem.Animacao.iniciarAnimacao();
                        }

                        itens2.Add(novoItem);

                        if (nuvem)
                        {
                            for (int x = 0; x < (largura / quadros) / Tile.Dimensoes.X; x++)
                            {
                                mapa[Y][X + x].Nuvem = true;
                            }
                        }

                        break;
                    #endregion


                    case "agua":
                    #region Agua
                        largura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["largura"].Value);
                        altura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["altura"].Value);

                        enchente = new Enchente(new Vector2(X * Tile.Dimensoes.X, Y * Tile.Dimensoes.Y), largura, altura);
                        enchente.LoadContent(principal.Content);
                        
                        break;
                    #endregion


                    case "cenario3":
                    #region Itens de primeiro plano
                        largura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["largura"].Value);
                        altura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["altura"].Value);
                        quadros = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["quadros"].Value);
                        passavel = XMLitens.DocumentElement.ChildNodes[i].Attributes["passavel"].Value == "s";
                        nuvem = XMLitens.DocumentElement.ChildNodes[i].Attributes["nuvem"].Value == "s";
                        novoItem = new Item(principal, new Vector2(X, Y), passavel, false, nuvem, texturaItem);
                        novoItem.Initialize();
                        novoItem.Tipo = tipo;
                        novoItem.Animacao.Animacoes.Add("parado", new Animacao(largura / quadros, altura, 1, 1, false, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("indo", new Animacao(largura, altura, quadros, 5, true, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("voltando", new Animacao(largura, altura, quadros, 5, true, false, true, 0, 0));
                        novoItem.Animacao.AnimacaoAtual = XMLitens.DocumentElement.ChildNodes[i].Attributes["animacao"].Value;
                        novoItem.Animacao.iniciarAnimacao();
                        itens3.Add(novoItem);

                        if (nuvem)
                        {
                            for (int x = 0; x < (largura / quadros) / Tile.Dimensoes.X; x++)
                            {
                                mapa[Y][X + x].Nuvem = true;
                            }
                        }

                        break;
                    #endregion


                    default:
                    //"cenario"
                    #region Itens de fundo
                        largura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["largura"].Value);
                        altura = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["altura"].Value);
                        quadros = int.Parse(XMLitens.DocumentElement.ChildNodes[i].Attributes["quadros"].Value);
                        passavel = XMLitens.DocumentElement.ChildNodes[i].Attributes["passavel"].Value == "s";
                        nuvem = XMLitens.DocumentElement.ChildNodes[i].Attributes["nuvem"].Value == "s";
                        novoItem = new Item(principal, new Vector2(X, Y), passavel, false, nuvem, texturaItem);
                        novoItem.Initialize();
                        novoItem.Tipo = tipo;
                        novoItem.Animacao.Animacoes.Add("parado", new Animacao(largura / quadros, altura, 1, 1, false, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("indo", new Animacao(largura, altura, quadros, 10, false, false, 0, 0));
                        novoItem.Animacao.Animacoes.Add("voltando", new Animacao(largura, altura, quadros, 10, false, false, true, 0, 0));
                        novoItem.Animacao.AnimacaoAtual = XMLitens.DocumentElement.ChildNodes[i].Attributes["animacao"].Value;
                        novoItem.Animacao.iniciarAnimacao();
                        itens.Add(novoItem);

                        if (nuvem)
                        {
                            for (int x = 0; x < (largura / quadros) / Tile.Dimensoes.X; x++)
                            {
                                mapa[Y][X + x].Nuvem = true;
                            }
                        }

                        break;
                    #endregion
                }
            }
            #endregion
        }

        /// <summary>
        /// Cria os inimigos e os posiciona na tela
        /// </summary>
        public void criarInimigos()
        {
            //Limpar inimigos atuais
            inimigos.Clear();

            //Criar Inimigos
            XMLinimigos = new XmlDocument();

            CustomText xml = principal.Content.Load<CustomText>(caminho + "_Inimigos");
            XMLinimigos.LoadXml(xml.SourceCode);

            for (int i = 0; i < XMLinimigos.DocumentElement.ChildNodes.Count; i++)
            {
                String texturaItem = XMLinimigos.DocumentElement.ChildNodes[i].Attributes["sprite"].Value;
                int X = int.Parse(XMLinimigos.DocumentElement.ChildNodes[i].Attributes["x"].Value);
                int Y = int.Parse(XMLinimigos.DocumentElement.ChildNodes[i].Attributes["y"].Value);
                float velocidade = float.Parse(XMLinimigos.DocumentElement.ChildNodes[i].Attributes["velocidade"].Value);
                Inimigo novoInimigo = new Inimigo(principal, new Vector2(X * Tile.Dimensoes.X, Y * Tile.Dimensoes.Y) - new Vector2(0, 1), velocidade, texturaItem);
                novoInimigo.EsperaMax = float.Parse(XMLinimigos.DocumentElement.ChildNodes[i].Attributes["espera"].Value);
                novoInimigo.Initialize();
                novoInimigo.LoadContent(principal.Content);
                inimigos.Add(novoInimigo);
            }
        }
        #endregion


        #region Coordenadas
        /// <summary>
        /// Pega a posi��o do mouse no mapa
        /// </summary>
        /// <returns>Um vetor2 com a posicao do mouse no mapa</returns>
        public static Vector2 pegaMouse()
        {
            Vector2 indice = new Vector2();
            indice.X = (int)Math.Floor(Mouse.GetState().X / Tile.Dimensoes.X);
            indice.Y = (int)Math.Floor(Mouse.GetState().Y / Tile.Dimensoes.Y);
            return indice;
        }

        /// <summary>
        /// Pega a posi��o no mapa com base na posi��o da tela
        /// </summary>
        /// <returns>Um vetor2 com a posicao do mouse no mapa</returns>
        public static Vector2 pegaIndice(Vector2 posicao)
        {
            Vector2 indice = new Vector2();
            indice.X = (int)Math.Floor(posicao.X / Tile.Dimensoes.X);
            indice.Y = (int)Math.Floor(posicao.Y / Tile.Dimensoes.Y);
            return indice;
        }

        /// <summary>
        /// Retorna a tile com base na informa��o do mapa
        /// </summary>
        /// <param name="qual">O valor retirado de uma posi��o no mapa</param>
        /// <param name="posicao">A posi��o da tile retornada</param>
        /// <returns>Uma tile com as caracter�sticas j� acertadas</returns>
        public virtual Tile pegaTile(XmlAttributeCollection qual, Vector2 posicao, int origem)
        {
            Tile tmpTile = new TileVazia(posicao, origem);
            tmpTile.carregaPropriedades(qual);
            return tmpTile;
        }

        /// <summary>
        /// Pega a posi��o na tela com base na posi��o no mapa
        /// </summary>
        /// <param name="posicao">A posi��o atual</param>
        /// <returns>Um vetor2 com a posi��o na tela</returns>
        public Vector2 converteIndice(Vector2 posicao)
        {
            Vector2 indice = new Vector2();
            indice.X = posicao.X * Tile.Dimensoes.X;
            indice.Y = posicao.Y * Tile.Dimensoes.Y;
            return indice;
        }
        #endregion


        #region Pathfinding
        /// <summary>
        /// Verifica as tiles visinhas se s�o pass�veis ou n�o (�til para pathfinding)
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>Um array de booleanos com cada dire��o na forma de [N, S, L, O, NE, NO, SE, SO]</returns>
        public bool[] checaTile(Vector2 posicao)
        {
            bool N, S, L, O, NE, NO, SE, SO;

            //Norte
            N = checaNorte(posicao);

            //Sul
            S = checaSul(posicao);

            //Leste
            L = checaLeste(posicao);

            //Oeste
            O = checaOeste(posicao);

            //Nordeste
            NE = checaNordeste(posicao);

            //Noroeste
            NO = checaNoroeste(posicao);

            //Sudeste
            SE = checaSudeste(posicao);

            //Sudoeste
            SO = checaSudoeste(posicao);

            //Monta o array para retornar
            bool[] tmpDirecoes = new bool[8];

            tmpDirecoes[0] = N;
            tmpDirecoes[1] = S;
            tmpDirecoes[2] = L;
            tmpDirecoes[3] = O;
            tmpDirecoes[4] = NE;
            tmpDirecoes[5] = NO;
            tmpDirecoes[6] = SE;
            tmpDirecoes[7] = SO;

            return tmpDirecoes;
        }

        /// <summary>
        /// Verifica se o norte da posi��o informada � pass�vel
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>true se for pass�vel e false se n�o</returns>
        public bool checaNorte(Vector2 posicao)
        {
            //Se estiver no topo do mapa
            if ((int)posicao.Y < 0)
            {
                //N�o pode mais subir
                return false;
            }
            else
            {
                //Caso contr�rio, se a tile de cima for pass�vel ou for nuvem
                if (mapa[(int)posicao.Y - 1][(int)posicao.X].Passavel)
                {
                    //Pode subir
                    return true;
                }
                else
                {
                    //Caso contr�rio, n�o pode
                    return false;
                }
            }
        }

        /// <summary>
        /// Verifica se o sul da posi��o informada � pass�vel
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>true se for pass�vel e false se n�o</returns>
        public bool checaSul(Vector2 posicao)
        {
            //Se estiver na base do mapa
            if ((int)posicao.Y >= mapa.Length - 1f)
            {
                //N�o pode mais descer
                return false;
            }
            else
            {
                //Caso contr�rio, se a tile de baixo for pass�vel e n�o for nuvem
                if (mapa[(int)posicao.Y + 1][(int)posicao.X].Passavel)
                {
                    //Pode descer
                    return true;
                }
                else
                {
                    //Caso contr�rio, n�o pode
                    return false;
                }
            }
        }

        /// <summary>
        /// Verifica se o oeste da posi��o informada � pass�vel
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>true se for pass�vel e false se n�o</returns>
        public bool checaOeste(Vector2 posicao)
        {
            //Se estiver na primeira coluna do mapa
            if ((int)posicao.X < 0)
            {
                //N�o pode mais passar
                return false;
            }
            else
            {
                //Caso contr�rio, se a tile da esquerda for pass�vel
                if (mapa[(int)posicao.Y][(int)posicao.X - 1].Passavel)
                {
                    //Pode passar
                    return true;
                }
                else
                {
                    //Caso contr�rio, n�o pode
                    return false;
                }
            }
        }

        /// <summary>
        /// Verifica se o leste da posi��o informada � pass�vel
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>true se for pass�vel e false se n�o</returns>
        public bool checaLeste(Vector2 posicao)
        {
            //Se estiver na �ltima coluna do mapa
            if ((int)posicao.X >= mapa[0].Length)
            {
                //N�o pode mais passar
                return false;
            }
            else
            {
                //Caso contr�rio, se a tile da direita for pass�vel
                if (mapa[(int)posicao.Y][(int)posicao.X + 1].Passavel)
                {
                    //Pode passar
                    return true;
                }
                else
                {
                    //Caso contr�rio, n�o pode
                    return false;
                }
            }
        }

        /// <summary>
        /// Verifica se o nordeste da posi��o informada � pass�vel
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>true se for pass�vel e false se n�o</returns>
        public bool checaNordeste(Vector2 posicao)
        {
            //Se a tile de cima e a da direita forem pass�veis
            if (checaNorte(posicao) && checaLeste(posicao))
            {
                //Se a tile de cima-direita for pass�vel ou for nuvem
                if (mapa[(int)posicao.Y - 1][(int)posicao.X + 1].Passavel || mapa[(int)posicao.Y - 1][(int)posicao.X + 1].Nuvem)
                {
                    //Pode passar
                    return true;
                }
                else
                {
                    //Caso contr�rio, n�o pode
                    return false;
                }
            }
            else
            {
                //Caso contr�rio, n�o pode
                return false;
            }
        }

        /// <summary>
        /// Verifica se o noroeste da posi��o informada � pass�vel
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>true se for pass�vel e false se n�o</returns>
        public bool checaNoroeste(Vector2 posicao)
        {
            //Se a tile de cima e a da esquerda forem pass�veis
            if (checaNorte(posicao) && checaOeste(posicao))
            {
                //Se a tile de cima-esquerda for pass�vel ou for nuvem
                if (mapa[(int)posicao.Y - 1][(int)posicao.X - 1].Passavel || mapa[(int)posicao.Y - 1][(int)posicao.X - 1].Nuvem)
                {
                    //Pode passar
                    return true;
                }
                else
                {
                    //Caso contr�rio, n�o pode
                    return false;
                }
            }
            else
            {
                //Caso contr�rio, n�o pode
                return false;
            }
        }

        /// <summary>
        /// Verifica se o sudeste da posi��o informada � pass�vel
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>true se for pass�vel e false se n�o</returns>
        public bool checaSudeste(Vector2 posicao)
        {
            //Se a tile de baixo e a da direita forem pass�veis
            if (checaSul(posicao) && checaLeste(posicao))
            {
                //Se a tile de baixo-direita for pass�vel e n�o for nuvem
                if (mapa[(int)posicao.Y + 1][(int)posicao.X + 1].Passavel && !mapa[(int)posicao.Y + 1][(int)posicao.X + 1].Nuvem)
                {
                    //Pode passar
                    return true;
                }
                else
                {
                    //Caso contr�rio, n�o pode
                    return false;
                }
            }
            else
            {
                //Caso contr�rio, n�o pode
                return false;
            }
        }

        /// <summary>
        /// Verifica se o sudoeste da posi��o informada � pass�vel
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>true se for pass�vel e false se n�o</returns>
        public bool checaSudoeste(Vector2 posicao)
        {
            //Se a tile de baixo e a da esquerda forem pass�veis
            if (checaSul(posicao) && checaOeste(posicao))
            {
                //Se a tile de baixo-esquerda for pass�vel e n�o for nuvem
                if (mapa[(int)posicao.Y + 1][(int)posicao.X - 1].Passavel && !mapa[(int)posicao.Y + 1][(int)posicao.X - 1].Nuvem)
                {
                    //Pode passar
                    return true;
                }
                else
                {
                    //Caso contr�rio, n�o pode
                    return false;
                }
            }
            else
            {
                //Caso contr�rio, n�o pode
                return false;
            }
        }
        #endregion


        #region Colisoes
        /// <summary>
        /// Verifica se a posi��o informada � nuvem
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>true se for nuvem e false se n�o</returns>
        public bool checaNuvem(Vector2 posicao)
        {
            //Se estiver na base do mapa
            if ((int)posicao.X < 0 || (int)posicao.X >= (int)Medidas.X || (int)posicao.Y < 0 || (int)posicao.Y >= (int)Medidas.Y)
            {
                //N�o � nuvem
                return false;
            }
            else
            {
                //Caso contr�rio, retorna se a tile � nuvem
                return mapa[(int)posicao.Y][(int)posicao.X].Nuvem;
            }
        }

        /// <summary>
        /// Verifica se a posi��o informada � escada
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>true se for escada e false se n�o</returns>
        public bool checaEscada(Vector2 posicao)
        {
            //Se estiver na base do mapa
            if ((int)posicao.X < 0 || (int)posicao.X >= (int)Medidas.X || (int)posicao.Y < 0 || (int)posicao.Y >= (int)Medidas.Y)
            {
                //N�o � escada
                return false;
            }
            else
            {
                //Caso contr�rio, retorna se a tile � escada
                return mapa[(int)posicao.Y][(int)posicao.X].Escada;
            }
        }

        /// <summary>
        /// Verifica se a posi��o informada � passavel
        /// </summary>
        /// <param name="posicao">A posi��o da tile atual</param>
        /// <returns>true se for passavel e false se n�o</returns>
        public bool checaPassavel(Vector2 posicao)
        {
            //Se estiver na base do mapa
            if ((int)posicao.X < 0 || (int)posicao.X >= (int)Medidas.X || (int)posicao.Y < 0 || (int)posicao.Y >= (int)Medidas.Y)
            {
                //N�o � passavel
                return false;
            }
            else
            {
                //Caso contr�rio, retorna se a tile � passavel
                return mapa[(int)posicao.Y][(int)posicao.X].Passavel;
            }
        }
        #endregion
        #endregion
    }
}