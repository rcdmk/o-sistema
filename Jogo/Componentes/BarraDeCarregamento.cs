using Microsoft.Xna.Framework;


namespace Jogo.Componentes
{
    /// <summary>
    /// Classe de barra de carregamento simples
    /// </summary>
    public class BarraDeCarregamento : DrawableGameComponent
    {
        #region Variáveis
        private Principal principal;
        private Rectangle areaBarra;
        private Vector2 posicao;
        private int largura;
        private int altura;
        private float progresso;
        #endregion

        #region Propriedades
        public Vector2 Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public float Progresso
        {
            get { return progresso; }
            set { progresso = MathHelper.Clamp(value, 0f, 1f); }
        }
        #endregion

        #region Construtor
        public BarraDeCarregamento(Principal principal, Vector2 _posicao, int largura, int altura)
            : base(principal)
        {
            this.principal = principal;
            this.posicao = _posicao;
            this.largura = largura;
            this.altura = altura;
            this.progresso = 0f;
            this.areaBarra = new Rectangle((int)posicao.X, (int)posicao.Y, largura, altura);
        }
        #endregion

        #region Métodos Padrão
        public override void Draw(GameTime gameTime)
        {
            principal.SpriteBatch.Begin();
            // Desenha a borda da barra de carregamento
            principal.SpriteBatch.Draw(principal.texturaBranca, areaBarra, Color.White);

            // Desenha o preenchimento da barra de carregamento com base no progresso
            Rectangle areaPreenchida = new Rectangle(areaBarra.X + 2, areaBarra.Y + 2, (int)((areaBarra.Width - 4) * progresso), areaBarra.Height - 4);
            principal.SpriteBatch.Draw(principal.texturaBranca, areaPreenchida, Color.Green);
            principal.SpriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
    }
}