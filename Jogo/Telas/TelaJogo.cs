using System;
using System.Collections.Generic;
using Jogo.Componentes;
using Jogo.Engine;
using Jogo.Personagens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Jogo.Telas
{
    public class TelaJogo : Tela
    {
        #region Variaveis
        protected Texture2D fundo;
        public Mapa mapa;
        private Personagem personagem;

        //Interface
        protected Texture2D fundoHUD;
        protected HUD hud;
        protected Fader fader;
        protected ImagemCentral dica;

        //Menus
        protected MenuImagem menu;
        protected MenuImagem menuSair;
        protected MenuImagem menuReiniciar;
        protected Rectangle personagemBox;

        //Flags
        protected bool iniciado = false;
        protected bool pausado = false;
        protected bool derrota = false;
        protected bool vitoria = false;

        //Sons
        protected SoundEffectInstance goteira = Sons.Goteira.CreateInstance();
        protected SoundEffectInstance arrastando = Sons.Arrastar.CreateInstance();
        protected SoundEffectInstance arrastandoLeve = Sons.ArrastarLeve.CreateInstance();
        protected SoundEffectInstance manivela = Sons.Manivela.CreateInstance();
        #endregion


        #region Propriedades
        public Personagem Personagem
        {
            get { return personagem; }
        }

        public HUD HUD
        {
            get { return hud; }
        }

        public bool Pausado
        {
            get { return pausado; }
            set
            {
                pausado = value;
                if (pausado)
                {
                    if (MediaPlayer.State == MediaState.Playing) MediaPlayer.Pause();
                }
                else
                {
                    if (MediaPlayer.State == MediaState.Paused) MediaPlayer.Resume();
                }
            }
        }

        public bool Derrota
        {
            get { return derrota; }
            set { derrota = value; }
        }

        public bool Vitoria
        {
            get { return vitoria; }
            set { vitoria = value; }
        }

        public Fader Fader
        {
            get { return fader; }
        }

        public MenuImagem Menu
        {
            get { return menu; }
        }

        public Texture2D Fundo
        {
            get { return fundo; }
            set { fundo = value; }
        }

        public bool Iniciado
        {
            get { return iniciado; }
            set { iniciado = value; }
        }
        #endregion


        #region Construtor
        public TelaJogo(Principal _principal) : base(_principal)
        {
            this.principal = _principal;

            //Criando mapa Inicial
            mapa = new Mapa(_principal, "Mapas\\Inicio", this);
            Componentes.Add(mapa);

            //Adicionando o personagem
            personagem = new Personagem(_principal, new Vector2(mapa.Inicio.X * Tile.Dimensoes.X, mapa.Inicio.Y * Tile.Dimensoes.Y), 1.2f, 3, "Sprites\\Personagem\\personagem");
            Componentes.Add(personagem);

            //adicionando o Fader
            fader = new Fader(_principal, "Sprites\\Componentes\\FadeIn", "Sprites\\Componentes\\FadeOut");
            Componentes.Add(fader);
        }
        #endregion


        #region Metodos Padrao
        protected override void LoadContent()
        {
            //Carregando o persongem
            personagem.LoadContent(principal.Content);

            //HUD
            hud = new HUD(principal, mapa.PosicaoCamera, principal.Content.Load<Texture2D>("Sprites\\Interface\\HUD_FINAL"));
            Componentes.Add(hud);

            //Criando os menus
            Texture2D fundoPause = principal.Content.Load<Texture2D>("Sprites\\Menus\\Fundos\\fundo_pause");
            menu = new MenuImagem(principal, this, fundoPause);
            menu.Esconder();
            Componentes.Add(menu);

            menuSair = new MenuImagem(principal, this, fundoPause);
            menuSair.Esconder();
            Componentes.Add(menuSair);

            menuReiniciar = new MenuImagem(principal, this, principal.Content.Load<Texture2D>("Telas\\tela_continue"));
            menuReiniciar.Esconder();
            Componentes.Add(menuReiniciar);

            //Preenchendo os menus
            //String[] itens = { "Retornar", "Controles", "Sair" };
            Texture2D[] itens = { principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\continue"), principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\controles_pause"), principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\sair_pause") };
            Menu[] menus = { null, null, menuSair };
            Tela[] telas = { null, principal.telaControles, null };

            menu.criarMenu(itens, menus, telas);
            menu.Posicao = new Vector2((Game.Window.ClientBounds.Width - menu.Largura) / 2, (Game.Window.ClientBounds.Height - menu.Altura) / 2);

            //String[] itensSair = { "Ir para a tela inicial", "Cancelar" };
            Texture2D[] itensSair = { principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\menu_inicial"), principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\cancelar") };
            Menu[] menusSair = { null, menu };
            Tela[] telasSair = { principal.telaInicial, null };
            menuSair.criarMenu(itensSair, menusSair, telasSair);
            menuSair.Posicao = new Vector2((Game.Window.ClientBounds.Width - menuSair.Largura) / 2, (Game.Window.ClientBounds.Height - menuSair.Altura) / 2);

            //String[] itensReiniciar = { "Reiniciar tela", "Ir para a tela inicial" };
            Texture2D[] itensReiniciar = { principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\continue"), principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\menu_inicial") };
            Tela[] telasReiniciar = { null, principal.telaInicial };
            menuReiniciar.criarMenu(itensReiniciar, telasReiniciar);
            menuReiniciar.Posicao = new Vector2((Game.Window.ClientBounds.Width - menuReiniciar.Largura) / 2, (Game.Window.ClientBounds.Height - menuReiniciar.Altura) / 2);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            teclado = Keyboard.GetState();
            controle = GamePad.GetState(PlayerIndex.One);

            if (personagem.Vidas <= 0)
            {
                derrota = true;
                fader.FadeOut();
            }

            //Se não tiver mostrando uma dica
            if (dica == null)
            {
                //Controles dos menus
                controleMenus(gameTime);
            }

            //Controle das dicas (mostrar e ocultar)
            controleDicas(gameTime);


            if (!derrota && !pausado && !vitoria)
            {
                if (!personagem.Morto)
                {
                    //Posicionar a camera
                    mapa.ScrollCamera(principal.SpriteBatch.GraphicsDevice.Viewport);

                    //Atualizar itens e personagens
                    mapa.Update(gameTime);

                    //Controles do HUD
                    controleHUD(gameTime);

                    //Os inimigos s�o controlados pela IA no pr�prio objeto

                    //Controles do personagem
                    controlePersonagem(gameTime);

                    //Controles dos Portais
                    controlePortais(gameTime);

                    //Controle e intera��o com os Itens
                    controleItens(gameTime);


                    //Parar anima��es do persoangem se não estiver fazendo nada ou não puder se mexer
                    pararAnimacoes(gameTime);


                    if (!iniciado)
                    {
                        //Sons e m�sicas de fundo
                        //Se for a tela da �gua
                        if (mapa.Caminho.EndsWith("Agua"))
                        {
                            //Toca a goteira
                            if (!Principal.Mudo)
                            {
                                if (goteira.State != SoundState.Playing)
                                {
                                    if (!goteira.IsLooped) goteira.IsLooped = true;
                                    goteira.Volume = 0.5f;
                                    goteira.Play();
                                }
                            }

                            MediaPlayer.IsRepeating = true;
                            MediaPlayer.Volume = 0.5f;
                            MediaPlayer.Play(Sons.MusicaAgua);
                        }
                        else if (mapa.Caminho.EndsWith("Inicio"))
                        {
                            if (goteira.State != SoundState.Stopped) goteira.Stop();

                            MediaPlayer.IsRepeating = true;
                            MediaPlayer.Volume = 0.8f;
                            MediaPlayer.Play(Sons.MusicaInicio);
                        }
                        else if (mapa.Caminho.EndsWith("Roldanas"))
                        {
                            if (goteira.State != SoundState.Stopped) goteira.Stop();

                            MediaPlayer.IsRepeating = true;
                            MediaPlayer.Volume = 0.5f;
                            MediaPlayer.Play(Sons.MusicaRoldanas);
                        }
                        else
                        {
                            if (goteira.State != SoundState.Stopped) goteira.Stop();
                            if (MediaPlayer.State != MediaState.Stopped) MediaPlayer.Stop();
                        }

                        iniciado = true;
                    }
                }
                base.Update(gameTime);
            }
            else
            {
                menu.Update(gameTime);
                menuSair.Update(gameTime);
                menuReiniciar.Update(gameTime);
            }

            fader.Update(gameTime);
            tecladoAnterior = teclado;
            controleAnterior = controle;
        }

        public override void Draw(GameTime gameTime)
        {
            //Inicia o desenho posicionando a camera
            mapa.ScrollCamera(principal.SpriteBatch.GraphicsDevice.Viewport);
            Matrix transformacaoCamera = Matrix.CreateTranslation(-mapa.PosicaoCamera.X, -mapa.PosicaoCamera.Y, 0.0f);
            principal.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, transformacaoCamera);

            //Desenha o fundo
            var tamanhoFundo = new Rectangle((int)mapa.PosicaoCamera.X, (int)mapa.PosicaoCamera.Y, principal.Window.ClientBounds.Width, principal.Window.ClientBounds.Height);
            principal.SpriteBatch.Draw(fundo, tamanhoFundo, Color.White);

            //Desenha o mapa e os componentes da tela
            mapa.Draw(gameTime, principal.SpriteBatch);

            //Desenha o HUD
            hud.Posicao = mapa.PosicaoCamera;
            hud.Draw(gameTime, principal.SpriteBatch);


            //Fader
            fader.Posicao = mapa.PosicaoCamera;
            fader.Draw(gameTime, principal.SpriteBatch);

            //Mostrar mensagens de carregando ou salvando
            if ((personagem.Passando && fader.Animacao.Animacoes[fader.Animacao.AnimacaoAtual].QuadroAtual == 4) || principal.Carregando)
            {
                principal.SpriteBatch.DrawString(Fontes.Creditos, "Carrengando...", new Vector2(Mapa.Camera.X + Mapa.Camera.Width - Fontes.Menu.MeasureString("Carregando...").X - 50, Mapa.Camera.Y + Mapa.Camera.Height - Fontes.Menu.MeasureString("Cgd").Y - 30), Fontes.CorScore);
            }
            else if (principal.Salvando)
            {
                principal.SpriteBatch.DrawString(Fontes.Creditos, "Salvando...", new Vector2(Mapa.Camera.X + Mapa.Camera.Width - Fontes.Menu.MeasureString("Carregando...").X - 50, Mapa.Camera.Y + Mapa.Camera.Height - Fontes.Menu.MeasureString("Cgd").Y - 30), Fontes.CorScore);
            }

            //Encerra o desenho
            principal.SpriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion


        #region Metodos
        public override void Mostrar()
        {
            iniciado = false;

            personagem.inicializar();
            hud.limpar();
            menu.Esconder();
            menuSair.Esconder();
            menuReiniciar.Esconder();
            pausado = false;
            derrota = false;
            vitoria = false;
            fader.FadeOut();
            fader.FadeIn();

            base.Mostrar();
        }

        public override void Esconder()
        {
            if (MediaPlayer.State == MediaState.Playing) MediaPlayer.Stop();
            if (goteira.State != SoundState.Stopped) goteira.Stop();
            base.Esconder();
        }

        private void controleMenus(GameTime gameTime)
        {
            if (derrota && !menuReiniciar.Enabled)
            {
                menuReiniciar.Mostrar();
            }
            else
            {
                if ((tecladoAnterior.IsKeyDown(Keys.Escape) && teclado.IsKeyUp(Keys.Escape)) || (controleAnterior.Buttons.Back == ButtonState.Pressed && controle.Buttons.Back == ButtonState.Released))
                {
                    if (!Principal.Mudo) Sons.MenuOK.Play();

                    if (!derrota && !vitoria)
                    {
                        pausado = !pausado;
                        if (pausado)
                        {
                            //fader.FadeOut();
                            menu.Mostrar();
                        }
                        else
                        {
                            //fader.FadeIn();
                            menu.Esconder();
                        }
                    }
                    else if (!menuReiniciar.Enabled)
                    {
                        //fader.FadeOut();
                        //menuReiniciar.Mostrar();
                    }
                }
            }

            if ((pausado || derrota || vitoria) && !principal.telaControles.Visible)
            {
                if ((tecladoAnterior.IsKeyDown(Keys.Enter) && teclado.IsKeyUp(Keys.Enter)) || (controleAnterior.Buttons.Start == ButtonState.Pressed && controleAnterior.Buttons.Start == ButtonState.Released))
                {
                    if (menuReiniciar.Enabled)
                    {
                        switch (menuReiniciar.ItemSelecionado)
                        {
                            case 0:
                                if (!Principal.Mudo) Sons.MenuOK.Play();
                                principal.solicitarCarregar();
                                pausado = false;
                                derrota = false;
                                menuReiniciar.Esconder();
                                fader.FadeIn();
                                break;
                            default:
                                menuReiniciar.AbrirOpcao(menuReiniciar.ItemSelecionado);
                                break;
                        }
                    }
                    else if (menu.Enabled)
                    {
                        switch (menu.ItemSelecionado)
                        {
                            case 0:
                                if (!Principal.Mudo) Sons.MenuOK.Play();
                                pausado = false;
                                menu.Esconder();
                                fader.FadeIn();
                                if (derrota || vitoria) Principal.mostrarTela(principal.telaInicial);
                                break;
                            default:
                                menu.AbrirOpcao(menu.ItemSelecionado);
                                break;
                        }
                    }
                    else if (menuSair.Enabled)
                    {
                        menuSair.AbrirOpcao(menuSair.ItemSelecionado);
                    }
                }
            }
        }

        private void controlePersonagem(GameTime gameTime)
        {
            //Mover direita ou esquerda
            if (teclado.IsKeyDown(Keys.D) || teclado.IsKeyDown(Keys.Right) || controle.DPad.Right == ButtonState.Pressed)
            {
                if (!personagem.Subindo)
                {
                    personagem.Velocidade += new Vector2(personagem.VelocidadeIncremental, 0f);

                    if (!personagem.Pulando)
                    {
                        personagem.Animacao.AnimacaoAtual = "andando";
                        personagem.Animacao.iniciarAnimacao();
                    }
                    personagem.flip = SpriteEffects.None;
                }
            }
            else if (teclado.IsKeyDown(Keys.A) || teclado.IsKeyDown(Keys.Left) || controle.DPad.Left == ButtonState.Pressed)
            {
                if (!personagem.Subindo)
                {
                    personagem.Velocidade -= new Vector2(personagem.VelocidadeIncremental, 0f);
                    if (!personagem.Pulando)
                    {
                        personagem.Animacao.AnimacaoAtual = "andando";
                        personagem.Animacao.iniciarAnimacao();
                    }
                    personagem.flip = SpriteEffects.FlipHorizontally;
                }
            }
            //Subir
            if (teclado.IsKeyDown(Keys.Up) || teclado.IsKeyDown(Keys.W) || controle.DPad.Up == ButtonState.Pressed)
            {
                if ((mapa.checaEscada(Mapa.pegaIndice(new Vector2(personagem.Posicao.X + personagem.Medidas.X / 3, personagem.Bordas.Y))) && mapa.checaEscada(Mapa.pegaIndice(new Vector2(personagem.Posicao.X + personagem.Medidas.X / 3f, personagem.Bordas.Y - Tile.Dimensoes.Y)))) || mapa.checaEscada(Mapa.pegaIndice(new Vector2(personagem.Posicao.X + personagem.Medidas.X / 3f, personagem.Bordas.Y - Tile.Dimensoes.Y))))
                {
                    if (personagem.SomEscada.State == SoundState.Stopped)
                    {
                        if (!Principal.Mudo) personagem.SomEscada.Play();
                    }

                    personagem.Velocidade = Vector2.Zero;
                    personagem.Posicao = new Vector2((float)Math.Floor((personagem.Posicao.X + (personagem.Medidas.X / 3)) / Tile.Dimensoes.X) * Tile.Dimensoes.X, personagem.Posicao.Y - personagem.VelocidadeIncremental);
                    personagem.Subindo = true;
                    personagem.Caindo = false;
                    personagem.Pulando = false;
                    personagem.Animacao.AnimacaoAtual = "subindo";
                    personagem.Animacao.iniciarAnimacao();
                }
                else
                {
                    if (personagem.SomEscada.State != SoundState.Stopped) personagem.SomEscada.Stop();
                    if (personagem.Subindo)
                    {
                        personagem.Animacao.pararAnimacao();
                    }
                }
            }
            //Descer
            else if (!personagem.Pulando && (teclado.IsKeyDown(Keys.Down) || teclado.IsKeyDown(Keys.S) || controle.DPad.Down == ButtonState.Pressed))
            {
                if (mapa.checaEscada(Mapa.pegaIndice(new Vector2(personagem.Posicao.X + personagem.Medidas.X / 3, personagem.Bordas.Y + 1))))
                {
                    if (personagem.SomEscada.State == SoundState.Stopped)
                    {
                        if (!Principal.Mudo) personagem.SomEscada.Play();
                    }

                    personagem.Velocidade = Vector2.Zero;
                    personagem.Posicao = new Vector2((float)Math.Floor((personagem.Posicao.X + (personagem.Medidas.X / 3)) / Tile.Dimensoes.X) * Tile.Dimensoes.X, personagem.Posicao.Y + personagem.VelocidadeIncremental);
                    personagem.Subindo = true;
                    personagem.Caindo = false;
                    personagem.Pulando = false;
                    personagem.Animacao.AnimacaoAtual = "subindo";
                    personagem.Animacao.iniciarAnimacao();
                }
                else
                {
                    if (personagem.SomEscada.State != SoundState.Stopped) personagem.SomEscada.Stop();
                    personagem.Subindo = false;
                    personagem.checaChao();
                }
            }
            //Pular
            if (teclado.IsKeyDown(Keys.Space) && tecladoAnterior.IsKeyUp(Keys.Space) || (controleAnterior.Buttons.B == ButtonState.Pressed && controle.Buttons.B == ButtonState.Released))
            {
                personagem.pular();
            }
        }

        private void controleHUD(GameTime gameTime)
        {
            if (hud.Itens.Count > 0)
            {
                int itemSelecionadoAnterior = hud.ItemSelecionado;

                //Controle por numeros
                if (tecladoAnterior.IsKeyUp(Keys.D1) && teclado.IsKeyDown(Keys.D1) && hud.Itens.Count >= 1) hud.ItemSelecionado = 0;
                if (tecladoAnterior.IsKeyUp(Keys.D2) && teclado.IsKeyDown(Keys.D2) && hud.Itens.Count >= 2) hud.ItemSelecionado = 1;
                if (tecladoAnterior.IsKeyUp(Keys.D3) && teclado.IsKeyDown(Keys.D3) && hud.Itens.Count >= 3) hud.ItemSelecionado = 2;
                if (tecladoAnterior.IsKeyUp(Keys.D4) && teclado.IsKeyDown(Keys.D4) && hud.Itens.Count >= 4) hud.ItemSelecionado = 3;
                if (tecladoAnterior.IsKeyUp(Keys.D5) && teclado.IsKeyDown(Keys.D5) && hud.Itens.Count >= 5) hud.ItemSelecionado = 4;
                if (tecladoAnterior.IsKeyUp(Keys.D6) && teclado.IsKeyDown(Keys.D6) && hud.Itens.Count >= 6) hud.ItemSelecionado = 5;
                if (tecladoAnterior.IsKeyUp(Keys.D7) && teclado.IsKeyDown(Keys.D7) && hud.Itens.Count >= 7) hud.ItemSelecionado = 6;
                if (tecladoAnterior.IsKeyUp(Keys.D8) && teclado.IsKeyDown(Keys.D8) && hud.Itens.Count >= 8) hud.ItemSelecionado = 7;
                if (tecladoAnterior.IsKeyUp(Keys.D9) && teclado.IsKeyDown(Keys.D9) && hud.Itens.Count == 9) hud.ItemSelecionado = 8;

                //Controle por R e L  do controle ou < e > do teclado
                if ((controleAnterior.Buttons.LeftShoulder == ButtonState.Released && controle.Buttons.LeftShoulder == ButtonState.Pressed) || (tecladoAnterior.IsKeyUp(Keys.OemComma) && teclado.IsKeyDown(Keys.OemComma)))
                {
                    hud.ItemSelecionado--;
                }
                else if ((controleAnterior.Buttons.RightShoulder == ButtonState.Released && controle.Buttons.RightShoulder == ButtonState.Pressed) || (tecladoAnterior.IsKeyUp(Keys.OemPeriod) && teclado.IsKeyDown(Keys.OemPeriod)))
                {
                    hud.ItemSelecionado++;
                }

                //Manter a seleção válida se sair das bordas do HUD
                if (hud.ItemSelecionado < 0)
                {
                    hud.ItemSelecionado = hud.Itens.Count - 1;
                }
                else if (hud.ItemSelecionado >= hud.Itens.Count)
                {
                    hud.ItemSelecionado = 0;
                }

                if (itemSelecionadoAnterior != hud.ItemSelecionado)
                {
                    if (!Principal.Mudo) Sons.Menu.Play();
                }
            }
        }

        private void controlePortais(GameTime gameTime)
        {
            Rectangle personagemBox = personagem.HitTest;

            //Se estiver na agua nao usa a porta
            if (mapa.Caminho.EndsWith("Agua"))
            {
                if (mapa.Enchente.HitTest.Intersects(personagemBox)) return;
            }
            //Se estiver empurrando alguma coisas tambem nao
            else if (personagem.Animacao.AnimacaoAtual == "empurrando")
            {
                return;
            }



            for (int i = 0; i < mapa.Portais.Count; i++)
            {
                if (mapa.Portais[i].HitTest.Contains(personagemBox) && ((tecladoAnterior.IsKeyDown(Keys.LeftControl) && teclado.IsKeyUp(Keys.LeftControl)) || (tecladoAnterior.IsKeyDown(Keys.RightControl) && teclado.IsKeyUp(Keys.RightControl)) || (controleAnterior.Buttons.X == ButtonState.Pressed && controle.Buttons.X == ButtonState.Released)) && !personagem.Pulando && !personagem.Subindo)
                {
                    if (mapa.Portais[i].Tipo == "elevador")
                    {
                        if (!Principal.Mudo) Sons.Elevador.Play();
                    }
                    else
                    {
                        if (!Principal.Mudo) Sons.Porta.Play();
                    }

                    personagem.Passando = true;
                    mapa.Portais[i].Animacao.AnimacaoAtual = "abrindo";
                    mapa.Portais[i].Animacao.iniciarAnimacao();
                    mapa.Portais[i].Personagem = personagem;
                }
            }
        }

        private void controleItens(GameTime gameTime)
        {
            personagemBox = personagem.HitTest;

            //Itens de cenario nao precisam de tratamento


            #region Itens de segundo plano
            for (int i = 0; i < mapa.Itens2.Count; i++)
            {
                //Se for a maquina de refri e usar uma moeda
                checarSave(mapa.Itens2[i], personagemBox);

                //Se nao for passivel, nao atravessa
                tratarColisoes(mapa.Itens2[i], personagem, personagemBox);

                //Se tiver na tela das Roldanas
                if (mapa.Caminho.EndsWith("Roldanas"))
                {
                    //Se for uma roldana vazia e usar uma roldana
                    checarRoldana(mapa.Itens2[i], personagemBox);

                    //Controlar manivelas
                    checarManivela(mapa.Itens2[i], personagemBox);
                }
                //Se tiver na primeira tela ou na tela da �gua
                else if (mapa.Caminho.EndsWith("Inicio") || mapa.Caminho.EndsWith("Agua"))
                {
                    //Aplica gravidade e empuxo aos itens
                    aplicarFisicaItens(mapa.Itens2[i]);
                }

                if (i < 0 || i >= mapa.Itens2.Count) break;

                for (int j = 0; j < mapa.Inimigos.Count; j++)
                {
                    tratarColisoes(mapa.Itens2[i], mapa.Inimigos[j], mapa.Inimigos[j].HitTest);
                }
            }
            #endregion


            #region Itens coletaveis
            for (int i = 0; i < mapa.Coletaveis.Count; i++)
            {
                //Se for coletavel e pressionar CTRL
                if (coletarItem(mapa.Coletaveis, i, personagemBox))
                {
                    i--;
                    if (i < 0) break;
                }
            }
            #endregion


            #region Agua
            if (mapa.Caminho.EndsWith("Agua"))
            {
                checarAgua(personagem, personagemBox);

                for (int j = 0; j < mapa.Inimigos.Count; j++)
                {
                    checarAgua(mapa.Inimigos[j], mapa.Inimigos[j].HitTest);
                    if (mapa.Inimigos[j].Morto)
                    {
                        mapa.Inimigos.RemoveAt(j);
                        j--;
                        if (j < 0) break;
                    }
                }
            }
            #endregion

            //Itens de primeiro plano não precisam de tratamento pois são apenas itens de cenário
        }

        private void controleDicas(GameTime gameTime)
        {
            if (!pausado && !derrota && !personagem.Morto && hud.Itens.Count > 0)
            {
                //Se pressionar a tecla de usar item
                if ((tecladoAnterior.IsKeyUp(Keys.LeftShift) && teclado.IsKeyDown(Keys.LeftShift)) || (tecladoAnterior.IsKeyDown(Keys.RightShift) && teclado.IsKeyUp(Keys.RightShift)) || (controleAnterior.Buttons.B == ButtonState.Released && controle.Buttons.B == ButtonState.Pressed))
                {
                    //E se o item selecionado for uma dica
                    if (hud.Itens[hud.ItemSelecionado].Tipo == "engrenagem")
                    {
                        if (!Principal.Mudo) Sons.Item.Play();

                        //Se tiver na primeira fase, mostra a dica da agua
                        if (mapa.Caminho.EndsWith("Inicio"))
                        {
                            dica = new ImagemCentral(principal, principal.Content.Load<Texture2D>("Sprites\\Esquemas\\esquemas_agua"), ImagemCentral.Modo.Esticado);
                            Componentes.Add(dica);
                            pausado = true;
                        }
                        //Se tiver na fase da �gua, mostra a dica das roldanas
                        else if (mapa.Caminho.EndsWith("Agua"))
                        {
                            dica = new ImagemCentral(principal, principal.Content.Load<Texture2D>("Sprites\\Esquemas\\esquemas_roldanas"), ImagemCentral.Modo.Esticado);
                            Componentes.Add(dica);
                            pausado = true;
                        }
                    }
                }
            }
            else if (pausado && !derrota && !personagem.Morto)
            {
                if (dica != null && ((tecladoAnterior.IsKeyDown(Keys.Enter) && teclado.IsKeyUp(Keys.Enter)) || (tecladoAnterior.IsKeyDown(Keys.Escape) && teclado.IsKeyUp(Keys.Escape)) || (controleAnterior.Buttons.Back == ButtonState.Pressed && controle.Buttons.Back == ButtonState.Released) || (controleAnterior.Buttons.A == ButtonState.Pressed && controle.Buttons.A == ButtonState.Released) || (controleAnterior.Buttons.B == ButtonState.Pressed && controle.Buttons.B == ButtonState.Released) || (controleAnterior.Buttons.X == ButtonState.Pressed && controle.Buttons.X == ButtonState.Released) || (controleAnterior.Buttons.Y == ButtonState.Pressed && controle.Buttons.Y == ButtonState.Released) || (controleAnterior.Buttons.Start == ButtonState.Pressed && controle.Buttons.Start == ButtonState.Released)))
                {
                    if (!Principal.Mudo) Sons.MenuOK.Play();
                    Componentes.Remove(dica);
                    dica = null;
                    pausado = false;
                }
            }
        }

        private bool coletarItem(List<Item> itens, int i, Rectangle personagemBox)
        {
            //Se estiver empurrando, não coleta o item
            if (personagem.Animacao.AnimacaoAtual == "empurrando") return false;

            if (itens[i].Coletavel)
            {
                if (itens[i].HitTest.Intersects(personagemBox) && ((tecladoAnterior.IsKeyUp(Keys.RightControl) && teclado.IsKeyDown(Keys.RightControl)) || (tecladoAnterior.IsKeyUp(Keys.LeftControl) && teclado.IsKeyDown(Keys.LeftControl)) || (controleAnterior.Buttons.A == ButtonState.Released && controle.Buttons.A == ButtonState.Pressed)) && !personagem.Pulando && !personagem.Subindo)
                {
                    //coleta o item
                    //Se não conseguir por estar cheio, retorna falso, caso contrário, coleta e remove o item da tela
                    if (!hud.addItem(itens[i].Tipo, itens[i].NomeTexturaHUD)) return false;

                    if (!Principal.Mudo) Sons.PegandoItem.Play(0.5f, 0, 0);
                    itens[i].Dispose();
                    itens.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        private void checarManivela(Item item, Rectangle personagemBox)
        {
            if (item.ItemInterativo >= 0 && item.Tipo == "manivela")
            {
                if (item.HitTest.Intersects(personagemBox) && ((tecladoAnterior.IsKeyUp(Keys.LeftControl) && teclado.IsKeyDown(Keys.LeftControl)) || (tecladoAnterior.IsKeyUp(Keys.RightControl) && teclado.IsKeyDown(Keys.RightControl)) || (controleAnterior.Buttons.X == ButtonState.Released && controle.Buttons.X == ButtonState.Pressed) || (personagem.Animacao.AnimacaoAtual == "manivela" && ((teclado.IsKeyDown(Keys.RightControl) || teclado.IsKeyDown(Keys.LeftControl) || controle.Buttons.X == ButtonState.Pressed)))) && !personagem.Pulando && !personagem.Subindo)
                {
                    Vector2 posicaoManivela = new Vector2();
                    posicaoManivela.X = item.PosicaoReal.X - Tile.Dimensoes.X / 2.5f + 1;
                    posicaoManivela.Y = personagem.Posicao.Y;

                    personagem.flip = SpriteEffects.None;
                    personagem.Posicao = posicaoManivela;
                    personagem.Velocidade = Vector2.Zero;

                    personagem.Animacao.AnimacaoAtual = "manivela";
                    personagem.Animacao.pararAnimacao();

                    //Se tiver um item interativo e uma corda interativa definidos, interage
                    if ((item.ItemInterativo < mapa.Itens2.Count) && (item.CordaInterativa >= 0 && item.CordaInterativa < mapa.Cordas.Count))
                    {
                        //Verifica se tem os objetivos completos suficientes para rodar
                        if ((mapa.Itens2[item.ItemInterativo].NomeTextura.EndsWith("CORDA") && personagem.ObjetivosCompletos >= 2) || (mapa.Itens2[item.ItemInterativo].NomeTextura.EndsWith("SOBE") && personagem.ObjetivosCompletos >= 3))
                        {
                            //Se a corda não estiver pequena demais, sobe o item e diminui a corda
                            if (mapa.Cordas[item.CordaInterativa].Tamanho > Tile.Dimensoes.Y / 2)
                            {
                                if (!Principal.Mudo)
                                {
                                    if (manivela.State == SoundState.Stopped) manivela.Play();
                                }

                                personagem.Animacao.iniciarAnimacao();

                                mapa.Cordas[item.CordaInterativa].Tamanho -= 1;

                                Vector2 posicaoItem = new Vector2();
                                posicaoItem.X = mapa.Itens2[item.ItemInterativo].PosicaoReal.X;
                                posicaoItem.Y = mapa.Itens2[item.ItemInterativo].PosicaoReal.Y - 1;
                                mapa.Itens2[item.ItemInterativo].PosicaoReal = posicaoItem;
                                return;
                            }
                        }
                    }

                    if (manivela.State == SoundState.Playing) manivela.Stop();
                }
            }
        }

        private void checarSave(Item item, Rectangle personagemBox)
        {
            if (mapa.Caminho.EndsWith("Agua"))
            {
                if (mapa.Enchente.HitTest.Intersects(personagemBox)) return;
            }

            if (hud.Itens.Count > 0 && item.Tipo == "save")
            {
                if (item.HitTest.Intersects(personagemBox) && ((tecladoAnterior.IsKeyDown(Keys.RightShift) && teclado.IsKeyUp(Keys.RightShift)) || (tecladoAnterior.IsKeyDown(Keys.LeftShift) && teclado.IsKeyUp(Keys.LeftShift)) || (controleAnterior.Buttons.B == ButtonState.Pressed && controle.Buttons.B == ButtonState.Released)) && !personagem.Pulando && !personagem.Subindo)
                {
                    //se tiver a moeda
                    if (hud.Itens[hud.ItemSelecionado].Tipo == "saveItem")
                    {
                        if (!Principal.Mudo) Sons.Moeda.Play();

                        //Usa a moeda
                        hud.removerItem(hud.ItemSelecionado);

                        //Checkpoint
                        Item checkpoint = new Item(principal, item.Posicao, true, false, false, "Sprites\\Geral\\CHECKPOINT");
                        checkpoint.Animacao.Animacoes.Clear();
                        checkpoint.Initialize();
                        checkpoint.Animacao.Animacoes.Add("indo", new Animacao(960, 64, 5, 8, false, false, 0, 0));
                        checkpoint.AnimacaoAtual = "indo";
                        checkpoint.Animacao.iniciarAnimacao();
                        mapa.Itens2.Add(checkpoint);

                        //Salva o jogo
                        principal.solicitarSalvar();
                        if (!Principal.Mudo) Sons.Moeda.Play();
                    }
                }
            }
        }

        private void checarRoldana(Item item, Rectangle personagemBox)
        {
            if (hud.Itens.Count > 0 && item.Tipo == "roldanaSuporte" && !item.Ativado)
            {
                if (item.HitTest.Intersects(personagemBox) && ((tecladoAnterior.IsKeyDown(Keys.RightShift) && teclado.IsKeyUp(Keys.RightShift)) || (tecladoAnterior.IsKeyDown(Keys.LeftShift) && teclado.IsKeyUp(Keys.LeftShift)) || (controleAnterior.Buttons.B == ButtonState.Pressed && controle.Buttons.B == ButtonState.Released)) && !personagem.Pulando && !personagem.Subindo)
                {
                    //se tiver a roldana
                    if (hud.Itens[hud.ItemSelecionado].Tipo == "roldana")
                    {
                        //Coloca no lugar
                        hud.removerItem(hud.ItemSelecionado);
                        item.Textura = principal.Content.Load<Texture2D>(item.NomeTexturaCompleta);
                        item.Ativado = true;

                        if (!Principal.Mudo) Sons.Item.Play();

                        if (item.CordaDe >= 0 && item.CordaDe < mapa.Itens2.Count)
                        {
                            Corda novaCorda = new Corda("Sprites\\Componentes\\corda", mapa.Itens2[item.CordaDe].PosicaoReal + Tile.Dimensoes / 2, item.PosicaoReal + Tile.Dimensoes / 2);
                            novaCorda.LoadContent(principal.Content);

                            mapa.Cordas.Add(novaCorda);
                        }

                        personagem.ObjetivosCompletos++;
                    }
                }
            }
        }

        private void tratarColisoes(Item item, Personagem personagem, Rectangle personagemBox)
        {
            if ((!item.Passavel || (item.Tipo == "empurravel" && (tecladoAnterior.IsKeyUp(Keys.LeftControl) && teclado.IsKeyDown(Keys.LeftControl)) || (tecladoAnterior.IsKeyUp(Keys.RightControl) && teclado.IsKeyDown(Keys.RightControl)) || (controleAnterior.Buttons.X == ButtonState.Released && controle.Buttons.X == ButtonState.Pressed) || (personagem.Animacao.AnimacaoAtual == "empurrando" && (teclado.IsKeyDown(Keys.LeftControl) || teclado.IsKeyDown(Keys.RightControl) || controle.Buttons.X == ButtonState.Pressed)))) && item.HitTest.Intersects(personagemBox))
            {
                Vector2 retorno = CalcularMTD(personagemBox, item.HitTest);
                if (retorno != Vector2.Zero)
                {
                    //Cima
                    if (retorno.Y < 0 && personagem.Velocidade.Y > 0 && !item.Passavel)
                    {
                        personagem.Posicao = new Vector2(personagem.Posicao.X, personagem.Posicao.Y + retorno.Y + .5f);
                        personagem.Velocidade = new Vector2(personagem.Velocidade.X, 0);
                        personagem.Caindo = false;
                        personagem.Pulando = false;
                    }
                    //Baixo
                    else if (retorno.Y > 0 && personagem.Velocidade.Y < 0 && !item.Passavel)
                    {
                        personagem.Posicao = new Vector2(personagem.Posicao.X, personagem.Posicao.Y + retorno.Y);
                        personagem.Velocidade = new Vector2(personagem.Velocidade.X, 0);
                    }
                    //Esquerda
                    else if (retorno.X < 0 && (personagem.Velocidade.X > 0 || (item.Tipo == "empurravel" && (personagem.Animacao.AnimacaoAtual == "empurrando" || teclado.IsKeyDown(Keys.LeftControl) || teclado.IsKeyDown(Keys.RightControl) || controle.Buttons.X == ButtonState.Pressed))))
                    {
                        personagem.Velocidade = new Vector2(0, personagem.Velocidade.Y);

                        //Se for um dos seguranças
                        if (!personagem.Equals(this.personagem) && !item.Passavel)
                        {
                            personagem.Posicao = new Vector2(personagem.Posicao.X + retorno.X - Tile.Dimensoes.X / 2, personagem.Posicao.Y);

                            if (((Inimigo)personagem).Espera <= 0) ((Inimigo)personagem).Espera = ((Inimigo)personagem).EsperaMax;
                        }
                        else
                        {
                            personagem.Posicao = new Vector2(personagem.Posicao.X + retorno.X + 1, personagem.Posicao.Y);

                            //Se for empurravel, empurra
                            if (item.Tipo == "empurravel" && (personagem.Animacao.AnimacaoAtual == "empurrando" || teclado.IsKeyDown(Keys.LeftControl) || teclado.IsKeyDown(Keys.RightControl) || controle.Buttons.X == ButtonState.Pressed) && !personagem.Pulando)
                            {
                                //Se o item for leve
                                if (item.Peso <= 10)
                                {
                                    if (!Principal.Mudo)
                                    {
                                        if (arrastandoLeve.State == SoundState.Stopped) arrastandoLeve.Play();
                                    }
                                    item.Velocidade = new Vector2(personagem.VelocidadeIncremental, item.Velocidade.Y);
                                }
                                else
                                {
                                    if (!Principal.Mudo)
                                    {
                                        if (arrastando.State == SoundState.Stopped) arrastando.Play();
                                    }
                                    item.Velocidade = new Vector2(personagem.VelocidadeIncremental / 2, item.Velocidade.Y);
                                }

                                personagem.flip = SpriteEffects.None;
                                personagem.Posicao = new Vector2(item.HitTest.Left - personagem.Medidas.X + 2, personagem.Posicao.Y);
                            }
                        }

                        if (!personagem.Pulando)
                        {
                            if (item.Tipo == "empurravel")
                            {
                                personagem.Animacao.AnimacaoAtual = "empurrando";
                            }
                            else
                            {
                                personagem.Animacao.AnimacaoAtual = "parado";
                            }

                            personagem.Animacao.iniciarAnimacao();
                        }
                    }
                    //Direita
                    else if (retorno.X > 0 && (personagem.Velocidade.X < 0 || (item.Tipo == "empurravel" && (personagem.Animacao.AnimacaoAtual == "empurrando" || teclado.IsKeyDown(Keys.LeftControl) || teclado.IsKeyDown(Keys.RightControl) || controle.Buttons.X == ButtonState.Pressed))))
                    {
                        personagem.Velocidade = new Vector2(0, personagem.Velocidade.Y);

                        //Se for um dos seguranças
                        if (!personagem.Equals(this.personagem) && !item.Passavel)
                        {
                            personagem.Posicao = new Vector2(personagem.Posicao.X + retorno.X + Tile.Dimensoes.X / 2, personagem.Posicao.Y);

                            if (((Inimigo)personagem).Espera <= 0) ((Inimigo)personagem).Espera = ((Inimigo)personagem).EsperaMax;
                        }
                        else
                        {
                            personagem.Posicao = new Vector2(personagem.Posicao.X + retorno.X - 1, personagem.Posicao.Y);

                            //Se for empurravel, empurra
                            if (item.Tipo == "empurravel" && (personagem.Animacao.AnimacaoAtual == "empurrando" || teclado.IsKeyDown(Keys.LeftControl) || teclado.IsKeyDown(Keys.RightControl) || controle.Buttons.X == ButtonState.Pressed) && !personagem.Pulando)
                            {
                                //Se o item for leve
                                if (item.Peso <= 10)
                                {
                                    if (!Principal.Mudo)
                                    {
                                        if (arrastandoLeve.State == SoundState.Stopped) arrastandoLeve.Play();
                                    }
                                    item.Velocidade = new Vector2(-personagem.VelocidadeIncremental, item.Velocidade.Y);
                                }
                                else
                                {
                                    if (!Principal.Mudo)
                                    {
                                        if (arrastando.State == SoundState.Stopped) arrastando.Play();
                                    }
                                    item.Velocidade = new Vector2(-personagem.VelocidadeIncremental / 2, item.Velocidade.Y);
                                }

                                personagem.flip = SpriteEffects.FlipHorizontally;
                                personagem.Posicao = new Vector2(item.HitTest.Right - 2, personagem.Posicao.Y);

                            }
                        }

                        if (!personagem.Pulando)
                        {
                            if (item.Tipo == "empurravel")
                            {
                                personagem.Animacao.AnimacaoAtual = "empurrando";
                            }
                            else
                            {
                                personagem.Animacao.AnimacaoAtual = "parado";
                            }

                            personagem.Animacao.iniciarAnimacao();
                        }
                    }
                    else if (personagem.Animacao.AnimacaoAtual != "empurrando")
                    {
                        if (arrastando.State != SoundState.Stopped) arrastando.Stop();
                        if (arrastandoLeve.State != SoundState.Stopped) arrastandoLeve.Stop();
                    }

                    if (retorno.Y < 0) personagem.Pulando = false;
                }
                else
                {
                    item.Velocidade = new Vector2(0, item.Velocidade.Y);
                }
            }
            else if (item.Tipo == "empurravel")
            {
                item.Velocidade = new Vector2(0, item.Velocidade.Y);
            }
        }

        private void aplicarFisicaItens(Item item)
        {
            if (item.Tipo == "empurravel")
            {
                if (mapa.checaPassavel(Mapa.pegaIndice(new Vector2(item.PosicaoReal.X + ((item.Velocidade.X <= 0) ? item.HitTest.Width : 0), item.HitTest.Bottom + 3))) && !mapa.checaNuvem(Mapa.pegaIndice(new Vector2(item.PosicaoReal.X + ((item.Velocidade.X <= 0) ? item.HitTest.Width : 0), item.HitTest.Bottom + 3))))
                {
                    item.Velocidade += new Vector2(0, Fisica.Gravidade);
                }
                else
                {
                    //Impacto
                    if (item.Velocidade.Y > 5)
                    {
                        if (item.Velocidade.Y > 10)
                        {
                            if (!Principal.Mudo) Sons.Queda.Play(1, -1, 0);
                        }
                        else
                        {
                            if (!Principal.Mudo) Sons.Queda.Play(0.5f, 0, 0);
                        }
                    }

                    item.PosicaoReal = new Vector2(item.PosicaoReal.X, Mapa.pegaIndice(new Vector2(item.PosicaoReal.X, item.PosicaoReal.Y + 2)).Y * Tile.Dimensoes.Y);
                    item.Velocidade = new Vector2(item.Velocidade.X, 0);
                }

                //se tiver na tela da água, aplica física e checagens da água
                if (mapa.Caminho.EndsWith("Agua")) checarAguaItem(item);

                item.Velocidade = new Vector2(item.Velocidade.X * Fisica.Friccao, item.Velocidade.Y);

                if (Math.Abs(item.Velocidade.X) < 0.05f) item.Velocidade = new Vector2(0, item.Velocidade.Y);
                if (Math.Abs(item.Velocidade.Y) < 0.05f) item.Velocidade = new Vector2(item.Velocidade.X, 0);

                item.PosicaoReal += item.Velocidade;
            }
        }

        private void checarAgua(Personagem personagem, Rectangle personagemBox)
        {
            if (mapa.Enchente.HitTest.Intersects(personagemBox))
            {
                if (!personagem.Agua)
                {
                    if (!Principal.Mudo) Sons.AguaImpacto.Play();
                    personagem.Agua = true;
                }

                personagem.Velocidade = new Vector2(personagem.Velocidade.X, personagem.Velocidade.Y + ((mapa.Enchente.Posicao.Y - personagem.Bordas.Y + personagem.Medidas.Y / 3) / (personagem.Peso * 20)));
                personagem.Velocidade *= Agua.Densidade;
                personagem.Caindo = true;
                personagem.Subindo = false;

                if (personagem.Velocidade.Y == 0) personagem.Pulando = false;

                if (personagem.Posicao.Y >= mapa.Enchente.Posicao.Y)
                {
                    personagem.morrer();
                    return;
                }

                if (personagem.Bordas.Y + personagem.Velocidade.Y > mapa.Enchente.HitTest.Bottom && personagem.Velocidade.Y >= personagem.Peso - (personagem.Peso * Agua.Densidade))
                {
                    personagem.Posicao = new Vector2(personagem.Posicao.X, mapa.Enchente.HitTest.Bottom - personagem.Medidas.Y);
                    personagem.morrer();
                }
            }
            else
            {
                personagem.Agua = false;
            }
        }

        private void checarAguaItem(Item item)
        {
            if (!item.Objetivo)
            {
                if (mapa.Enchente.HitTest.Intersects(item.HitTest))
                {
                    if (!item.Ativado)
                    {
                        if (item.Peso <= 10)
                        {
                            if (!Principal.Mudo) Sons.AguaImpactoLeve.Play();
                        }
                        else
                        {
                            if (!Principal.Mudo) Sons.AguaImpacto.Play();
                        }

                        item.Ativado = true;
                        personagem.ItensUsados++;
                    }

                    //Se o item for leve, aplica empuxo
                    if (item.Peso <= 10) item.Velocidade = new Vector2(item.Velocidade.X, item.Velocidade.Y + ((mapa.Enchente.Posicao.Y - item.HitTest.Bottom + item.HitTest.Height / 2) / (item.Peso * 10)));

                    //Aplica desaceleração pela densidade da água
                    item.Velocidade *= Agua.Densidade;

                    //Se o item for pesado e estiver parando
                    if (item.Peso > 10 && item.Velocidade.Y <= 0.01f)
                    {
                        //Para o item de vez e marca como objetivo
                        item.Velocidade = new Vector2(item.Velocidade.X, 0);
                        item.PosicaoReal = new Vector2(item.PosicaoReal.X, mapa.Enchente.HitTest.Bottom - item.HitTest.Height - 2);

                        item.Objetivo = true;
                        personagem.ObjetivosCompletos++;

                        //Atualiza o quadro atual da porta de vidro
                        for (int i = 0; i < mapa.Itens2.Count; i++)
                        {
                            if (mapa.Itens2[i].Tipo == "vidro")
                            {
                                if (personagem.ObjetivosCompletos >= 0 && personagem.ObjetivosCompletos <= 3)
                                {
                                    mapa.Itens2[i].Animacao.AnimacaoAtual = "indo";
                                    mapa.Itens2[i].Animacao.pararAnimacao();
                                    mapa.Itens2[i].Animacao.Animacoes["indo"].QuadroAtual = personagem.ObjetivosCompletos;
                                }

                                if (personagem.ObjetivosCompletos == 3) mapa.Itens2[i].Passavel = true;
                            }
                        }

                        //Se jogar 3 ou mais itens na água, quebra o vidro
                        if (personagem.ObjetivosCompletos == 3)
                        {
                            if (!Principal.Mudo) Sons.Vidro.Play();
                            if (!Principal.Mudo) Sons.Agua.Play();

                            Vector2 posicaoAgua = new Vector2();

                            posicaoAgua.Y = 29 * Tile.Dimensoes.Y;
                            mapa.Enchente.Posicao = posicaoAgua;
                            mapa.Enchente.Largura = (int)mapa.Medidas.X;
                            mapa.Enchente.MontarAgua(principal.Content);

                            for (int i = 0; i < mapa.Itens2.Count; i++)
                            {
                                if (mapa.Itens2[i].Ativado)
                                {
                                    mapa.Itens2.RemoveAt(i);
                                    i--;
                                    if (i < 0) break;
                                }
                            }

                        }//Se jogar 1, sobe um pouco a água
                        else if (personagem.ObjetivosCompletos == 1)
                        {
                            mapa.Enchente.UltimaFileira = "metade";
                        }
                        //se jogar 2, enche a água
                        else if (personagem.ObjetivosCompletos == 2)
                        {
                            mapa.Enchente.UltimaFileira = "cheio";
                        }
                    }

                    if (personagem.ItensUsados >= 3)
                    {
                        for (int i = 0; i < mapa.Inimigos.Count; i++)
                        {
                            mapa.Inimigos[i].Perseguidor = true;
                        }
                    }
                }
                else
                {
                    item.Ativado = false;
                }
            }
        }

        private void pararAnimacoes(GameTime gameTime)
        {
            //Parar animações se estiver na escada ou mudar para a animação de parado se não estiver na escada e estiver pressionando nenhuma tecla
            if ((teclado.GetPressedKeys().Length == 0 && controle.PacketNumber == 0 && personagem.Animacao.AnimacaoAtual != "manivela" && personagem.Animacao.AnimacaoAtual != "empurrando") || (personagem.Velocidade == Vector2.Zero && !personagem.Subindo && !personagem.Pulando && personagem.Animacao.AnimacaoAtual != "manivela" && personagem.Animacao.AnimacaoAtual != "empurrando") || (personagem.Subindo && teclado.IsKeyUp(Keys.W) && teclado.IsKeyUp(Keys.S) && teclado.IsKeyUp(Keys.Up) && teclado.IsKeyUp(Keys.Down) && controle.DPad.Up == ButtonState.Released && controle.DPad.Down == ButtonState.Released) || (!personagem.Morto && (personagem.Animacao.AnimacaoAtual == "manivela" || personagem.Animacao.AnimacaoAtual == "empurrando") && teclado.IsKeyUp(Keys.RightControl) && teclado.IsKeyUp(Keys.LeftControl) && controle.Buttons.X == ButtonState.Released))
            {
                if (personagem.Subindo && !personagem.Pulando)
                {
                    personagem.Posicao = new Vector2((float)Math.Floor((personagem.Posicao.X + (personagem.Medidas.X / 2f)) / Tile.Dimensoes.X) * Tile.Dimensoes.X - (personagem.Medidas.X - Tile.Dimensoes.X) / 2, personagem.Posicao.Y);
                    personagem.Animacao.AnimacaoAtual = "subindo";
                    personagem.Animacao.pararAnimacao();
                }
                else if (!personagem.Pulando)
                {
                    personagem.Animacao.AnimacaoAtual = "parado";
                    personagem.Animacao.iniciarAnimacao();
                }
            }
        }

        /// <summary>
        /// Função que calcula a mínima distância absoluta(MTD) resultante de uma intersecção entre 2 retângulos. Usada para resolver colisões. (Retirada de www.ziggyware.com e modigicada por mim RCDMK)
        /// </summary>
        /// <param name="obj1">O retângulo do objeto 1</param>
        /// <param name="obj2">O retângulo do objeto 2</param>
        /// <returns>O vetor resultante da intersecção com a menor distância usada para separar os objetos</returns>
        public static Vector2 CalcularMTD(Rectangle obj1, Rectangle obj2)
        {
            // O resultado da intersecção que é usado pra corrigir a posição do objeto
            Vector2 resultado = Vector2.Zero;

            // Isto é usado pra calcular a diferença de distâncias entre os lados
            float diferenca = 0.0f;

            // Isto guarda a mínima distância absoluta(MTD) usada pra separar os objetos que colidem
            float minimumTranslationDistance = 0.0f;

            // Eixo guarda os valores de X e Y.  X = 0, Y = 1.
            // Lado guarda o valor de esquerda (-1) ou direita (+1).
            // São usados para calcular o vetor resultante (resultado).
            int eixo = 0, lado = 0;

            // Esquerda
            diferenca = (obj1.X + obj1.Width) - obj2.X;
            if (diferenca < 0.0f)
            {
                return Vector2.Zero;
            }
            minimumTranslationDistance = diferenca;
            eixo = 0;
            lado = -1;

            // Direita
            diferenca = (obj2.X + obj2.Width) - obj1.X;
            if (diferenca < 0.0f)
            {
                return Vector2.Zero;
            }
            if (diferenca < minimumTranslationDistance)
            {
                minimumTranslationDistance = diferenca;
                eixo = 0;
                lado = 1;
            }

            // Baixo
            diferenca = (obj1.Y + obj1.Height) - obj2.Y;
            if (diferenca < 0.0f)
            {
                return Vector2.Zero;
            }
            if (diferenca < minimumTranslationDistance)
            {
                minimumTranslationDistance = diferenca;
                eixo = 1;
                lado = -1;
            }

            // Cima
            diferenca = (obj2.Y + obj2.Height) - obj1.Y;
            if (diferenca < 0.0f)
            {
                return Vector2.Zero;
            }
            if (diferenca < minimumTranslationDistance)
            {
                minimumTranslationDistance = diferenca;
                eixo = 1;
                lado = 1;
            }

            // Intersecção ocorrida:
            if (eixo == 1) // Eixo Y
                resultado.Y = (float)lado * minimumTranslationDistance;
            else // Eixo X
                resultado.X = (float)lado * minimumTranslationDistance;

            return resultado;
        }
        #endregion
    }
}
