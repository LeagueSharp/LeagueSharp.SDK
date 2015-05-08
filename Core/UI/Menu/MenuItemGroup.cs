using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Values;

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     An Item Group.
    /// </summary>
    public class MenuItemGroup
    {
        private readonly List<MenuItem> Items = new List<MenuItem>();

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(MenuItem item)
        {
            if (item.ValueAsObject is MenuBool)
            {
                Items.Add(item);
                ((AMenuValue) item.ValueAsObject).ValueChanged += (sender, args) =>
                {
                    if (args.GetValue<MenuBool>().Value)
                    {
                        DisableAll(((AMenuValue) item.ValueAsObject));
                    }
                };
            }
            else
            {
                Console.WriteLine("{0} not implemented into MenuGroup", item);
            }
        }

        /// <summary>
        /// Disables all.
        /// </summary>
        /// <param name="except">The except.</param>
        public void DisableAll(AMenuValue except)
        {
            var i = Items.Where(s => s.ValueAsObject as AMenuValue != except);

            foreach (var abs in i.Select(x => x.ValueAsObject as AMenuValue))
            {
                if (abs is MenuBool)
                {
                    (abs as MenuBool).Value = false;
                }
                else if (abs is MenuKeyBind)
                {
                    (abs as MenuKeyBind).Active = false;
                }
            }
        }
    }
}