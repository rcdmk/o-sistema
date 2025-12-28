using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using LibVLCSharp;
using LibVLCSharp.Shared;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace Jogo.Componentes
{
    public class VideoPlayer : IVideoPlayer
    {
        private LibVLC libvlc;
        private MediaPlayer mediaPlayer;
        private Media media;
        private int videoWidth;
        private int videoHeight;
        private byte[] frameBuffer;
        private GCHandle frameBufferHandle;
        private bool isDisposed;

        public event EventHandler<EventArgs> EndReached;

        public MediaState State
        {
            get
            {
                switch (this.mediaPlayer.State)
                {
                    case VLCState.Playing:
                        return MediaState.Playing;
                    case VLCState.Paused:
                        return MediaState.Paused;
                    default:
                        return MediaState.Stopped;
                }
            }
        }

        public bool IsMuted
        {
            get { return mediaPlayer.Mute; }
            set { mediaPlayer.Mute = value; }
        }

        public float PlayPosition
        {
            get { return mediaPlayer.Position; }
            set { mediaPlayer.Position = value; }
        }

        /// <summary>
        /// Creates a VideoPlayer that extracts frames via callbacks without requiring a window.
        /// </summary>
        public VideoPlayer(Principal principal)
        {
            libvlc = new LibVLC();
            mediaPlayer = new MediaPlayer(libvlc);
            mediaPlayer.EnableKeyInput = false;
            mediaPlayer.EnableMouseInput = false;
            mediaPlayer.EnableHardwareDecoding = false;
            mediaPlayer.Fullscreen = false;
            mediaPlayer.EndReached += endReached;

            // Set up frame callbacks for efficient pixel data extraction
            // No window needed - callbacks handle rendering directly into our buffer
            SetupFrameCallbacks();
        }

        ~VideoPlayer()
        {
            Dispose();
        }

        public void LoadMedia(string path)
        {
            media = new Media(libvlc, path);
        }

        public void Play()
        {
            mediaPlayer.Play(media);
        }

        public void Pause()
        {
            mediaPlayer.Pause();
        }

        public void Stop()
        {
            mediaPlayer.EnableKeyInput = false;
            mediaPlayer.Stop();
        }

        public Texture2D GetCurrentFrame(GraphicsDevice graphicsDevice)
        {
            // Return frame if available from callbacks
            if (frameBuffer != null && videoWidth > 0 && videoHeight > 0)
            {
                try
                {
                    Texture2D texture = new Texture2D(graphicsDevice, videoWidth, videoHeight);
                    texture.SetData(frameBuffer);
                    return texture;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;

            mediaPlayer.EndReached -= endReached;
            media?.Dispose();
            mediaPlayer.Dispose();
            libvlc.Dispose();

            if (frameBufferHandle.IsAllocated)
                frameBufferHandle.Free();
        }

        private void SetupFrameCallbacks()
        {
            // Set format callback - request format from VLC
            mediaPlayer.SetVideoFormatCallbacks(
                (ref IntPtr opaque, IntPtr chroma, ref uint width, ref uint height, ref uint pitches, ref uint lines) =>
                {
                    videoWidth = (int)width;
                    videoHeight = (int)height;

                    // Request RGBA format by writing FourCC code
                    // Write as bytes to ensure proper endianness
                    Marshal.WriteByte(chroma, 0, (byte)'R');
                    Marshal.WriteByte(chroma, 1, (byte)'G');
                    Marshal.WriteByte(chroma, 2, (byte)'B');
                    Marshal.WriteByte(chroma, 3, (byte)'A');

                    // Allocate buffer for RGBA format (4 bytes per pixel)
                    frameBuffer = new byte[videoWidth * videoHeight * 4];
                    frameBufferHandle = GCHandle.Alloc(frameBuffer, GCHandleType.Pinned);
                    // Set pitch (bytes per row) for RGBA
                    pitches = (uint)(videoWidth * 4);
                    lines = height;
                    return 1; // Return 1 picture buffer
                },
                null // No cleanup callback needed
            );

            // Set frame callbacks - LibVLC calls these during playback
            mediaPlayer.SetVideoCallbacks(
                (IntPtr opaque, IntPtr planes) =>
                {
                    // Lock callback - provide buffer to write frame data
                    if (frameBuffer != null && frameBufferHandle.IsAllocated)
                    {
                        IntPtr addr = frameBufferHandle.AddrOfPinnedObject();
                        Marshal.WriteIntPtr(planes, addr);
                        return addr;
                    }
                    return IntPtr.Zero;
                },
                null, // No unlock callback needed
                null  // No display callback needed
            );
        }

        private void endReached(object sender, EventArgs e)
        {
            OnEndReached(e);
        }

        protected virtual void OnEndReached(EventArgs e)
        {
            EndReached?.Invoke(this, e);
        }
    }
}