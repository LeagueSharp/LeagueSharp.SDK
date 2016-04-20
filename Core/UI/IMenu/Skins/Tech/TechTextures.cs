// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechTextures.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.UI.Skins.Tech
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp.SDK.Properties;

    using SharpDX.Direct3D9;

    internal enum TechTexture
    {
        Dragging,
    }

    internal class TechTextures
    {
        #region Static Fields

        public static readonly TechTextures Instance = new TechTextures();

        #endregion

        #region Fields

        private readonly Dictionary<TechTexture, TechTextureWrapper> textures =
            new Dictionary<TechTexture, TechTextureWrapper>();

        #endregion

        #region Constructors and Destructors

        private TechTextures()
        {
            this.textures[TechTexture.Dragging] = this.BuildTexture(Resources.cursor_drag, 16, 16);
        }

        ~TechTextures()
        {
            foreach (var entry in this.textures.Where(entry => !entry.Value.Texture.IsDisposed))
            {
                entry.Value.Texture.Dispose();
            }
        }

        #endregion

        #region Public Indexers

        public TechTextureWrapper this[TechTexture textureType]
        {
            get
            {
                return this.textures[textureType];
            }
        }

        #endregion

        #region Public Methods and Operators

        public TechTextureWrapper AddTexture(Image bmp, int width, int height, TechTexture textureType)
        {
            this.textures[textureType] = this.BuildTexture(bmp, height, width);
            return this.textures[textureType];
        }

        #endregion

        #region Methods

        private TechTextureWrapper BuildTexture(Image bmp, int height, int width)
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
            return new TechTextureWrapper(texture, width, height);
        }

        #endregion
    }

    internal class TechTextureWrapper
    {
        #region Constructors and Destructors

        public TechTextureWrapper(Texture texture, int width, int height)
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