#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.UI.Skins.Default;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu User Interface.
    /// </summary>
    public class Menu : AMenuComponent
    {
        /// <summary>
        ///     Menu Component Sub-Components.
        /// </summary>
        public readonly IDictionary<string, AMenuComponent> Components = new Dictionary<string, AMenuComponent>();

        private bool _toggled;

        /// <summary>
        ///     Menu Constructor.
        /// </summary>
        /// <param name="name">Menu Name</param>
        /// <param name="displayName">Menu Display Name</param>
        /// <param name="uniqueString">Unique string</param>
        public Menu(string name, string displayName, string uniqueString = "") : base(name, displayName, uniqueString) {}

        /// <summary>
        ///     Component Sub Object accessability.
        /// </summary>
        /// <param name="name">Child Menu Component name</param>
        /// <returns>Child Menu Component of this component.</returns>
        public override AMenuComponent this[string name]
        {
            get { return Components.ContainsKey(name) ? Components[name] : null; }
        }

        /// <summary>
        ///     Returns the menu visiblity.
        /// </summary>
        public override sealed bool Visible { get; set; }

        public bool Hovering { get; private set; }

        public bool SharedSettings { get; set; }

        /// <summary>
        ///     Returns if the menu has been toggled.
        /// </summary>
        public override sealed bool Toggled
        {
            get { return _toggled; }
            set
            {
                _toggled = value;
                //Hide children when untoggled
                foreach (var comp in Components)
                {
                    comp.Value.Visible = value;
                    if (!_toggled)
                    {
                        comp.Value.Toggled = false;
                    }
                }
            }
        }

        /// <summary>
        ///     Menu Position
        /// </summary>
        public override Vector2 Position { get; set; }

        public override int Width
        {
            get { return ThemeManager.Current.CalcWidthMenu(this); }
        }

        public override string Path
        {
            get
            {
                if (SharedSettings)
                {
                    return MenuManager.ConfigFolder.CreateSubdirectory("SharedConfig").FullName;
                }
                if (Parent == null)
                {
                    return
                        MenuManager.ConfigFolder.CreateSubdirectory(AssemblyName)
                            .CreateSubdirectory(Name + UniqueString)
                            .FullName;
                }
                return Directory.CreateDirectory(System.IO.Path.Combine(Parent.Path, Name + UniqueString)).FullName;
            }
        }

        /// <summary>
        ///     Add a menu component to this menu.
        /// </summary>
        /// <param name="component"><see cref="AMenuComponent" /> component</param>
        public void Add(AMenuComponent component)
        {
            if (!Components.ContainsKey(component.Name))
            {
                component.Parent = this;
                Components[component.Name] = component;
            }
            else
            {
                throw new Exception("This menu already contains a component with the name " + component.Name);
            }
        }

        /// <summary>
        ///     Removes a menu component from this menu.
        /// </summary>
        /// <param name="component"><see cref="AMenuComponent" /> component instance</param>
        public void Remove(AMenuComponent component)
        {
            if (Components.ContainsKey(component.Name))
            {
                component.Parent = null;
                Components.Remove(component.Name);
            }
        }

        public override T GetValue<T>(string name)
        {
            if (Components.ContainsKey(name))
            {
                return ((MenuItem<T>) Components[name]).Value;
            }
            throw new Exception("Could not find child with name " + name);
        }

        public override T GetValue<T>()
        {
            throw new Exception("Cannot get the Value of a Menu");
        }

        /// <summary>
        ///     Menu Drawing callback.
        /// </summary>
        public override void OnDraw(Vector2 position, int index)
        {
            Position = position;

            ThemeManager.Current.OnMenu(this, position, index);
        }

        /// <summary>
        ///     Menu Windows Process Messages callback.
        /// </summary>
        /// <param name="args"></param>
        public override void OnWndProc(WindowsKeys args)
        {
            if ((MenuManager.Instance.MenuVisible && Parent == null) || Visible)
            {
                if (args.Cursor.IsUnderRectangle(Position.X, Position.Y, MenuWidth, DefaultSettings.ContainerHeight))
                {
                    Hovering = true;
                    if (args.Msg == WindowsMessages.LBUTTONDOWN && Components.Count > 0)
                    {
                        Toggled = !Toggled;

                        //Toggling siblings logic
                        if (Parent == null)
                        {
                            foreach (Menu rootComponent in MenuManager.Instance.Menus.Where(c => !c.Equals(this)))
                            {
                                rootComponent.Toggled = false;
                            }
                        }
                        else
                        {
                            foreach (var comp in Parent.Components.Where(comp => comp.Value.Name != Name))
                            {
                                comp.Value.Toggled = false;
                            }
                        }

                        return;
                    }
                }
                else
                {
                    Hovering = false;
                }

                //Pass OnWndProc on to children
                if (Toggled)
                {
                    foreach (var item in Components.Where(c => c.Value.Visible))
                    {
                        item.Value.OnWndProc(args);
                    }
                }
            }
        }

        /// <summary>
        ///     Menu Update callback.
        /// </summary>
        public override void OnUpdate() {}


        public override void Save()
        {
            foreach (var comp in Components)
            {
                comp.Value.Save();
            }
        }

        public override void Load()
        {
            foreach (var comp in Components)
            {
                comp.Value.Load();
            }
        }

        /// <summary>
        ///     Attaches the menu towards the main menu.
        /// </summary>
        /// <returns>Menu Instance</returns>
        public Menu Attach()
        {
            if (Parent == null)
            {
                AssemblyName = Assembly.GetCallingAssembly().GetName().Name;
                MenuManager.Instance.Add(this);
            }
            else
            {
                throw new Exception("You should not add a Menu that already has a parent.");
            }
            return this;
        }
    }
}