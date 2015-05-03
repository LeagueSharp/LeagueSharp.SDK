#region

using System;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

#endregion

namespace LeagueSharp.CommonEx.Core.Render
{
    /// <summary>
    ///     Draws formatted text. These methods support ANSI and Unicode strings.
    /// </summary>
    /// <remarks>
    ///     The parameters of this method are very similar to those of the GDI DrawText function.
    ///     This method supports both ANSI and Unicode strings.
    ///     This method must be called inside a BeginScene ... EndScene block. The only exception is when an application calls
    ///     DrawText with DT_CALCRECT to calculate the size of a given block of text.
    ///     Unless the FontDrawFlags.NoClip format is used, this method clips the text so that it does not appear outside the
    ///     specified
    ///     rectangle. All formatting is assumed to have multiple lines unless the FontDrawFlags.SingleLine format is
    ///     specified.
    ///     If the selected font is too large for the rectangle, this method does not attempt to substitute a smaller font.
    ///     This method supports only fonts whose escapement and orientation are both zero.
    /// </remarks>
    public sealed class Text : IDisposable
    {
        #region Private Fields

        private readonly Font font;

        #endregion

        /// <summary>
        ///     Text dispose.
        /// </summary>
        public void Dispose()
        {
            font.Dispose();
        }

        #region Class Internal Drawing

        /// <summary>
        ///     Draws formatted text. This method supports ANSI and Unicode strings.
        /// </summary>
        /// <param name="text">String to draw.</param>
        /// <param name="vector2">
        ///     Vector2 structure that contains the X-Axis and Y-Axis to draw the given text on the screen
        ///     projector.
        /// </param>
        /// <param name="color">Color of the text.</param>
        /// <returns>
        ///     If the function succeeds, the return value is the height of the text in logical units. If the function fails,
        ///     the return value is zero.
        /// </returns>
        public int Draw(string text, Vector2 vector2, ColorBGRA color)
        {
            return font.DrawText(null, text, (int) vector2.X, (int) vector2.Y, color);
        }

        #endregion

        #region Constructors / Overloads

        /// <summary>
        ///     Initializes a new instance of the <see cref="Text" /> class.
        /// </summary>
        /// <param name="font">
        ///     Font interface encapsulates the textures and resources needed to render a specific font on a
        ///     specific device.
        /// </param>
        public Text(Font font)
        {
            this.font = font;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Text" /> class.
        /// </summary>
        /// <param name="font">
        ///     Font interface encapsulates the textures and resources needed to render a specific font on a
        ///     specific device, from the <see cref="System.Drawing.Font" /> class.
        /// </param>
        public Text(System.Drawing.Font font)
        {
            this.font = new Font(Drawing.Direct3DDevice, font);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Text" /> class.
        /// </summary>
        /// <param name="fontDescription">
        ///     Defines font attributes for a Font interface which encapsulates the textures and resources needed to render a
        ///     specific font on a
        ///     specific device.
        /// </param>
        public Text(FontDescription fontDescription)
        {
            font = new Font(Drawing.Direct3DDevice, fontDescription);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Text" /> class.
        /// </summary>
        public Text()
        {
            font = Constants.LeagueSharpFont;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Text" /> class.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="mipLevels">The mip levels.</param>
        /// <param name="isItalic">if set to <c>true</c> [is italic].</param>
        /// <param name="characterSet">The character set.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="quality">The quality.</param>
        /// <param name="pitchAndFamily">The pitch and family.</param>
        /// <param name="faceName">Name of the face.</param>
        public Text(int height,
            int width,
            FontWeight weight,
            int mipLevels,
            bool isItalic,
            FontCharacterSet characterSet,
            FontPrecision precision,
            FontQuality quality,
            FontPitchAndFamily pitchAndFamily,
            string faceName)
        {
            font = new Font(
                Drawing.Direct3DDevice, height, width, weight, mipLevels, isItalic, characterSet, precision, quality,
                pitchAndFamily, faceName);
        }

        #endregion

        #region External Drawing

        #region DrawText :: Font, Sprite, Text, Vector2, Color

        /// <summary>
        ///     Draws formatted text. This method supports ANSI and Unicode strings.
        /// </summary>
        /// <param name="font">
        ///     Font interface encapsulates the textures and resources needed to render a specific font on a
        ///     specific device.
        /// </param>
        /// <param name="sprite">
        ///     Sprite interface provides a set of methods that simplify the process of drawing sprites using
        ///     Microsoft Direct3D.
        /// </param>
        /// <param name="text">String to draw.</param>
        /// <param name="vector2">
        ///     Vector2 structure that contains the X-Axis and Y-Axis to draw the given text on the screen
        ///     projector.
        /// </param>
        /// <param name="color">Color of the text.</param>
        /// <returns>
        ///     If the function succeeds, the return value is the height of the text in logical units. If the function fails,
        ///     the return value is zero.
        /// </returns>
        public static int DrawText(Font font, Sprite sprite, string text, Vector2 vector2, ColorBGRA color)
        {
            return font.DrawText(sprite, text, (int) vector2.X, (int) vector2.Y, color);
        }

        /// <summary>
        ///     Draws formatted text. This method supports ANSI and Unicode strings.
        /// </summary>
        /// <param name="font">
        ///     Font interface encapsulates the textures and resources needed to render a specific font on a
        ///     specific device.
        /// </param>
        /// <param name="sprite">
        ///     Sprite interface provides a set of methods that simplify the process of drawing sprites using
        ///     Microsoft Direct3D.
        /// </param>
        /// <param name="text">String to draw.</param>
        /// <param name="vector2">
        ///     Vector2 structure that contains the X-Axis and Y-Axis to draw the given text on the screen
        ///     projector.
        /// </param>
        /// <param name="color">Color of the text.</param>
        /// <returns></returns>
        public static int DrawText(Font font, Sprite sprite, string text, Vector2 vector2, Color color)
        {
            return font.DrawText(
                sprite, text, (int) vector2.X, (int) vector2.Y, new ColorBGRA(color.R, color.G, color.B, color.A));
        }

        #endregion

        #region DrawText :: Font, Sprite, Text, Int, Int, Color

        /// <summary>
        ///     Draws formatted text. This method supports ANSI and Unicode strings.
        /// </summary>
        /// <param name="font">
        ///     Font interface encapsulates the textures and resources needed to render a specific font on a
        ///     specific device.
        /// </param>
        /// <param name="sprite">
        ///     Sprite interface provides a set of methods that simplify the process of drawing sprites using
        ///     Microsoft Direct3D.
        /// </param>
        /// <param name="text">String to draw.</param>
        /// <param name="x">X-Axis on the screen projector to draw on.</param>
        /// <param name="y">Y-Axis on the screen projector to draw on.</param>
        /// <param name="color">Color of the text.</param>
        /// <returns></returns>
        public static int DrawText(Font font, Sprite sprite, string text, int x, int y, Color color)
        {
            return font.DrawText(sprite, text, x, y, new ColorBGRA(color.R, color.G, color.B, color.A));
        }

        #endregion

        #region DrawText :: Font, Sprite, String, Int, Rectangle, Int, Color

        /// <summary>
        ///     Draws formatted text. This method supports ANSI and Unicode strings.
        /// </summary>
        /// <param name="font">
        ///     Font interface encapsulates the textures and resources needed to render a specific font on a
        ///     specific device.
        /// </param>
        /// <param name="spriteRef">
        ///     Pointer to an Sprite object that contains the string. Can be null, in which case Direct3D will
        ///     render the string with its own sprite object. To improve efficiency, a sprite object should be specified if
        ///     DrawText is to be called more than once in a row.
        /// </param>
        /// <param name="stringRef">Pointer to a string to draw. If the Count parameter is -1, the string must be null-terminated.</param>
        /// <param name="count">
        ///     Specifies the number of characters in the string. If Count is -1, then the string parameter is
        ///     assumed to be a reference to a null-terminated string and DrawText computes the character count automatically.
        /// </param>
        /// <param name="rectRef">
        ///     Pointer to a Rectangle structure that contains the rectangle, in logical coordinates, in which
        ///     the text is to be formatted. The coordinate value of the rectangle's right side must be greater than that of its
        ///     left side. Likewise, the coordinate value of the bottom must be greater than that of the top.
        /// </param>
        /// <param name="format">Specifies the method of formatting the text.</param>
        /// <param name="color">Color of the text.</param>
        /// <returns>
        ///     If the function succeeds, the return value is the height of the text in logical units. If
        ///     FontDrawFlags.VerticalCenter or FontDrawFlags.Bottom is specified, the return value is the offset from pRect (top
        ///     to the bottom) of the drawn text. If the function fails, the return value is zero.
        /// </returns>
        public static int DrawText(Font font,
            Sprite spriteRef,
            string stringRef,
            int count,
            IntPtr rectRef,
            int format,
            Color color)
        {
            return font.DrawText(
                spriteRef, stringRef, count, rectRef, format, new ColorBGRA(color.R, color.G, color.B, color.A));
        }

        #endregion

        #region DrawText :: Font, Sprite, String, Rectangle, FontDrawFlags, Color

        /// <summary>
        ///     Draws formatted text. This method supports ANSI and Unicode strings.
        /// </summary>
        /// <param name="font">
        ///     Font interface encapsulates the textures and resources needed to render a specific font on a
        ///     specific device.
        /// </param>
        /// <param name="sprite">
        ///     Sprite interface provides a set of methods that simplify the process of drawing sprites using
        ///     Microsoft Direct3D.
        /// </param>
        /// <param name="text">String to draw.</param>
        /// <param name="rect">
        ///     Rectangle structure that contains the rectangle, in logical coordinates, in which
        ///     the text is to be formatted. The coordinate value of the rectangle's right side must be greater than that of its
        ///     left side. Likewise, the coordinate value of the bottom must be greater than that of the top.
        /// </param>
        /// <param name="drawFlags">Specifies the method of formatting the text.</param>
        /// <param name="color">Color of the text.</param>
        /// <returns>
        ///     If the function succeeds, the return value is the height of the text in logical units. If
        ///     FontDrawFlags.VerticalCenter or FontDrawFlags.Bottom is specified, the return value is the offset from pRect (top
        ///     to the bottom) of the drawn text. If the function fails, the return value is zero.
        /// </returns>
        public static int DrawText(Font font,
            Sprite sprite,
            string text,
            Rectangle rect,
            FontDrawFlags drawFlags,
            Color color)
        {
            return font.DrawText(sprite, text, rect, drawFlags, new ColorBGRA(color.R, color.G, color.B, color.A));
        }

        #endregion

        #endregion
    }
}