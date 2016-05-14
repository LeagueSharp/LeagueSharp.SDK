// <copyright file="DefaultTextures.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx.UI.Skins.Default
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp.SDKEx.Properties;

    using SharpDX.Direct3D9;

    /// <summary>
    ///     The Default Texture Types.
    /// </summary>
    internal enum DefaultTexture
    {
        /// <summary>
        ///     The Dragging Type.
        /// </summary>
        Dragging
    }

    /// <summary>
    ///     The Default Textures.
    /// </summary>
    internal class DefaultTextures
    {
        #region Static Fields

        /// <summary>
        ///     The Instance.
        /// </summary>
        public static readonly DefaultTextures Instance = new DefaultTextures();

        #endregion

        #region Fields

        private readonly Dictionary<DefaultTexture, TextureWrapper> textures =
            new Dictionary<DefaultTexture, TextureWrapper>();

        #endregion

        #region Constructors and Destructors

        private DefaultTextures()
        {
            this.textures[DefaultTexture.Dragging] = BuildTexture(Resources.cursor_drag, 16, 16);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="DefaultTextures" /> class.
        /// </summary>
        ~DefaultTextures()
        {
            foreach (var entry in this.textures.Where(entry => !entry.Value.Texture.IsDisposed))
            {
                entry.Value.Texture.Dispose();
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     The Indexer.
        /// </summary>
        /// <param name="textureType">
        ///     The Texture Type.
        /// </param>
        /// <returns>
        ///     The <see cref="TextureWrapper" />.
        /// </returns>
        public TextureWrapper this[DefaultTexture textureType] => this.textures[textureType];

        #endregion

        #region Methods

        /// <summary>
        ///     Builds the texture.
        /// </summary>
        /// <param name="bmp">
        ///     The image
        /// </param>
        /// <param name="height">
        ///     The height
        /// </param>
        /// <param name="width">
        ///     The width
        /// </param>
        /// <returns>
        ///     The <see cref="TextureWrapper" />
        /// </returns>
        private static TextureWrapper BuildTexture(Image bmp, int height, int width)
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
            return new TextureWrapper(texture, width, height);
        }

        #endregion
    }

    /// <summary>
    ///     The Texture Wrapper.
    /// </summary>
    internal class TextureWrapper
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextureWrapper" /> class.
        /// </summary>
        /// <param name="texture">
        ///     The texture
        /// </param>
        /// <param name="width">
        ///     The width
        /// </param>
        /// <param name="height">
        ///     The height
        /// </param>
        public TextureWrapper(Texture texture, int width, int height)
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
        ///     Gets the Texture.
        /// </summary>
        public Texture Texture { get; }

        /// <summary>
        ///     Gets the Width.
        /// </summary>
        public int Width { get; private set; }

        #endregion
    }
}