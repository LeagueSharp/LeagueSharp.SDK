using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Blue
{
    using System.Drawing;

    using LeagueSharp.SDK.Properties;

    using SharpDX;
    using SharpDX.Direct3D9;

    internal enum BlueTexture
    {
        Dragging,
    }

    internal class BlueTextures
    {

        private readonly Dictionary<BlueTexture, BlueTextureWrapper> textures = new Dictionary<BlueTexture, BlueTextureWrapper>();

        public static readonly BlueTextures Instance = new BlueTextures();

        private BlueTextures()
        {
            this.textures[BlueTexture.Dragging] = BuildTexture(Resources.cursor_drag, 16, 16);
        }

        ~BlueTextures()
        {
            foreach (var entry in this.textures.Where(entry => !entry.Value.Texture.IsDisposed)) {
                entry.Value.Texture.Dispose();
            }
        }

        public BlueTextureWrapper this[BlueTexture textureType]
        {
            get
            {
                return this.textures[textureType];
            }
        }

        private BlueTextureWrapper BuildTexture(Image bmp, int height, int width)
        {
            var resized = new Bitmap(bmp, width, height);
            var texture =  Texture.FromMemory(
                Drawing.Direct3DDevice,
                (byte[])new ImageConverter().ConvertTo(resized, typeof(byte[])),
                resized.Width,
                resized.Height,
                0,
                Usage.None,
                Format.A1,
                Pool.Managed,
                Filter.Default,
                Filter.Default,
                0);
            resized.Dispose();
            bmp.Dispose();
            return new BlueTextureWrapper(texture, width, height);
        }

        public BlueTextureWrapper AddTexture(Image bmp, int width, int height, BlueTexture textureType)
        {
            this.textures[textureType] = BuildTexture(bmp, height, width);
            return this.textures[textureType];
        }
        
    }

    internal class BlueTextureWrapper
    {
        public Texture Texture { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public BlueTextureWrapper(Texture texture, int width, int height)
        {
            this.Texture = texture;
            this.Width = width;
            this.Height = height;
        }
        
    }
}
