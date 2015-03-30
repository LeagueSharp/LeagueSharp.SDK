using System.Collections.Generic;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Math;
using LeagueSharp.CommonEx.Properties;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu Skins handler (mostly for sprites)
    /// </summary>
    public class MenuSkins
    {
        /// <summary>
        ///     Loads a selected skin
        /// </summary>
        /// <param name="skinId">Skin Id</param>
        /// <param name="sprite">Sprite for measurement</param>
        /// <param name="position">Position</param>
        /// <param name="sprites">Sprites output list</param>
        public static void Load(int skinId,
            Sprite sprite,
            Vector2 position,
            out IDictionary<string, MenuSprite> sprites)
        {
            #region SKIN #0

            sprites = new Dictionary<string, MenuSprite>();

            if (skinId == 0)
            {
                var rect = new Rectangle(
                    (int) position.X, (int) position.Y, (int) MenuInterface.MainMenu.Width, Drawing.Height);
                var center = rect.GetCenter(
                    sprite,
                    new Rectangle(
                        (int) position.X, (int) position.Y, Resources.skin0_img001.Width, Resources.skin0_img001.Height),
                    CenteredFlags.HorizontalCenter);

                sprites.Add(
                    "Header_Logo",
                    new MenuSprite(Resources.skin0_img000) { Position = new Vector2(position.X, position.Y + 5f) });
                sprites.Add(
                    "Container_LeagueSharp",
                    new MenuSprite(Resources.skin0_img001)
                    {
                        Position = new Vector2(center.X, GetSeperatorVertices(sprites["Header_Logo"])[0].Y + 20f),
                    }.AddGlow(Resources.skin0_img001_sub));
                sprites.Add(
                    "Container_Assemblies",
                    new MenuSprite(Resources.skin0_img002)
                    {
                        Position =
                            new Vector2(center.X, GetSeperatorVertices(sprites["Container_LeagueSharp"])[0].Y + 80f)
                    }
                        .AddGlow(Resources.skin0_img002_sub));
                sprites.Add(
                    "Container_Settings",
                    new MenuSprite(Resources.skin0_img003)
                    {
                        Position =
                            new Vector2(center.X, GetSeperatorVertices(sprites["Container_Assemblies"])[0].Y + 80f)
                    }
                        .AddGlow(Resources.skin0_img003_sub));

                center = rect.GetCenter(
                    sprite,
                    new Rectangle(
                        (int) position.X, (int) position.Y, Resources.skin0_img004.Width, Resources.skin0_img004.Height),
                    CenteredFlags.HorizontalCenter);

                sprites.Add(
                    "Footer_Logo",
                    new MenuSprite(Resources.skin0_img004)
                    {
                        Position = new Vector2(center.X, Drawing.Height - 170f)
                    });

                center = rect.GetCenter(
                    sprite,
                    new Rectangle(
                        (int)position.X, (int)position.Y, Resources.special_F3VD2VG.Width, Resources.special_F3VD2VG.Height),
                    CenteredFlags.HorizontalCenter);
                sprites.Add(
                    "Special_F3VD2VG",
                    new MenuSprite(Resources.special_F3VD2VG)
                    {
                        Position = new Vector2(center.X, Drawing.Height - 200f)
                    });
            }

            #endregion
        }

        /// <summary>
        ///     Gets the seperator position vertices
        /// </summary>
        /// <param name="sprite">Sprite</param>
        /// <param name="useLocalWidth">Should use local width</param>
        /// <param name="flip">Should flip locaiton</param>
        /// <param name="extraF">Extra Distance</param>
        /// <returns>Vertices</returns>
        public static Vector2[] GetSeperatorVertices(MenuSprite sprite,
            bool useLocalWidth = false,
            bool flip = false,
            float extraF = 10f)
        {
            return new[]
            {
                new Vector2(
                    (!useLocalWidth) ? sprite.Position.X : MenuInterface.Position.X,
                    sprite.Position.Y + ((!flip) ? sprite.Height + extraF : -extraF)),
                new Vector2(
                    (!useLocalWidth)
                        ? sprite.Position.X + sprite.Width
                        : MenuInterface.Position.X + MenuInterface.MainMenu.Width + 1,
                    sprite.Position.Y + ((!flip) ? sprite.Height + extraF : -extraF))
            };
        }
    }
}