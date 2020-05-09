#region using
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Jogo.Componentes
{
    public class TelaAjuda :Tela
    {
        public TelaAjuda(Principal _principal, Texture2D _fundo, Texture2D _textura):base(_principal)
        {
            Componentes.Add(new ImagemCentral(_principal, _fundo, ImagemCentral.Modo.Esticado));
            Componentes.Add(new ImagemCentral(_principal, _textura , ImagemCentral.Modo.Centralizado));
        }
    }
}
