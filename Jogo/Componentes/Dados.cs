using System;
using System.Xml.Serialization;
using Jogo.Personagens;
using Microsoft.Xna.Framework;

namespace Jogo.Componentes
{
    /// <summary>
    /// Classe básica para armazenar os dados e serializar para salvar e carregar
    /// </summary>
    [Serializable]
    public class Dados
    {
        #region Propriedades
        public int Vidas { get; set; }

        public int ObjetivosCompletos { get; set; }

        public int ItensUsados { get; set; }

        public string Caminho { get; set; }

        public bool TelaInteira { get; set; }

        public ListaItens<ItemHUD> ItensHUD { get; set; }

        public ListaItens<Item> ItensMapa { get; set; }

        public ListaItens<Item> Coletaveis { get; set; }

        public ListaItens<Inimigo> Inimigos { get; set; }

        public ListaItens<Corda> Cordas { get; set; }

        public Vector2 PosicaoAgua { get; set; }

        public Vector2 MedidasAgua { get; set; }

        public string NivelAgua { get; set; }
        #endregion


        #region Contrutor
        public Dados() { }
        #endregion
    }
}
