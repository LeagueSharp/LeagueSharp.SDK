using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI
{
    using System.ComponentModel.Design;

    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Values;

    /// <summary>
    /// 
    /// </summary>
    public class RadioMenu : Menu
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Menu" /> class.
        ///     Menu Constructor.
        /// </summary>
        /// <param name="name">
        ///     Menu Name
        /// </param>
        /// <param name="displayName">
        ///     Menu Display Name
        /// </param>
        /// <param name="root">
        ///     Root component
        /// </param>
        /// <param name="uniqueString">
        ///     Unique string
        /// </param>
        public RadioMenu(string name, string displayName, bool root = false, string uniqueString = "")
            : base(name, displayName, root, uniqueString)
        {
            MenuValueChanged += RadioMenuMenuValueChanged;
        }

        private void RadioMenuMenuValueChanged(object sender, OnMenuValueChangedEventArgs args)
        {
            try
            {
                MenuItem<MenuBool> menuBool = args.MenuItem as MenuItem<MenuBool>;

                if (menuBool != null && menuBool.Value.Value)
                {
                    foreach (var comp in Components)
                    {
                        MenuItem<MenuBool> child = comp.Value as MenuItem<MenuBool>;
                        if (child != null && child.Name != menuBool.Name)
                        {
                            child.Value.Value = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
