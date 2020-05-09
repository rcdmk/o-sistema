#region Using
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Jogo.Componentes
{
    public class HUD : GameComponent
    {
        #region Variaveis
        protected Principal principal;
        protected Vector2 posicao;
        protected Texture2D fundo;

        //Itens
        protected ListaItens<ItemHUD> itens;
        protected Vector2 posicaoVidas = Vector2.Zero;
        protected Vector2 offset = new Vector2(0, -10);
        public static Vector2 tamanhoItens = new Vector2(64);

        //Selecao
        protected Texture2D selecao;
        protected int itemSelecionado = 0;
        protected Vector2 posicaoSelecao = Vector2.Zero;
        #endregion


        #region Propriedades
        public Vector2 Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public Texture2D Fundo
        {
            get { return fundo; }
            set { fundo = value; }
        }

        public ListaItens<ItemHUD> Itens
        {
            get { return itens; }
            set { itens = value; }
        }

        public Vector2 PosicaoVidas
        {
            get { return posicaoVidas; }
            set { posicaoVidas = value; }
        }

        public int ItemSelecionado
        {
            get { return itemSelecionado; }
            set { itemSelecionado = value; }
        }
        #endregion


        #region Construtor
        public HUD(Principal _principal, Vector2 _posicao, Texture2D _fundo) : base(_principal)
        {
            this.principal = _principal;
            this.posicao = _posicao;
            this.fundo = _fundo;
            this.itens = new ListaItens<ItemHUD>();

            LoadContent(_principal.Content);
        }
        #endregion


        #region Metodos Padrao
        public void LoadContent(ContentManager Content)
        {
            selecao = Content.Load<Texture2D>("Sprites//Interface//selecao");
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            posicaoSelecao = posicao = principal.telaJogo.mapa.PosicaoCamera;
            posicaoSelecao.X += offset.X + (itemSelecionado * tamanhoItens.X);
            
            spriteBatch.Draw(fundo, posicao, Color.White);

            for (int i = 0; i < itens.Count; i++)
            {
                itens[i].PosicaoHUD = posicao;
                itens[i].Draw(gameTime, spriteBatch);
            }

            if (itens.Count > 0) spriteBatch.Draw(selecao, posicaoSelecao, Color.White);

            //Vidas
            spriteBatch.DrawString(Fontes.Score, String.Format("0{0}", principal.telaJogo.Personagem.Vidas), posicao + new Vector2(fundo.Width - 50, 10), Fontes.CorScore);
        }

        public override string ToString()
        {
            String descricao = "HUD: \n\tItens:";

            for (int i = 0; i < itens.Count; i++ )
            {
                descricao += itens[i].Nome + ";\n";
            }

            descricao += "Vidas: " + principal.telaJogo.Personagem.Vidas;

            return descricao;
        }
        #endregion


        #region Metodos
        public bool addItem(String tipo, String nomeTextura)
        {
            if (itens.Count < 9)
            {
                ItemHUD novoItem = new ItemHUD(principal, new Vector2(offset.X + (itens.Count * tamanhoItens.X), offset.Y), tipo, nomeTextura);
                novoItem.Initialize();
                itens.Add(novoItem);
                return true;
            }
            return false;
        }

        public void removerItem(int item)
        {
            if (item < 0 && item >= itens.Count) return;

            itens[item].Dispose();
            itens.RemoveAt(item);


            Vector2 posicaoItem = new Vector2();
            for (int i = item; i < itens.Count; i++ )
            {
                posicaoItem = itens[i].Posicao;
                posicaoItem.X -= HUD.tamanhoItens.X;
                itens[i].Posicao = posicaoItem;
            }

            if (itemSelecionado > item)
            {
                itemSelecionado--;
            }
            else if (itemSelecionado == item)
            {
                itemSelecionado = 0;
            }

            if (itemSelecionado < 0) itemSelecionado = 0;
        }

        public void limpar()
        {
            itens.Clear();
            itemSelecionado = 0;
        }
        #endregion
    }
}
