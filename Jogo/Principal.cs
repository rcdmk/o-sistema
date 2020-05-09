using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Jogo.Componentes;
using Jogo.Personagens;
using Jogo.Telas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Jogo
{
    /// <summary>
    /// Classe principal do jogo
    /// </summary>
    public class Principal : Microsoft.Xna.Framework.Game
    {
        #region Inicializar Graficos
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        #endregion


        #region Variaveis
        //FPS
        private int fps = 0;
        private float tempoPassado = 0f;
        private int frameCount = 0;
#if PROFILE
        static Profiler updateProfiler = new Profiler("Update");
        static Profiler drawProfiler = new Profiler("Draw");
#endif

        //Input
        private KeyboardState teclado;
        private GamePadState controle;
        private KeyboardState tecladoAnterior;
        private GamePadState controleAnterior;

        //Telas
        public static Tela telaAtual;
        public static Tela telaAnterior;
        public TelaLogo telaLogo;
        private Texture2D fundoInicial;
        public Tela telaInicial;
        private Texture2D fundoControles;
        private Texture2D controles;
        public TelaControles telaControles;
        public TelaJogo telaJogo;
        public TelaAbertura telaAbertura;
        public TelaCreditos telaCreditos;

        //Save
        Dados dados = new Dados();
        public bool saveLoadPendente = false;
        public bool Salvando;
        public bool Carregando;

        public bool guideVisivelAntes = false;

        public int Debug = 0;

        public static bool Mudo = false;
        #endregion


        #region Structs
#if PROFILE
        struct ProfileMarker : IDisposable
        {
            public ProfileMarker(Profiler profiler)
            {
                this.profiler = profiler;
                profiler.Start();
            }

            public void Dispose()
            {
                profiler.Stop();
            }

            Profiler profiler;
        }
#endif
        #endregion


        #region Construtor
        public Principal()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();

            this.Components.Add(new SafeGamerServicesComponent(this));


#if PROFILE
            Components.Add(new ProfilerComponent(this));

            this.IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
#endif
        } 
        #endregion


        #region Propriedades
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public float FPS
        {
            get { return fps; }
        }

        private float TempoPassado
        {
            get { return tempoPassado; }
        }

        private float FrameCount
        {
            get { return frameCount; }
        }
        #endregion


        #region MetodosPadrao
        /// <summary>
        /// Permite inicializar o jogo e todos os seus componentes n�o gr�ficos.
        /// </summary>
        protected override void Initialize()
        {
            //Inicializando a janela
#if XBOX
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
#else
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
#endif

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            
            base.Initialize();
        }

        /// <summary>
        /// � respons�vel pelo carregamento do conte�do do jogo, chamada apenas uma vez.
        /// </summary>
        protected override void LoadContent()
        {
            // Create o SpriteBatch para desenhar as texturas
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //carregando os sons
            Sons.LoadContent(Content);

            //Definindo as fontes
            Fontes.Menu = Content.Load<SpriteFont>("Fonts\\bitstreamMenu");
            Fontes.MenuSelecionado = Fontes.Menu;
            Fontes.Score = Content.Load<SpriteFont>("Fonts\\Score_Miramonte");
            Fontes.Creditos = Content.Load<SpriteFont>("Fonts\\bitstreamCreditos");
            Fontes.TituloCreditos = Fontes.Score;

            //Tela do logo
            telaLogo = new TelaLogo(this);
            telaLogo.Initialize();
            Components.Add(telaLogo);

            //Tela Inicial
            fundoInicial = Content.Load<Texture2D>("Telas\\tela_inicio");
            telaInicial = new TelaInicio(this, fundoInicial);
            telaInicial.Initialize();
            Components.Add(telaInicial);


            //Tela Controles
            fundoControles = Content.Load<Texture2D>("Telas\\tela_inicio");
            controles = Content.Load<Texture2D>("Telas\\telas_controles");
            telaControles = new TelaControles(this, fundoControles, controles);
            telaControles.Initialize();
            Components.Add(telaControles);

            //Tela Jogo
            telaJogo = new TelaJogo(this);
            telaJogo.Initialize();
            Components.Add(telaJogo);

            //Tela Abertura
            telaAbertura = new TelaAbertura(this);
            telaAbertura.Initialize();
            Components.Add(telaAbertura);

            //Tela Creditos
            telaCreditos = new TelaCreditos(this);
            telaCreditos.Initialize();
            Components.Add(telaCreditos);

            ((TelaInicio)telaInicial).CriarMenu();

            //Iniciar a primeira tela
            telaLogo.Mostrar();
            telaAtual = telaLogo;

            base.LoadContent();
        }

        /// <summary>
        /// Descarregar o conte�do, chamada apenas uma vez.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Permite atualizar o jogo, checar por colis�es, tocar sons, etc
        /// </summary>
        /// <param name="gameTime">O tempo que o jogo est� rodando</param>
        protected override void Update(GameTime gameTime)
        {
#if PROFILE
            using (new ProfileMarker(updateProfiler))
            {
#endif
                //Se o Guide n�o estiver vis�vel, continua
                if (!PluggableGuide.IsVisible)
                {
                    teclado = Keyboard.GetState();
                    controle = GamePad.GetState(PlayerIndex.One);

                    //Despausa o jogo se acabou de ser ocultado
                    if (guideVisivelAntes) telaJogo.Pausado = false;


                    //Tela inteira
#if !XBOX
                    if ((tecladoAnterior.IsKeyDown(Keys.F) && teclado.IsKeyUp(Keys.F)) && (tecladoAnterior.IsKeyDown(Keys.LeftControl) || tecladoAnterior.IsKeyDown(Keys.RightControl)))
                    {
                        graphics.ToggleFullScreen();
                    }
#else
                    if (!graphics.isFullScreen) graphics.ToggleFullScreen();
#endif

                    if (tecladoAnterior.IsKeyDown(Keys.F12) && teclado.IsKeyUp(Keys.F12))
                    {
#if DEBUG || PROFILE
                        Debug = ++Debug % 3;
#else
                        Debug = ++Debug % 2;
#endif
                    }

                    tecladoAnterior = teclado;
                    controleAnterior = controle;

                    //Calcular o FPS
                    tempoPassado += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                //Caso contr�rio, pausa o jogo se acabou de ser mostrado
                else if (!guideVisivelAntes && PluggableGuide.IsVisible)
                {
                    telaJogo.Pausado = true;
                }

                guideVisivelAntes = PluggableGuide.IsVisible;
                base.Update(gameTime);
#if PROFILE
            }
#endif
        }

        /// <summary>
        /// Permite "desenhar" na viewport o que se deseja
        /// </summary>
        /// <param name="gameTime">O tempo que o jogo est� rodando</param>
        protected override void Draw(GameTime gameTime)
        {
#if PROFILE
            using (new ProfileMarker(drawProfiler))
            {
#endif
                //Conta os quadros rodados
                fps = (int)Math.Round(1 / tempoPassado);
                tempoPassado = 0;

                //Limpa a tela
                graphics.GraphicsDevice.Clear(Color.Black);

                //desenha o resto
                base.Draw(gameTime);
                
                //Debug FPS somente
                if (Debug == 1)
                {
                    String infos = String.Format("FPS: {0}", fps);

                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                    SpriteBatch.DrawString(Fontes.Menu, infos, new Vector2(GraphicsDevice.Viewport.Width - 100, 10), Fontes.CorScore, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    spriteBatch.End();
                }
                //Debug geral
                else if (Debug == 2)
                {
                    String infos = String.Format("FPS: {0}\nObjetivos Completos: {1}\nCaindo: {2}\nSubindo: {3}\nPulando: {4}\nGC 1: {5}\nGC 2: {6}", fps, telaJogo.Personagem.ObjetivosCompletos, telaJogo.Personagem.Caindo, telaJogo.Personagem.Subindo, telaJogo.Personagem.Pulando, System.GC.CollectionCount(0), System.GC.CollectionCount(1));

                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                    SpriteBatch.DrawString(Fontes.Menu, infos, new Vector2(GraphicsDevice.Viewport.Width - 290, 10), Fontes.CorScore, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    spriteBatch.End();
                }

#if PROFILE
            }
#endif
        } 
        #endregion


        #region Metodos
        public static void mostrarTela(Tela tela)
        {
            telaAnterior = telaAtual;
            telaAtual.Esconder();
            telaAtual = tela;
            tela.Mostrar();
        }

        public void solicitarSalvar()
        {
            saveLoadPendente = true;
            PluggableGuide.BeginShowStorageDeviceSelector(PlayerIndex.One, SalvarCarregar, "Salvar");
        }

        public void solicitarCarregar()
        {
            saveLoadPendente = true;
            PluggableGuide.BeginShowStorageDeviceSelector(PlayerIndex.One, SalvarCarregar, "Carregar");
        }

        private void SalvarCarregar(IAsyncResult resultado)
        {
            bool pausado = false;
            if (!telaJogo.Pausado)
            {
                pausado = true;
                telaJogo.Pausado = true;
            }

            StorageDevice device = PluggableGuide.EndShowStorageDeviceSelector(resultado);

            if (device != null)
            {
                StorageContainer container = device.OpenContainer("O_Sistema");
                string arquivo = Path.Combine(container.Path, "save.sav");

                if ((string)resultado.AsyncState == "Salvar")
                {
                    //Salvar o jogo
                    Salvando = true;
                    Salvar(container, arquivo);
                }
                else if ((string)resultado.AsyncState == "Carregar")
                {
                    //Carregar o jogo
                    Carregando = true;
                    Carregar(container, arquivo);
                }
            }

            if (pausado) telaJogo.Pausado = false;
        }

        private void Salvar(StorageContainer container, string arquivo)
        {
            if (saveLoadPendente)
            {
                XmlSerializer serializador = new XmlSerializer(typeof(Dados));
                FileStream save = File.Open(arquivo, FileMode.Create);
                XmlWriter writer = XmlWriter.Create(save);
                
                AtualizarDados();

                serializador.Serialize(writer, dados);

                save.Close();

                container.Dispose();

                saveLoadPendente = false;
                Salvando = false;
            }
        }

        private void Carregar(StorageContainer container, string arquivo)
        {
            if (File.Exists(arquivo) && saveLoadPendente)
            {
                XmlSerializer serializador = new XmlSerializer(typeof(Dados));
                FileStream save = File.Open(arquivo, FileMode.Open);
                XmlReader reader = XmlReader.Create(save);

                dados = (Dados)serializador.Deserialize(reader);

                save.Close();

                container.Dispose();

                CarregarDados();

                saveLoadPendente = false;
                Carregando = false;
            }
            else
            {
                container.Dispose();
            }
        }

        private void AtualizarDados()
        {
            dados.Caminho = telaJogo.mapa.Caminho;

            dados.ItensHUD = telaJogo.HUD.Itens;
            dados.ItensMapa = telaJogo.mapa.Itens2;
            dados.Coletaveis = telaJogo.mapa.Coletaveis;
            dados.Cordas = telaJogo.mapa.Cordas;
            dados.Inimigos = telaJogo.mapa.Inimigos;

            dados.TelaInteira = graphics.IsFullScreen;

            dados.Vidas = telaJogo.Personagem.Vidas;

            dados.ObjetivosCompletos = telaJogo.Personagem.ObjetivosCompletos;
            dados.ItensUsados = telaJogo.Personagem.ItensUsados;

            if (telaJogo.mapa.Caminho.EndsWith("Agua"))
            {
                dados.PosicaoAgua = telaJogo.mapa.Enchente.Posicao;
                dados.MedidasAgua = new Vector2(telaJogo.mapa.Enchente.Largura, telaJogo.mapa.Enchente.Altura);
                dados.NivelAgua = telaJogo.mapa.Enchente.UltimaFileira;
            }
            else
            {
                dados.PosicaoAgua = Vector2.Zero;
                dados.MedidasAgua = Vector2.Zero;
                dados.NivelAgua = "";
            }
        }

        private void CarregarDados()
        {
            telaJogo.mapa.Caminho = dados.Caminho;
            telaJogo.mapa.LerXML();
            telaJogo.mapa.criarItens();
            telaJogo.mapa.criarInimigos();

            //Itens de segundo plano
            telaJogo.mapa.Itens2.Clear();

            for (int i = 0; i < dados.ItensMapa.Count; i++)
            {
                Item novoItem = dados.ItensMapa[i];
                novoItem.LoadContent(Content);
                if (novoItem.Ativado && novoItem.Tipo == "roldanaSuporte")
                {
                    novoItem.Textura = Content.Load<Texture2D>(novoItem.NomeTexturaCompleta);
                }
                else if (novoItem.Tipo == "vidro")
                {
                    novoItem.Animacao.pararAnimacao();
                    novoItem.Animacao.Animacoes["indo"].QuadroAtual = Math.Min(3 , dados.ObjetivosCompletos);
                }

                telaJogo.mapa.Itens2.Add(novoItem);
            }

            //Itens colet�veis
            telaJogo.mapa.Coletaveis.Clear();

            for (int i = 0; i < dados.Coletaveis.Count; i++)
            {
                Item novoItem = dados.Coletaveis[i];
                novoItem.LoadContent(Content);

                telaJogo.mapa.Coletaveis.Add(novoItem);
            }

            //Cordas
            telaJogo.mapa.Cordas.Clear();

            for (int i = 0; i < dados.Cordas.Count; i++)
            {
                Corda novaCorda = dados.Cordas[i];
                novaCorda.LoadContent(Content);
                telaJogo.mapa.Cordas.Add(novaCorda);
            }

            if (dados.Caminho.EndsWith("Agua"))
            {
                telaJogo.mapa.Enchente.Posicao = dados.PosicaoAgua;
                telaJogo.mapa.Enchente.Largura = (int)dados.MedidasAgua.X;
                telaJogo.mapa.Enchente.Altura = (int)dados.MedidasAgua.Y;
                telaJogo.mapa.Enchente.AlterarNivel(dados.NivelAgua);
                telaJogo.mapa.Enchente.MontarAgua(Content);
            }

            //Inimigos
            //telaJogo.mapa.Inimigos.Clear();

            //for (int i = 0; i < dados.Inimigos.Count; i++)
            //{
            //    Inimigo novoInimigo = dados.Inimigos[i];
            //    novoInimigo.Principal = this;
            //    novoInimigo.LoadContent(Content);
            //    telaJogo.mapa.Inimigos.Add(novoInimigo);
            //}

            telaJogo.Personagem.inicializar();
            telaJogo.Personagem.Vidas = dados.Vidas;
            telaJogo.Personagem.ObjetivosCompletos = dados.ObjetivosCompletos;
            telaJogo.Personagem.ItensUsados = dados.ItensUsados;

            telaJogo.HUD.limpar();
            for (int i = 0; i < dados.ItensHUD.Count; i++)
            {
                telaJogo.HUD.addItem(dados.ItensHUD[i].Tipo, dados.ItensHUD[i].NomeTextura);
            }
        }
        #endregion
    }
}
