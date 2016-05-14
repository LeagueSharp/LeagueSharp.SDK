﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColoredTextures.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   A custom implementation of <see cref="ADrawable{MenuTexture}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDKEx.UI.Skins.Colored
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp.SDKEx.Properties;

    using SharpDX.Direct3D9;

    internal enum ColoredTexture
    {
        Dragging,
    }

    internal class ColoredTextures
    {
        #region Static Fields

        public static readonly ColoredTextures Instance = new ColoredTextures();

        #endregion

        #region Fields

        private readonly Dictionary<ColoredTexture, ColoredTextureWrapper> textures =
            new Dictionary<ColoredTexture, ColoredTextureWrapper>();

        #endregion

        #region Constructors and Destructors

        private ColoredTextures()
        {
            this.textures[ColoredTexture.Dragging] = this.BuildTexture(Resources.cursor_drag, 16, 16);
        }

        ~ColoredTextures()
        {
            foreach (var entry in this.textures.Where(entry => !entry.Value.Texture.IsDisposed))
            {
                entry.Value.Texture.Dispose();
            }
        }

        #endregion

        #region Public Indexers

        public ColoredTextureWrapper this[ColoredTexture textureType] => this.textures[textureType];

        #endregion

        #region Public Methods and Operators

        public ColoredTextureWrapper AddTexture(Image bmp, int width, int height, ColoredTexture textureType)
        {
            this.textures[textureType] = this.BuildTexture(bmp, height, width);
            return this.textures[textureType];
        }

        #endregion

        #region Methods

        private ColoredTextureWrapper BuildTexture(Image bmp, int height, int width)
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
            return new ColoredTextureWrapper(texture, width, height);
        }

        #endregion
    }

    internal class ColoredTextureWrapper
    {
        #region Constructors and Destructors

        public ColoredTextureWrapper(Texture texture, int width, int height)
        {
            this.Texture = texture;
            this.Width = width;
            this.Height = height;
        }

        #endregion

        #region Public Properties

        public int Height { get; private set; }

        public Texture Texture { get; private set; }

        public int Width { get; private set; }

        #endregion
    }
}