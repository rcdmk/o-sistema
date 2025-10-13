using Jogo.Componentes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jogo.Telas
{
    public class TelaLogo : Tela
    {
        #region Variaveis
        protected Fader fader;
        protected float tempo = 0;
        protected bool iniciado = false;
        protected bool finalizado = false;
        #endregion


        #region Construtor
        public TelaLogo(Principal _principal)
            : base(_principal)
        { }
        #endregion


        #region Metodos padrao
        protected override void LoadContent()
        {
            Componentes.Add(new ImagemCentral(principal, principal.Content.Load<Texture2D>("Sprites\\Geral\\REATOR_logo"), ImagemCentral.Modo.Centralizado));

            fader = new Fader(principal, "Sprites\\Componentes\\FadeIn", "Sprites\\Componentes\\FadeOut");
            Componentes.Add(fader);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (iniciado && !finalizado)
            {
                tempo += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (tempo >= 3)
                {
                    fader.FadeOut();
                    tempo = 0;
                    finalizado = true;
                }
            }
            else if (finalizado)
            {
                tempo += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (tempo >= 0.5f)
                {
                    Principal.mostrarTela(principal.telaInicial);
                    tempo = 0;
                }
            }
            else
            {
                fader.FadeIn();
                iniciado = true;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (fader.Visivel)
            {
                principal.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                fader.Draw(gameTime, principal.SpriteBatch);
                principal.SpriteBatch.End();
            }
        }
        #endregion
    }
}
