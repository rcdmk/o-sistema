#region using
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Jogo.Componentes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public static class Fontes
    {
        #region variaveis
        //Fontes
        private static SpriteFont menu;
        private static SpriteFont menuSelecionado;
        private static SpriteFont score;
        private static SpriteFont creditos;
        private static SpriteFont tituloCreditos;

        //Tamanhos
        private static int tamanhoMenu = 14;
        private static int tamanhoMenuSelecionado = 15;
        private static int tamanhoScore = 15;
        private static int tamanhoCreditos = 10;
        private static int tamanhoTituloCreditos = 12;

        //Cores
        private static Color corMenu = new Color(119, 170, 54);
        private static Color corMenuSelecionado = new Color(33, 147, 195);
        private static Color corScore = Color.White;
        private static Color corCreditos = Color.White;
        private static Color corTituloCreditos = new Color(33, 147, 195);
        #endregion

        #region propriedades
        //Fontes
        public static SpriteFont Menu
        {
            get { return menu; }
            set { menu = value; }
        }

        public static SpriteFont MenuSelecionado
        {
            get { return menuSelecionado; }
            set { menuSelecionado = value; }
        }

        public static SpriteFont Score
        {
            get { return score; }
            set { score = value; }
        }

        public static SpriteFont Creditos
        {
            get { return creditos; }
            set { creditos = value; }
        }

        public static SpriteFont TituloCreditos
        {
            get { return tituloCreditos; }
            set { tituloCreditos = value; }
        }

        //Tamanhos
        public static int TamanhoMenu
        {
            get { return tamanhoMenu; }
            set { tamanhoMenu = value; }
        }

        public static int TamanhoMenuSelecionado
        {
            get { return tamanhoMenuSelecionado; }
            set { tamanhoMenuSelecionado = value; }
        }

        public static int TamanhoScore
        {
            get { return tamanhoScore; }
            set { tamanhoScore = value; }
        }

        public static int TamanhoCreditos
        {
            get { return tamanhoCreditos; }
            set { tamanhoCreditos = value;}
        }

        public static int TamanhoTituloCreditos
        {
            get { return tamanhoTituloCreditos; }
            set { tamanhoTituloCreditos = value; }
        }

        //Cores
        public static Color CorMenu
        {
            get { return corMenu; }
            set { corMenu = value; }
        }

        public static Color CorMenuSelecionado
        {
            get { return corMenuSelecionado; }
            set { corMenuSelecionado = value; }
        }

        public static Color CorScore
        {
            get { return corScore; }
            set { corScore = value; }
        }

        public static Color CorCreditos
        {
            get { return corCreditos; }
            set { corCreditos = value; }
        }

        public static Color CorTituloCreditos
        {
            get { return corTituloCreditos; }
            set { corTituloCreditos = value; }
        }
        #endregion
    }
}