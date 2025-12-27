using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Jogo.Componentes;
using Jogo.Telas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jogo
{
    /// <summary>
    /// Classe principal do jogo
    /// </summary>
    public class Principal : Game
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
        public Texture2D texturaBranca;
        public TelaControles telaControles;
        public TelaJogo telaJogo;
        public TelaAbertura telaAbertura;
        public TelaCreditos telaCreditos;

        private BarraDeCarregamento barraDeCarregamento;

        //Save
        Dados dados = new Dados();
        public bool saveLoadPendente = false;
        public bool Salvando;
        public bool Carregando;
        public bool carregandoAssets;

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
#if PROFILE
            Components.Add(new ProfilerComponent(this));

            this.IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
#endif
            graphics.ApplyChanges();
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

        public bool CarregandoAssets
        {
            get { return carregandoAssets; }
        }
        #endregion


        #region MetodosPadrao
        /// <summary>
        /// Permite inicializar o jogo e todos os seus componentes não gráficos.
        /// </summary>
        protected override void Initialize()
        {
            //Inicializando a janela
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            ToggleFullscreen();

            Content.RootDirectory = "Content";

            base.Initialize();
        }

        /// <summary>
        /// Responsável pelo carregamento do conteúdo do jogo, chamada apenas uma vez.
        /// </summary>
        protected override void LoadContent()
        {
            // Create o SpriteBatch para desenhar as texturas
            spriteBatch = new SpriteBatch(GraphicsDevice);

            carregandoAssets = true;

            texturaBranca = new Texture2D(GraphicsDevice, 1, 1);
            texturaBranca.SetData(new[] { Color.White });

            //Barra de carregamento
            var larguraBarra = (int)(GraphicsDevice.Viewport.Width * 0.7f);
            var alturaBarra = (int)(GraphicsDevice.Viewport.Height * 0.05f);
            barraDeCarregamento = new BarraDeCarregamento(this, new Vector2((GraphicsDevice.Viewport.Width - larguraBarra) / 2, GraphicsDevice.Viewport.Height * 0.8f), larguraBarra, alturaBarra);
            Components.Add(barraDeCarregamento);

            //Tela do logo
            telaLogo = new TelaLogo(this);
            telaLogo.Initialize();
            Components.Add(telaLogo);

            //Definindo as fontes
            Fontes.Menu = Content.Load<SpriteFont>("Fonts\\bitstreamMenu");
            Fontes.MenuSelecionado = Fontes.Menu;
            Fontes.Score = Content.Load<SpriteFont>("Fonts\\Score_Miramonte");
            Fontes.Creditos = Content.Load<SpriteFont>("Fonts\\bitstreamCreditos");
            Fontes.TituloCreditos = Fontes.Score;

            Task.Run(() =>
            {
                //carregando os sons
                Sons.LoadContent(Content);
                barraDeCarregamento.Progresso += 0.05f;
            }).ContinueWith(t =>
            {
                //Tela Inicial
                fundoInicial = Content.Load<Texture2D>("Telas\\tela_inicio");
                telaInicial = new TelaInicio(this, fundoInicial);
                telaInicial.Initialize();
                Components.Add(telaInicial);
                barraDeCarregamento.Progresso += 0.05f;
            }).ContinueWith(t =>
            {
                //Tela Controles
                fundoControles = Content.Load<Texture2D>("Telas\\tela_inicio");
                controles = Content.Load<Texture2D>("Telas\\telas_controles");
                telaControles = new TelaControles(this, fundoControles, controles);
                telaControles.Initialize();
                Components.Add(telaControles);
                barraDeCarregamento.Progresso += 0.1f;
            }).ContinueWith(t =>
            {
                //Tela Jogo
                telaJogo = new TelaJogo(this);
                telaJogo.Initialize();
                Components.Add(telaJogo);
                barraDeCarregamento.Progresso += 0.2f;
            }).ContinueWith(t =>
            {
                //Tela Abertura
                telaAbertura = new TelaAbertura(this);
                telaAbertura.Initialize();
                Components.Add(telaAbertura);
                barraDeCarregamento.Progresso += 0.3f;
            }).ContinueWith(t =>
            {
                //Tela Creditos
                telaCreditos = new TelaCreditos(this);
                telaCreditos.Initialize();
                Components.Add(telaCreditos);
                barraDeCarregamento.Progresso += 0.25f;
            }).ContinueWith(t =>
            {
                ((TelaInicio)telaInicial).CriarMenu();

                barraDeCarregamento.Progresso += 0.05f;
                carregandoAssets = false;
            });

            //Iniciar a primeira tela
            telaLogo.Mostrar();
            telaAtual = telaLogo;

            base.LoadContent();
        }

        /// <summary>
        /// Descarregar o conteúdo, chamada apenas uma vez.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Permite atualizar o jogo, checar por colisões, tocar sons, etc
        /// </summary>
        /// <param name="gameTime">O tempo que o jogo está rodando</param>
        protected override void Update(GameTime gameTime)
        {
#if PROFILE
            using (new ProfileMarker(updateProfiler))
            {
#endif

            teclado = Keyboard.GetState();
            controle = GamePad.GetState(PlayerIndex.One);

            //Tela inteira
            if ((tecladoAnterior.IsKeyDown(Keys.F) && teclado.IsKeyUp(Keys.F)) && (tecladoAnterior.IsKeyDown(Keys.LeftControl) || tecladoAnterior.IsKeyDown(Keys.RightControl)))
            {
                graphics.ToggleFullScreen();
            }

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

            base.Update(gameTime);
#if PROFILE
            }
#endif
        }

        /// <summary>
        /// Permite "desenhar" na viewport o que se deseja
        /// </summary>
        /// <param name="gameTime">O tempo que o jogo está rodando</param>
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

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                SpriteBatch.DrawString(Fontes.Menu, infos, new Vector2(GraphicsDevice.Viewport.Width - 100, 10), Fontes.CorScore, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.End();
            }
            //Debug geral
            else if (Debug == 2)
            {
                String infos = String.Format("FPS: {0}\nObjetivos Completos: {1}\nCaindo: {2}\nSubindo: {3}\nPulando: {4}\nGC 1: {5}\nGC 2: {6}", fps, telaJogo.Personagem.ObjetivosCompletos, telaJogo.Personagem.Caindo, telaJogo.Personagem.Subindo, telaJogo.Personagem.Pulando, System.GC.CollectionCount(0), System.GC.CollectionCount(1));

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
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
            SalvarCarregar("Salvar");
        }

        public void solicitarCarregar()
        {
            saveLoadPendente = true;
            SalvarCarregar("Carregar");
        }

        private void SalvarCarregar(string acao)
        {
            bool pausado = false;
            if (!telaJogo.Pausado)
            {
                pausado = true;
                telaJogo.Pausado = true;
            }

            string nomeArquivo = "O_Sistema/save.sav";
            string arquivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyGameSaves", nomeArquivo);
            Directory.CreateDirectory(Path.GetDirectoryName(arquivo)); // Ensure the directory exists

            if (acao == "Salvar")
            {
                //Salvar o jogo
                Salvando = true;
                Salvar(arquivo);
            }
            else if (acao == "Carregar")
            {
                //Carregar o jogo
                Carregando = true;
                Carregar(arquivo);
            }

            if (pausado) telaJogo.Pausado = false;
        }

        private void Salvar(string arquivo)
        {
            if (saveLoadPendente)
            {
                XmlSerializer serializador = new XmlSerializer(typeof(Dados));
                FileStream save = File.Open(arquivo, FileMode.Create);
                XmlWriter writer = XmlWriter.Create(save);

                AtualizarDados();
                serializador.Serialize(writer, dados);

                save.Close();

                saveLoadPendente = false;
                Salvando = false;
            }
        }

        private void Carregar(string arquivo)
        {
            if (File.Exists(arquivo) && saveLoadPendente)
            {
                XmlSerializer serializador = new XmlSerializer(typeof(Dados));
                FileStream save = File.Open(arquivo, FileMode.Open);
                XmlReader reader = XmlReader.Create(save);

                dados = (Dados)serializador.Deserialize(reader);

                save.Close();

                CarregarDados();

                saveLoadPendente = false;
                Carregando = false;
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
                    novoItem.Animacao.Animacoes["indo"].QuadroAtual = Math.Min(3, dados.ObjetivosCompletos);
                }

                telaJogo.mapa.Itens2.Add(novoItem);
            }

            //Itens coletáveis
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

        bool _isFullscreen = false;
        bool _isBorderless = false;
        int _width = 0;
        int _height = 0;

        public void ToggleFullscreen()
        {
            bool oldIsFullscreen = _isFullscreen;

            if (_isBorderless)
            {
                _isBorderless = false;
            }
            else
            {
                _isFullscreen = !_isFullscreen;
            }

            ApplyFullscreenChange(oldIsFullscreen);
        }

        public void ToggleBorderless()
        {
            bool oldIsFullscreen = _isFullscreen;

            _isBorderless = !_isBorderless;
            _isFullscreen = _isBorderless;

            ApplyFullscreenChange(oldIsFullscreen);
        }

        private void ApplyFullscreenChange(bool oldIsFullscreen)
        {
            if (_isFullscreen)
            {
                if (oldIsFullscreen)
                {
                    ApplyHardwareMode();
                }
                else
                {
                    SetFullscreen();
                }
            }
            else
            {
                UnsetFullscreen();
            }
        }

        private void ApplyHardwareMode()
        {
            this.graphics.HardwareModeSwitch = !_isBorderless;
            this.graphics.ApplyChanges();
        }

        private void SetFullscreen()
        {
            _width = Window.ClientBounds.Width;
            _height = Window.ClientBounds.Height;

            this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.graphics.HardwareModeSwitch = !_isBorderless;

            this.graphics.IsFullScreen = true;
            this.graphics.ApplyChanges();
        }

        private void UnsetFullscreen()
        {
            this.graphics.PreferredBackBufferWidth = _width;
            this.graphics.PreferredBackBufferHeight = _height;
            this.graphics.IsFullScreen = false;
            this.graphics.ApplyChanges();
        }
        #endregion
    }
}
