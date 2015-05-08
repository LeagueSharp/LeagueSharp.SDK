using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Values;

namespace LeagueSharp.CommonEx.Core.UI
{
    public class MenuItemGroup
    {
        private readonly List<MenuItem> Items = new List<MenuItem>();

        public void Add(MenuItem item)
        {
            if (item.ValueAsObject is MenuBool || item.ValueAsObject is MenuKeyBind)
            {
                Items.Add(item);
                ((AMenuValue) item.ValueAsObject).ValueChanged += args =>
                {
                    if (args.GetNewValue() is bool && ((bool) (args.GetNewValue())))
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

        public void Remove(MenuItem item)
        {
            ((AMenuValue) item.ValueAsObject).ValueChanged -= args =>
            {
                if (args.GetNewValue() is bool && ((bool) (args.GetNewValue())))
                {
                    DisableAll(((AMenuValue) item.ValueAsObject));
                }
            };
            Items.Remove(item);
        }

        public void DisableAll(AMenuValue except)
        {
            IEnumerable<MenuItem> i = Items.Where(s => s.ValueAsObject as AMenuValue != except);

            foreach (MenuItem _i in i)
            {
                var abs = _i.ValueAsObject as AMenuValue;

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