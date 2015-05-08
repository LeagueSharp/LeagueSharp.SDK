using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    /// <summary>
    ///     A menu seperator.
    /// </summary>
    public class MenuSeparator : AMenuValue
    {
        /// <summary>
        /// Value Width.
        /// </summary>
        public override int Width
        {
            get { return 0; }
        }

        /// <summary>
        /// Menu Value Position.
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        /// Drawing callback.
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

            var animation = ThemeManager.Current.Boolean.Animation;

            if (animation != null && animation.IsAnimating())
            {
                animation.OnDraw(component, position, index);

                return;
            }
            ThemeManager.Current.Separator.OnDraw(component, position, index);
        }

        /// <summary>
        /// Windows Process Messages callback.
        /// </summary>
        /// <param name="args"></param>
        public override void OnWndProc(WindowsKeys args)
        {
            // not needed                        
        }

        /// <summary>
        /// Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public override void Extract(AMenuValue component) {}
    }
}