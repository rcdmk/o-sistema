
namespace Jogo.Engine
{
    //Classe base para a física do jogo
    static class Fisica
    {
        #region Variaveis
        private static float gravidade = .5f;
        private static float friccao = 0.80f;
        #endregion

        #region Propriedades
        public static float Gravidade
        {
            get { return gravidade; }
            set { gravidade = value; }
        }

        public static float Friccao
        {
            get { return friccao; }
            set { friccao = value; }
        }
        #endregion
    }
}
