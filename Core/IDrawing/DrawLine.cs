// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawLine.cs" company="LeagueSharp">
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
//   The DrawLine class, contains a set of tool to simply draw pre-defined lines and simple lines, both in 3D and
//   2D.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.IDrawing
{
    using System;

    using LeagueSharp.SDK.Core.Enumerations;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     The DrawLine class, contains a set of tool to simply draw pre-defined lines and simple lines, both in 3D and
    ///     2D.
    /// </summary>
    public class DrawLine : DrawObject, IDisposable
    {
        #region Fields

        /// <summary>
        ///     The Line.
        /// </summary>
        private Line line;

        /// <summary>
        ///     The texture sprite.
        /// </summary>
        private Sprite sprite;

        /// <summary>
        ///     Width local value.
        /// </summary>
        private int width;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawLine" /> class.
        /// </summary>
        /// <param name="position">
        ///     The position
        /// </param>
        public DrawLine(Vector2? position = null)
            : base(position ?? new Vector2())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawLine" /> class.
        /// </summary>
        /// <param name="position">
        ///     The position
        /// </param>
        /// <param name="positionTo">
        ///     The position to
        /// </param>
        /// <param name="width">
        ///     The width
        /// </param>
        public DrawLine(Vector2 position, Vector2 positionTo, int width = 1)
            : this(position)
        {
            this.PositionTo = positionTo;
            this.Width = width;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawLine" /> class.
        /// </summary>
        /// <param name="position">
        ///     The position
        /// </param>
        /// <param name="positionTo">
        ///     The position to
        /// </param>
        /// <param name="color">
        ///     The color
        /// </param>
        /// <param name="width">
        ///     The width
        /// </param>
        public DrawLine(Vector2 position, Vector2 positionTo, Color color, int width = 1)
            : this(position, positionTo, width)
        {
            this.Color = color;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawLine" /> class.
        /// </summary>
        /// <param name="unit">
        ///     The unit
        /// </param>
        /// <param name="positionTo">
        ///     The position to
        /// </param>
        /// <param name="width">
        ///     The width
        /// </param>
        public DrawLine(GameObject unit, Vector2 positionTo, int width = 1)
            : this(Vector2.Zero, positionTo, width)
        {
            this.GameObjectUnit = unit;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawLine" /> class.
        /// </summary>
        /// <param name="unit">
        ///     The unit
        /// </param>
        /// <param name="positionTo">
        ///     The position to
        /// </param>
        /// <param name="color">
        ///     The color
        /// </param>
        /// <param name="width">
        ///     The width
        /// </param>
        public DrawLine(GameObject unit, Vector2 positionTo, Color color, int width = 1)
            : this(Vector2.Zero, positionTo, color, width)
        {
            this.GameObjectUnit = unit;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawLine" /> class.
        /// </summary>
        /// <param name="position">
        ///     The position
        /// </param>
        /// <param name="unitTo">
        ///     The unit to
        /// </param>
        /// <param name="width">
        ///     The width
        /// </param>
        public DrawLine(Vector2 position, GameObject unitTo, int width = 1)
            : this(position, Vector2.Zero, width)
        {
            this.GameObjectUnitTo = unitTo;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawLine" /> class.
        /// </summary>
        /// <param name="position">
        ///     The position
        /// </param>
        /// <param name="unitTo">
        ///     The unit to
        /// </param>
        /// <param name="color">
        ///     The color
        /// </param>
        /// <param name="width">
        ///     The width
        /// </param>
        public DrawLine(Vector2 position, GameObject unitTo, Color color, int width = 1)
            : this(position, Vector2.Zero, color, width)
        {
            this.GameObjectUnitTo = unitTo;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawLine" /> class.
        /// </summary>
        /// <param name="unit">
        ///     The unit
        /// </param>
        /// <param name="unitTo">
        ///     The unit to
        /// </param>
        /// <param name="width">
        ///     The width
        /// </param>
        public DrawLine(GameObject unit, GameObject unitTo, int width = 1)
            : this(Vector2.Zero, Vector2.Zero, width)
        {
            this.GameObjectUnit = unit;
            this.GameObjectUnitTo = unitTo;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawLine" /> class.
        /// </summary>
        /// <param name="unit">
        ///     The unit
        /// </param>
        /// <param name="unitTo">
        ///     The unit to
        /// </param>
        /// <param name="color">
        ///     The color
        /// </param>
        /// <param name="width">
        ///     The width
        /// </param>
        public DrawLine(GameObject unit, GameObject unitTo, Color color, int width = 1)
            : this(unit, unitTo, width)
        {
            this.Color = color;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="DrawLine" /> class.
        /// </summary>
        ~DrawLine()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the game object unit from to extend the line.
        /// </summary>
        public GameObject GameObjectUnit { get; set; }

        /// <summary>
        ///     Gets or sets the game object unit to extend the line.
        /// </summary>
        public GameObject GameObjectUnitTo { get; set; }

        /// <summary>
        ///     Gets or sets the sprite.
        /// </summary>
        public Line Line
        {
            get
            {
                if (this.line != null && !this.line.IsDisposed)
                {
                    return this.line;
                }

                if (Drawing.Direct3DDevice != null && !Drawing.Direct3DDevice.IsDisposed)
                {
                    return this.line = new Line(Drawing.Direct3DDevice) { GLLines = true, Antialias = true, Width = 1f };
                }

                return null;
            }

            set
            {
                this.line = value;
            }
        }

        /// <summary>
        ///     Gets or sets the position to.
        /// </summary>
        public Vector2 PositionTo { get; set; }

        /// <summary>
        ///     Gets or sets the sprite.
        /// </summary>
        public Sprite Sprite
        {
            get
            {
                if (this.sprite != null && !this.sprite.IsDisposed)
                {
                    return this.sprite;
                }

                if (Drawing.Direct3DDevice != null && !Drawing.Direct3DDevice.IsDisposed)
                {
                    return this.sprite = new Sprite(Drawing.Direct3DDevice);
                }

                return null;
            }

            set
            {
                this.sprite = value;
            }
        }

        /// <summary>
        ///     Gets or sets the sprite flags.
        /// </summary>
        public SpriteFlags SpriteFlags { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.width = value;

                if (this.Line != null && !this.Line.IsDisposed)
                {
                    this.Line.Width = value;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     The drawing event call.
        /// </summary>
        /// <param name="drawType">
        ///     The draw Type.
        /// </param>
        public override void Draw(DrawType drawType)
        {
            if (this.Sprite != null && !this.Sprite.IsDisposed && this.Line != null && !this.Line.IsDisposed
                && this.IsVisible)
            {
                this.Sprite.Begin(this.SpriteFlags);

                this.Line.Begin();
                this.Line.Draw(
                    new[]
                        {
                            this.GameObjectUnit != null && this.GameObjectUnit.IsValid
                                ? Drawing.WorldToScreen(this.GameObjectUnit.Position)
                                : this.Position, 
                            this.GameObjectUnitTo != null && this.GameObjectUnitTo.IsValid
                                ? Drawing.WorldToScreen(this.GameObjectUnitTo.Position)
                                : this.PositionTo
                        }, 
                    this.Color);
                this.Line.End();

                this.Sprite.Flush();
                this.Sprite.End();
            }

            this.CallOnDraw();
        }

        /// <summary>
        ///     The OnLostDevice event.
        /// </summary>
        public override void OnPostReset()
        {
            if (this.Sprite != null && !this.Sprite.IsDisposed)
            {
                this.Sprite.OnResetDevice();
            }

            if (this.Line != null && !this.Line.IsDisposed)
            {
                this.Line.OnResetDevice();
            }
        }

        /// <summary>
        ///     The OnPreReset event.
        /// </summary>
        public override void OnPreReset()
        {
            if (this.Sprite != null && !this.Sprite.IsDisposed)
            {
                this.Sprite.OnLostDevice();
            }

            if (this.Line != null && !this.Line.IsDisposed)
            {
                this.Line.OnLostDevice();
            }
        }

        /// <summary>
        ///     The OnUpdate event.
        /// </summary>
        public override void Update()
        {
            this.CallOnUpdate();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged and managed
        ///     resources.
        /// </summary>
        /// <param name="type">
        ///     The type of free, release or reset of resources.
        /// </param>
        private void Dispose(bool type)
        {
            if (type)
            {
                if (this.Sprite != null && !this.Sprite.IsDisposed)
                {
                    this.Sprite.Dispose();
                    this.Sprite = null;
                }

                if (this.Line != null && !this.Line.IsDisposed)
                {
                    this.Line.Dispose();
                    this.Line = null;
                }
            }
        }

        #endregion
    }
}