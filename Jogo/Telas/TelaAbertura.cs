using Jogo.Componentes;

namespace Jogo.Telas
{
    public class TelaAbertura : TelaVideo
    {
        public TelaAbertura(Principal principal) : base(principal, "Content\\Videos\\Abertura.mp4", principal.telaJogo) { }
    }
}
