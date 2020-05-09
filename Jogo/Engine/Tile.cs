#region using
using System.Xml;
using Microsoft.Xna.Framework;
#endregion

namespace Jogo.Engine
{
    /// <summary>
    /// Classe base para todas as tiles
    /// </summary>
    public abstract class Tile
    {
        #region Variaveis
        private Vector2 posicao = Vector2.Zero;
        private static Vector2 dimensoes = new Vector2(64f, 64f);
        private int origem = 0;
        private Vector2 velocidade = Vector2.Zero;
        private bool passavel = true;
        private bool escada = false;
        private bool nuvem = false;
        private bool movel = false;
        private bool coletavel = false; 
        #endregion


        #region Construtor
        //####################################################
        protected Tile(Vector2 _posicao, int _origem, Vector2 _velocidade, bool _passavel, bool _escada, bool _nuvem, bool _movel, bool _coletavel)
        {
            this.posicao = _posicao;
            this.origem = _origem;
            this.velocidade = _velocidade;
            this.passavel = _passavel;
            this.escada = _escada;
            this.nuvem = _nuvem;
            this.movel = _movel;
            this.coletavel = _coletavel;
        } 
        #endregion


        #region Propriedades
        //####################################################
        public static Vector2 Dimensoes
        {
            get { return dimensoes; }
            set { dimensoes = value; }
        }

        public Vector2 Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public int Origem
        {
            get { return origem; }
            set { origem = value; }
        }

        public Vector2 Velocidade
        {
            get { return velocidade; }
            set { velocidade = value; }
        }

        public bool Passavel
        {
            get { return passavel; }
            set { passavel = value; }
        }

        public bool Escada
        {
            get { return escada; }
            set { escada = value; }
        }

        public bool Nuvem
        {
            get { return nuvem; }
            set { nuvem = value; }
        }

        public bool Movel
        {
            get { return movel; }
            set { movel = value; }
        }

        public bool Coletavel
        {
            get { return coletavel; }
            set { coletavel = value; }
        } 
        #endregion


        #region metodos
        public void carregaPropriedades(XmlAttributeCollection atributos)
        {
            passavel = atributos[0].Value == "s";
            escada = atributos[1].Value == "s";
            nuvem = atributos[2].Value == "s";
        }
        #endregion
    }
}
