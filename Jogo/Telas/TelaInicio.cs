using System;
using Jogo.Componentes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Jogo.Telas
{
    public class TelaInicio : Tela
    {
        #region Variaveis
        protected MenuImagem menu;
        protected MenuImagem menuOpcoes;
        protected Texture2D fundo;
        protected bool iniciado = false;
        protected Fader fader;
        #endregion


        #region Construtor
        public TelaInicio(Principal _principal, Texture2D _fundo) : base(_principal)
        {
            this.principal = _principal;
            this.fundo = _fundo;
        }
        #endregion


        #region Metodos Padrao
        protected override void LoadContent()
        {
            //Criando o fundo
            Componentes.Add(new ImagemCentral(principal, fundo, ImagemCentral.Modo.Esticado));

            fader = new Fader(principal, "Sprites\\Componentes\\FadeIn", "Sprites\\Componentes\\FadeOut");
            Componentes.Add(fader);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!iniciado)
            {
                fader.FadeOut();
                fader.FadeIn();
                iniciado = true;
            }

            if (!menu.Enabled)
            {
                menu.Posicao = new Vector2(menu.Posicao.X, menu.Posicao.Y + 4f);
                if (menu.Posicao.Y >= 400f) menu.Enabled = true;
            }
            else
            {
                teclado = Keyboard.GetState();
                controle = GamePad.GetState(PlayerIndex.One);

                controleMenus();

                tecladoAnterior = teclado;
                controleAnterior = controle;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (fader.Visivel)
            {
                principal.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                fader.Draw(gameTime, principal.SpriteBatch);
                principal.SpriteBatch.End();
            }
        }
        #endregion


        #region Metodos
        public void CriarMenu()
        {
            //Criando o menu
            menu = new MenuImagem(principal, this, new Texture2D(principal.GraphicsDevice, 2, 2));
            menuOpcoes = new MenuImagem(principal, this, principal.Content.Load<Texture2D>("Sprites\\Menus\\Fundos\\fundo_opcoes"));
            
            //String[] itens = { "Jogar", "Controles", "Opções", "Sair" };
            Texture2D[] itens = { principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\jogar"), principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\controles"), principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\opcoes"), principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\sair") };
            Menu[] menus = { null, null, menuOpcoes, null };
            Tela[] telas = { principal.telaAbertura, principal.telaControles, null, null };
            

            //String telaCheia = "desativada";
            //if (principal.Graphics.IsFullScreen) telaCheia = "ativada";
            //String[] itensOpcoes = { String.Format("Tela Inteira: {0}", telaCheia), "Sair"};
            Texture2D[] itensOpcoes = { principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\tela_inteira"), principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\tela_inteira"), principal.Content.Load<Texture2D>("Sprites\\Menus\\Botoes\\voltar") };
            Menu[] menusOpcoes = { null, null, menu };
            

            menu.criarMenu(itens, menus, telas);
            menuOpcoes.criarMenu(itensOpcoes, menusOpcoes);

            Componentes.Add(menu);
            Componentes.Add(menuOpcoes);
            atualizarTelaInteira();
        }

        public override void Mostrar()
        {
            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Sons.MusicaAbertura);
            }
            
            menu.Posicao = new Vector2((Game.Window.ClientBounds.Width - menu.Largura) / 2, -menu.Altura - 10);
            menuOpcoes.Posicao = new Vector2((Game.Window.ClientBounds.Width - menuOpcoes.Largura) / 2, 350);

            menu.Visible = true;
            menuOpcoes.Esconder();

            atualizarTelaInteira();

            base.Mostrar();
        }

        public override void Esconder()
        {
            if (menu != null) menu.Esconder();
            if (menuOpcoes != null) menuOpcoes.Esconder();

            base.Esconder();
        }

        private void controleMenus()
        {
            if ((tecladoAnterior.IsKeyDown(Keys.Enter) && teclado.IsKeyUp(Keys.Enter)) || (controleAnterior.Buttons.B == ButtonState.Pressed && controle.Buttons.B == ButtonState.Released))
            {
                if (menu.Visible)
                {
                    switch (menu.ItemSelecionado)
                    {
                        case 0:
                            if (MediaPlayer.State == MediaState.Playing) MediaPlayer.Stop();
                            principal.telaJogo.mapa.Caminho = "Mapas\\Inicio";
                            principal.telaJogo.mapa.LerXML();
                            principal.telaJogo.mapa.criarItens();
                            principal.telaJogo.mapa.criarInimigos();
                            principal.telaJogo.HUD.limpar();
                            menu.AbrirOpcao(menu.ItemSelecionado);
                            principal.solicitarSalvar();
                            break;

                        case 3:
                            if (!Principal.Mudo) Sons.MenuOK.Play();
                            Game.Exit();
                            break;
                        
                        default:
                            atualizarTelaInteira();
                            menu.AbrirOpcao(menu.ItemSelecionado);
                            break;
                    }
                }
                else if (menuOpcoes.Visible)
                {
                    switch (menuOpcoes.ItemSelecionado)
                    {
                        case 0:
                            if (!Principal.Mudo) Sons.MenuOK.Play();
                            principal.Graphics.ToggleFullScreen();

                            atualizarTelaInteira();

                            break;

                        case 1:
                            if (Principal.Mudo) Sons.MenuOK.Play();

                            Principal.Mudo = !Principal.Mudo;

                            atualizarTelaInteira();

                            break;

                        default:
                            menuOpcoes.AbrirOpcao(menuOpcoes.ItemSelecionado);

                            break;
                    }
                }
            }
        }

        private void atualizarTelaInteira()
        {
            //String telaCheia = "desativada";
            //if (principal.Graphics.IsFullScreen) telaCheia = "ativada";

            //menuOpcoes.ItemSelecionado = 0;
            //menuOpcoes.Item(String.Format("Tela Inteira: {0}", telaCheia));

            bool[] ativados = new bool[2];
            bool[] ativacao = new bool[2];
            ativacao[0] = true;
            ativacao[1] = true;
            ativados[0] = principal.Graphics.IsFullScreen;
            ativados[1] = !Principal.Mudo;

            menuOpcoes.Ativacao = ativacao;
            menuOpcoes.Ativados = ativados;
        }
        #endregion
    }
}
