// <copyright file="LightTextures2.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Light2
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Properties;

    using SharpDX.Direct3D9;

    /// <summary>
    ///     The light texture enum.
    /// </summary>
    internal enum LightTexture2
    {
        /// <summary>
        ///     The Dragging type.
        /// </summary>
        Dragging,
    }

    /// <summary>
    ///     The light textures.
    /// </summary>
    internal class LightTextures2
    {
        #region Static Fields

        /// <summary>
        ///     The instance.
        /// </summary>
        public static readonly LightTextures2 Instance = new LightTextures2();

        #endregion

        #region Fields

        private readonly Dictionary<LightTexture2, BlueTextureWrapper> textures =
            new Dictionary<LightTexture2, BlueTextureWrapper>();

        #endregion

        #region Constructors and Destructors

        private LightTextures2()
        {
            this.textures[LightTexture2.Dragging] = BuildTexture(Resources.cursor_drag, 16, 16);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="LightTextures2" /> class.
        /// </summary>
        ~LightTextures2()
        {
            foreach (var entry in this.textures.Where(entry => !entry.Value.Texture.IsDisposed))
            {
                entry.Value.Texture.Dispose();
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     The public indexer.
        /// </summary>
        /// <param name="textureType">
        ///     The texture type.
        /// </param>
        /// <returns>
        ///     The <see cref="BlueTextureWrapper" />.
        /// </returns>
        public BlueTextureWrapper this[LightTexture2 textureType] => this.textures[textureType];

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Add a texture.
        /// </summary>
        /// <param name="bmp">
        ///     The bitmap.
        /// </param>
        /// <param name="width">
        ///     The width.
        /// </param>
        /// <param name="height">
        ///     The height.
        /// </param>
        /// <param name="textureType">
        ///     The texture type.
        /// </param>
        /// <returns>
        ///     The <see cref="BlueTextureWrapper" />.
        /// </returns>
        public BlueTextureWrapper AddTexture(Image bmp, int width, int height, LightTexture2 textureType)
        {
            this.textures[textureType] = BuildTexture(bmp, height, width);
            return this.textures[textureType];
        }

        #endregion

        #region Methods

        private static BlueTextureWrapper BuildTexture(Image bmp, int height, int width)
        {
            var resized = new Bitmap(bmp, width, height);
            var texture = Texture.FromMemory(
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

        #endregion
    }

    /// <summary>
    ///     The blue texture wrapper.
    /// </summary>
    internal class BlueTextureWrapper
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlueTextureWrapper" /> class.
        /// </summary>
        /// <param name="texture">
        ///     The texture.
        /// </param>
        /// <param name="width">
        ///     The width.
        /// </param>
        /// <param name="height">
        ///     The height.
        /// </param>
        public BlueTextureWrapper(Texture texture, int width, int height)
        {
            this.Texture = texture;
            this.Width = width;
            this.Height = height;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the Height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        ///     Gets the texture.
        /// </summary>
        public Texture Texture { get; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        public int Width { get; private set; }

        #endregion
    }
}