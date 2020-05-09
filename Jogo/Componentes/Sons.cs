#region using
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
#endregion

namespace Jogo.Componentes
{
    /// <summary>
    /// Classe de banco de sons do jogo
    /// </summary>
    public static class Sons
    {
        #region Variaveis
        //Sons
        private static SoundEffect morrendo;
        private static SoundEffect golpe;

        private static SoundEffect moeda;
        private static SoundEffect pegandoItem;
        private static SoundEffect item;

        private static SoundEffect menu;
        private static SoundEffect menuOK;

        private static SoundEffect queda;
        private static SoundEffect arrastar;
        private static SoundEffect arrastarLeve;

        private static SoundEffect porta;
        private static SoundEffect elevador;
        private static SoundEffect escada;

        private static SoundEffect agua;
        private static SoundEffect goteira;
        private static SoundEffect aguaImpacto;
        private static SoundEffect aguaImpactoLeve;
        private static SoundEffect vidro;

        private static SoundEffect manivela;

        //Musicas
        private static Song musicaAbertura;
        private static Song musicaInicio;
        private static Song musicaAgua;
        private static Song musicaRoldanas;
        #endregion


        #region Propriedades
        //Sons
        public static SoundEffect Morrendo
        {
            get { return morrendo; }
        }

        public static SoundEffect Golpe
        {
            get { return golpe; }
        }

        public static SoundEffect Moeda
        {
            get { return moeda; }
        }

        public static SoundEffect PegandoItem
        {
            get { return pegandoItem; }
        }

        public static SoundEffect Item
        {
            get { return item; }
        }

        public static SoundEffect Menu
        {
            get { return menu; }
        }

        public static SoundEffect MenuOK
        {
            get { return menuOK; }
        }

        public static SoundEffect Queda
        {
            get { return queda; }
        }

        public static SoundEffect Arrastar
        {
            get { return arrastar; }
        }

        public static SoundEffect ArrastarLeve
        {
            get { return arrastarLeve; }
        }

        public static SoundEffect Porta
        {
            get { return porta; }
        }

        public static SoundEffect Elevador
        {
            get { return elevador; }
        }

        public static SoundEffect Escada
        {
            get { return escada; }
        }

        public static SoundEffect Goteira
        {
            get { return goteira; }
        }

        public static SoundEffect Agua
        {
            get { return agua; }
        }

        public static SoundEffect AguaImpacto
        {
            get { return aguaImpacto; }
        }

        public static SoundEffect AguaImpactoLeve
        {
            get { return aguaImpactoLeve; }
        }

        public static SoundEffect Vidro
        {
            get { return vidro; }
        }

        public static SoundEffect Manivela
        {
            get { return manivela; }
        }

        //Musicas
        public static Song MusicaAbertura
        {
            get { return musicaAbertura; }
        }

        public static Song MusicaInicio
        {
            get { return musicaInicio; }
        }

        public static Song MusicaAgua
        {
            get { return musicaAgua; }
        }

        public static Song MusicaRoldanas
        {
            get { return musicaRoldanas; }
        }
        #endregion


        #region Metodos Padrao
        public static void LoadContent(ContentManager Content)
        {
            //Sons
            morrendo = Content.Load<SoundEffect>("Sons\\Geral\\morrendo");
            golpe = Content.Load<SoundEffect>("Sons\\Geral\\golpe");

            moeda = Content.Load<SoundEffect>("Sons\\Geral\\moeda");
            pegandoItem = Content.Load<SoundEffect>("Sons\\Geral\\pegando_item");
            item = Content.Load<SoundEffect>("Sons\\Geral\\item");

            menu = Content.Load<SoundEffect>("Sons\\Menus\\menu_navegacao");
            menuOK = Content.Load<SoundEffect>("Sons\\Menus\\confirmar");

            queda = Content.Load<SoundEffect>("Sons\\Geral\\caindo");
            arrastar = Content.Load<SoundEffect>("Sons\\Agua\\arrastar");
            arrastarLeve = Content.Load<SoundEffect>("Sons\\Agua\\arrastar_leve");

            porta = Content.Load<SoundEffect>("Sons\\Geral\\porta_edit");
            elevador = Content.Load<SoundEffect>("Sons\\Geral\\elevador_edit");
            escada = Content.Load<SoundEffect>("Sons\\Geral\\escada");

            goteira = Content.Load<SoundEffect>("Sons\\Agua\\goteira");
            agua = Content.Load<SoundEffect>("Sons\\Agua\\agua");
            aguaImpacto = Content.Load<SoundEffect>("Sons\\Agua\\impacto_agua_pesado");
            aguaImpactoLeve = Content.Load<SoundEffect>("Sons\\Agua\\impacto_agua_leve");
            vidro = Content.Load<SoundEffect>("Sons\\Agua\\vidro_quebrando");

            manivela = Content.Load<SoundEffect>("Sons\\Roldanas\\roldana_edit");

            //Musicas
            musicaAbertura = Content.Load<Song>("Sons\\Musicas\\Introdution2");
            musicaInicio = Content.Load<Song>("Sons\\Musicas\\som2");
            musicaAgua = Content.Load<Song>("Sons\\Musicas\\som6");
            musicaRoldanas = Content.Load<Song>("Sons\\Musicas\\som12");
        }
        #endregion
    }
}