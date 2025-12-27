using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using LibVLCSharp.Shared;

namespace Jogo.Componentes
{
    /// <summary>
    /// Video player interface compatible with XNA MediaPlayer API.
    /// Provides video playback control and frame extraction as Texture2D.
    /// </summary>
    public interface IVideoPlayer : IDisposable
    {
        /// <summary>
        /// Gets the current playback state.
        /// </summary>
        MediaState State { get; }

        /// <summary>
        /// Gets or sets the mute state.
        /// </summary>
        bool IsMuted { get; set; }

        /// <summary>
        /// Gets or sets the playback position (0.0 to 1.0).
        /// </summary>
        float PlayPosition { get; set; }

        /// <summary>
        /// Occurs when the video playback reaches the end.
        /// </summary>
        event EventHandler<EventArgs> EndReached;

        /// <summary>
        /// LoadMedia a media object from the specified file path.
        /// </summary>
        void LoadMedia(string path);

        /// <summary>
        /// Starts playback of the loaded video.
        /// </summary>
        void Play();

        /// <summary>
        /// Pauses the current playback.
        /// </summary>
        void Pause();

        /// <summary>
        /// Stops the current playback.
        /// </summary>
        void Stop();

        /// <summary>
        /// Extracts the current video frame as a Texture2D.
        /// Returns null if no frame is available.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device to use for texture creation.</param>
        /// <returns>A Texture2D containing the current video frame, or null.</returns>
        Texture2D GetCurrentFrame(GraphicsDevice graphicsDevice);
    }
}
