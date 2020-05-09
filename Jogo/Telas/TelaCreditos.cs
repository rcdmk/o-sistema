using Jogo.Componentes;
using Microsoft.Xna.Framework.Media;

namespace Jogo.Telas
{
    public class TelaCreditos : TelaVideo
    {
        public TelaCreditos(Principal principal) : base(principal, "Videos\\CREDITOS", principal.telaInicial) { }

        public override void Mostrar()
        {
            if (MediaPlayer.State != MediaState.Stopped) MediaPlayer.Stop();
            base.Mostrar();
        }
    }
}
