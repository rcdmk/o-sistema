#region Using
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Jogo.Componentes
{
    /// <summary>
    /// A classe base para o menu do jogo
    /// </summary>
    public class Menu : DrawableGameComponent
    {
        #region variaveis
        protected SpriteBatch spriteBatch;
        protected Vector2 posicao = Vector2.Zero;
        protected int itemSelecionado = 0;
        protected List<String> itens;
        protected List<Tela> telas;
        protected List<Menu> menus;
        protected int largura;
        protected int altura;
        protected KeyboardState tecladoAnterior;
        protected GamePadState controleAnterior;
        protected Tela tela;
        #endregion


        #region Propriedades
        public int Largura
        {
            get { return largura; }
        }

        public int Altura
        {
            get { return altura; }
        }

        public int ItemSelecionado
        {
            get { return itemSelecionado; }
            set { itemSelecionado = value; }
        }

        public Vector2 Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public Tela Tela
        {
            get { return tela; }
            set { tela = value; }
        }
        #endregion


        #region construtor
        public Menu(Principal _principal, Tela _tela) : base(_principal)
        {
            tecladoAnterior = Keyboard.GetState();
            controleAnterior = GamePad.GetState(PlayerIndex.One);

            this.spriteBatch = _principal.SpriteBatch;
            this.tela = _tela;

            itens = new List<string>();
            telas = new List<Tela>();
            menus = new List<Menu>();

            Esconder();
        }
        #endregion


        #region metodos
        public virtual void criarMenu(String[] lista)
        {
            itens.Clear();
            itens.AddRange(lista);

            menus.Clear();

            telas.Clear();

            CalcularMedidas();
        }

        public virtual void criarMenu(String[] lista, Tela[] listaTelas)
        {
            itens.Clear();
            itens.AddRange(lista);

            telas.Clear();
            telas.AddRange(listaTelas);

            menus.Clear();

            CalcularMedidas();
        }

        public virtual void criarMenu(String[] lista, Menu[] listaMenus)
        {
            itens.Clear();
            itens.AddRange(lista);

            menus.Clear();
            menus.AddRange(listaMenus);

            telas.Clear();

            CalcularMedidas();
        }

        public virtual void criarMenu(String[] lista, Menu[] listaMenus, Tela[] listaTelas)
        {
            itens.Clear();
            itens.AddRange(lista);

            menus.Clear();
            menus.AddRange(listaMenus);

            telas.Clear();
            telas.AddRange(listaTelas);

            CalcularMedidas();
        }

        protected virtual void CalcularMedidas()
        {
            largura = 0;
            altura = 0;

            foreach (string item in itens)
            {
                Vector2 tamanho = Fontes.MenuSelecionado.MeasureString(item);
                if (tamanho.X > largura)
                {
                    largura = (int)tamanho.X;
                }
                altura += Fontes.MenuSelecionado.LineSpacing;
            }
        }

        public virtual void Esconder()
        {
            this.Visible = false;
            this.Enabled = false;
        }

        public virtual void Mostrar()
        {
            this.Visible = true;
            this.Enabled = true;
            this.itemSelecionado = 0;
        }

        public virtual void AbrirOpcao(int i)
        {
            if (i >= 0)
            {
                if (i < menus.Count || i < telas.Count)
                {
                    if (!Principal.Mudo) Sons.MenuOK.Play();
                }

                if (i < menus.Count)
                {
                    if (menus[i] != null)
                    {
                        Esconder();
                        menus[i].Mostrar();
                    }
                }

                if (i < telas.Count)
                {
                    if (telas[i] != null)
                    {
                        Principal.mostrarTela(telas[i]);
                    }
                }
            }
        }

        public virtual String Item()
        {
            return itens[ItemSelecionado];
        }

        public virtual void Item(String texto)
        {
            itens[ItemSelecionado] = texto;
        }
        #endregion


        #region metodosPadrao
        public override void Update(GameTime gameTime)
        {
            GamePadState controle = GamePad.GetState(PlayerIndex.One);
            KeyboardState teclado = Keyboard.GetState();

            if (this.Visible && this.Enabled)
            {
                bool baixo, cima;
                baixo = (tecladoAnterior.IsKeyDown(Keys.Down) && teclado.IsKeyUp(Keys.Down)) || (tecladoAnterior.IsKeyDown(Keys.S) && teclado.IsKeyUp(Keys.S));
                cima = (tecladoAnterior.IsKeyDown(Keys.Up) && teclado.IsKeyUp(Keys.Up)) || (tecladoAnterior.IsKeyDown(Keys.W) && teclado.IsKeyUp(Keys.W));
                baixo |= (controleAnterior.DPad.Down == ButtonState.Pressed && controle.DPad.Down == ButtonState.Released);
                cima |= (controleAnterior.DPad.Up == ButtonState.Pressed && controle.DPad.Up == ButtonState.Released);

                if (baixo || cima) Sons.Menu.Play();

                if (baixo)
                {
                    itemSelecionado++;
                    if (itemSelecionado >= itens.Count)
                    {
                        itemSelecionado = 0;
                    }
                }
                else if (cima)
                {
                    itemSelecionado--;
                    if (itemSelecionado < 0)
                    {
                        itemSelecionado = itens.Count - 1;
                    }
                }
            }

            tecladoAnterior = teclado;
            controleAnterior = controle;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

            float y = posicao.Y;
            for (int i = 0; i < itens.Count; i++)
            {
                SpriteFont fonte;
                Color cor;

                if (i == itemSelecionado)
                {
                    fonte = Fontes.MenuSelecionado;
                    cor = Fontes.CorMenuSelecionado;
                }
                else
                {
                    fonte = Fontes.Menu;
                    cor = Fontes.CorMenu;
                }

                spriteBatch.DrawString(fonte, itens[i], new Vector2(posicao.X + 1f, y + 1f), Color.Black);
                spriteBatch.DrawString(fonte, itens[i], new Vector2(posicao.X, y), cor);

                y += fonte.LineSpacing;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override string ToString()
        {
            return String.Format("Menu: item selecionado = {0}", itemSelecionado);
        }
        #endregion
    }
}