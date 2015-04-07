#region

using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    /// <summary>
    ///     Menu Bool.
    /// </summary>
    public class MenuBool : AMenuValue
    {
        /// <summary>
        ///     Constructor for MenuBool
        /// </summary>
        /// <param name="value">Bool Value</param>
        public MenuBool(bool value = false)
        {
            Value = value;
        }

        /// <summary>
        ///     Boolean Item Width requirement.
        /// </summary>
        public override int Width
        {
            get { return 0; }
        }

        /// <summary>
        ///     Bool Value.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        ///     Boolean Item Position.
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        ///     Boolean Item Draw callback.
        /// </summary>
        /// <param name="component">Parent Component</param>
        /// <param name="position">Position</param>
        /// <param name="index">Item Index</param>
        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            if (!Position.Equals(position))
            {
                Position = position;
            }

            SkinIndex.Skin[Configuration.GetValidMenuSkin()].OnBoolDraw(component, position, index);
        }

        /// <summary>
        ///     Boolean Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">WindowsKeys</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (args.Msg == WindowsMessages.LBUTTONDOWN && Position.IsValid())
            {
                var rect = SkinIndex.Skin[Configuration.GetValidMenuSkin()].GetBooleanContainerRectangle(Position);
                if (args.Cursor.IsUnderRectangle(rect.X, rect.Y, rect.Width, rect.Height))
                {
                    Value = !Value;
                }
            }
        }
    }
}