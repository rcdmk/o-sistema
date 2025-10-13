using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jogo.Componentes
{
    public class MenuImagem : Menu
    {
        #region Variaveis
        protected List<Texture2D> imagens;
        protected bool[] ativados = new bool[2];
        protected bool[] ativacao = new bool[2];
        protected int espaco = -10;
        protected Texture2D fundo;
        protected Color cor = Color.White;
        protected Color corSelecionado = Color.Red;
        protected Rectangle retangulo;
        protected bool texto = false;
        #endregion


        #region Propriedades
        public int Espaco
        {
            get { return espaco; }
            set { espaco = value; }
        }

        public Texture2D Fundo
        {
            get { return fundo; }
            set { fundo = value; }
        }

        public bool Texto
        {
            get { return texto; }
            set { texto = value; }
        }

        public bool[] Ativacao
        {
            get { return ativacao; }
            set { ativacao = value; }
        }

        public bool[] Ativados
        {
            get { return ativados; }
            set { ativados = value; }
        }
        #endregion


        #region Contrutor
        public MenuImagem(Principal _principal, Tela _tela, Texture2D _fundo)
            : base(_principal, _tela)
        {
            imagens = new List<Texture2D>();
            this.fundo = _fundo;
        }
        #endregion


        #region Metodos Padrao
        public override void Update(GameTime gameTime)
        {
            GamePadState controle = GamePad.GetState(PlayerIndex.One);
            KeyboardState teclado = Keyboard.GetState();

            if (this.Visible)
            {
                bool baixo, cima;
                baixo = (tecladoAnterior.IsKeyDown(Keys.Down) && teclado.IsKeyUp(Keys.Down)) || (tecladoAnterior.IsKeyDown(Keys.S) && teclado.IsKeyUp(Keys.S));
                cima = (tecladoAnterior.IsKeyDown(Keys.Up) && teclado.IsKeyUp(Keys.Up)) || (tecladoAnterior.IsKeyDown(Keys.W) && teclado.IsKeyUp(Keys.W));
                baixo |= (controleAnterior.DPad.Down == ButtonState.Pressed && controle.DPad.Down == ButtonState.Released);
                cima |= (controleAnterior.DPad.Up == ButtonState.Pressed && controle.DPad.Up == ButtonState.Released);

                if (baixo || cima)
                {
                    if (!Principal.Mudo) Sons.Menu.Play();
                }

                if (baixo)
                {
                    itemSelecionado++;
                    if (itemSelecionado >= imagens.Count)
                    {
                        itemSelecionado = 0;
                    }
                }
                else if (cima)
                {
                    itemSelecionado--;
                    if (itemSelecionado < 0)
                    {
                        itemSelecionado = imagens.Count - 1;
                    }
                }
            }

            tecladoAnterior = teclado;
            controleAnterior = controle;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            //Posicionar a imagem de fundo entralizada no menu
            Vector2 posicaoFundo = new Vector2();
            posicaoFundo.X = posicao.X - (fundo.Width - largura) / 2;
            posicaoFundo.Y = posicao.Y - (fundo.Height - altura) / 2;

            //Desenha o fundo
            spriteBatch.Draw(fundo, posicaoFundo, Color.White);


            //Posiciona o primeiro item
            Vector2 posicaoItem = posicao;
            posicaoItem.Y += 25;


            //Posiciona e desenha os itens do menu
            for (int i = 0; i < imagens.Count; i++)
            {
                //Define o retângulo de origem com a opção selecionada ou não
                retangulo.Width = imagens[i].Width / 2;
                retangulo.Height = imagens[i].Height;

                //Posiciona o item
                if (i > 0) posicaoItem.Y += imagens[i - 1].Height;

                //Se existir a lista de ativados
                if (ativacao.Length > 0 && i < ativacao.Length)
                {
                    //Se não for opção de ativação (on/off)
                    if (!ativacao[i])
                    {
                        //Seleciona com base no item selecionado
                        if (itemSelecionado == i)
                        {
                            retangulo.X = imagens[i].Width / 2;
                        }
                        else
                        {
                            retangulo.X = 0;
                        }
                        posicaoItem.X = posicao.X - (retangulo.Width - largura) / 2;
                        posicaoItem.Y += espaco;
                    }
                    else
                    {
                        retangulo.Width = imagens[i].Width / 4;

                        //Caso contrário, se existir uma lista de itens ativados
                        if (ativados.Length > 0 && i < ativados.Length)
                        {
                            //Seleciona com base na lista de ativados
                            if (ativados[i])
                            {
                                retangulo.X = (itemSelecionado == i) ? retangulo.Width + imagens[i].Width / 2 : imagens[i].Width / 2;
                            }
                            else
                            {
                                retangulo.X = (itemSelecionado == i) ? imagens[i].Width / 4 : 0;
                            }
                        }

                        posicaoItem.X = posicao.X + 100 - (retangulo.Width - largura) / 2;
                        posicaoItem.Y += espaco / 2;
                    }
                }
                else
                {
                    //Seleciona com base no item selecionado
                    if (itemSelecionado == i)
                    {
                        retangulo.X = imagens[i].Width / 2;
                    }
                    else
                    {
                        retangulo.X = 0;
                    }
                    posicaoItem.X = posicao.X - (retangulo.Width - largura) / 2;
                    posicaoItem.Y += espaco;
                }

                //desenha o item na tela
                spriteBatch.Draw(imagens[i], posicaoItem, retangulo, cor);
            }

            spriteBatch.End();

            //se for um menu de texto, desenha o texto (não ajustado ainda)
            if (texto) base.Draw(gameTime);
        }
        #endregion


        #region Metodos
        public void criarMenu(Texture2D[] lista)
        {
            imagens.Clear();
            imagens.AddRange(lista);

            menus.Clear();

            telas.Clear();

            CalcularMedidas();
        }

        public void criarMenu(Texture2D[] lista, Menu[] listaMenus)
        {
            imagens.Clear();
            imagens.AddRange(lista);

            menus.Clear();
            menus.AddRange(listaMenus);

            telas.Clear();

            CalcularMedidas();
        }

        public void criarMenu(Texture2D[] lista, Tela[] listaTelas)
        {
            imagens.Clear();
            imagens.AddRange(lista);

            menus.Clear();

            telas.Clear();
            telas.AddRange(listaTelas);

            CalcularMedidas();
        }

        public void criarMenu(Texture2D[] lista, Menu[] listaMenus, Tela[] listaTelas)
        {
            imagens.Clear();
            imagens.AddRange(lista);

            menus.Clear();
            menus.AddRange(listaMenus);

            telas.Clear();
            telas.AddRange(listaTelas);

            CalcularMedidas();
        }

        protected override void CalcularMedidas()
        {
            largura = 0;
            altura = 0;

            for (int i = 0; i < imagens.Count; i++)
            {
                if (imagens[i].Width / 2 > largura) largura = imagens[i].Width / 2;

                if (i < imagens.Count - 1)
                {
                    altura += imagens[i].Height + espaco;
                }
                else
                {
                    altura += imagens[i].Height;
                }
            }
        }
        #endregion
    }
}
