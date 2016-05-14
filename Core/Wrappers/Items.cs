// <copyright file="Items.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDKEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    /// <summary>
    ///     Item class used to easily manage items.
    /// </summary>
    public static class Items
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns true if the player has the item and its not on cool-down.
        /// </summary>
        /// <param name="name">
        ///     Name of the Item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CanUseItem(string name)
        {
            return
                GameObjects.Player.InventoryItems.Where(slot => slot.IData.DisplayName == name)
                    .Select(
                        slot =>
                        GameObjects.Player.Spellbook.Spells.FirstOrDefault(
                            spell => (int)spell.Slot == slot.Slot + (int)SpellSlot.Item1))
                    .Select(inst => inst != null && inst.State == SpellState.Ready)
                    .FirstOrDefault();
        }

        /// <summary>
        ///     Returns true if the player has the item and its not on cool-down.
        /// </summary>
        /// <param name="id">
        ///     Id of the Item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CanUseItem(int id)
        {
            return
                GameObjects.Player.InventoryItems.Where(slot => slot.Id == (ItemId)id)
                    .Select(
                        slot =>
                        GameObjects.Player.Spellbook.Spells.FirstOrDefault(
                            spell => (int)spell.Slot == slot.Slot + (int)SpellSlot.Item1))
                    .Select(inst => inst != null && inst.State == SpellState.Ready)
                    .FirstOrDefault();
        }

        /// <summary>
        ///     Returns the ward slot.
        /// </summary>
        /// <returns>
        ///     The <see cref="InventorySlot" />.
        /// </returns>
        public static InventorySlot GetWardSlot()
        {
            var wardIds = new[] { 2049, 2045, 2301, 2302, 2303, 3711, 1408, 1409, 1410, 1411, 3932, 3340, 2043 };
            return (from wardId in wardIds
                    where CanUseItem(wardId)
                    select GameObjects.Player.InventoryItems.FirstOrDefault(slot => slot.Id == (ItemId)wardId))
                .FirstOrDefault();
        }

        /// <summary>
        ///     Returns true if the hero has the item.
        /// </summary>
        /// <param name="name">
        ///     Name of the Item.
        /// </param>
        /// <param name="hero">
        ///     Hero to be checked.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasItem(string name, Obj_AI_Hero hero = null)
        {
            return (hero ?? GameObjects.Player).InventoryItems.Any(slot => slot.IData.DisplayName == name);
        }

        /// <summary>
        ///     Returns true if the hero has the item.
        /// </summary>
        /// <param name="id">
        ///     Id of the Item.
        /// </param>
        /// <param name="hero">
        ///     Hero to be checked.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasItem(int id, Obj_AI_Hero hero = null)
        {
            return (hero ?? GameObjects.Player).InventoryItems.Any(slot => slot.Id == (ItemId)id);
        }

        /// <summary>
        ///     Casts the item on the target.
        /// </summary>
        /// <param name="name">
        ///     Name of the Item.
        /// </param>
        /// <param name="target">
        ///     Target to be hit.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool UseItem(string name, Obj_AI_Base target = null)
        {
            return
                GameObjects.Player.InventoryItems.Where(slot => slot.IData.DisplayName == name)
                    .Select(
                        slot =>
                        target != null
                            ? GameObjects.Player.Spellbook.CastSpell(slot.SpellSlot, target)
                            : GameObjects.Player.Spellbook.CastSpell(slot.SpellSlot))
                    .FirstOrDefault();
        }

        /// <summary>
        ///     Casts the item on the target.
        /// </summary>
        /// <param name="id">
        ///     Id of the Item.
        /// </param>
        /// <param name="target">
        ///     Target to be hit.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool UseItem(int id, Obj_AI_Base target = null)
        {
            return
                GameObjects.Player.InventoryItems.Where(slot => slot.Id == (ItemId)id)
                    .Select(
                        slot =>
                        target != null
                            ? GameObjects.Player.Spellbook.CastSpell(slot.SpellSlot, target)
                            : GameObjects.Player.Spellbook.CastSpell(slot.SpellSlot))
                    .FirstOrDefault();
        }

        /// <summary>
        ///     Casts the item on a Vector2 position.
        /// </summary>
        /// <param name="id">
        ///     Id of the Item.
        /// </param>
        /// <param name="position">
        ///     Position of the Item cast.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool UseItem(int id, Vector2 position)
        {
            return UseItem(id, position.ToVector3());
        }

        /// <summary>
        ///     Casts the item on a Vector3 position.
        /// </summary>
        /// <param name="id">
        ///     Id of the Item.
        /// </param>
        /// <param name="position">
        ///     Position of the Item cast.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool UseItem(int id, Vector3 position)
        {
            return position.IsValid()
                   && GameObjects.Player.InventoryItems.Where(slot => slot.Id == (ItemId)id)
                          .Select(slot => GameObjects.Player.Spellbook.CastSpell(slot.SpellSlot, position))
                          .FirstOrDefault();
        }

        #endregion

        /// <summary>
        ///     Item class.
        /// </summary>
        public class Item
        {
            #region Fields

            /// <summary>
            ///     Range of the Item
            /// </summary>
            private float range;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Item" /> class.
            /// </summary>
            /// <param name="id">
            ///     The Id
            /// </param>
            /// <param name="range">
            ///     The Range
            /// </param>
            public Item(int id, float range)
            {
                var item = ItemData.Entries.FirstOrDefault(i => (int)i.Id == id);

                if (item == null)
                {
                    throw new MissingMemberException($"Unable to find item with the id {id}");
                }

                this.Id = (int)item.Id;
                this.Name = item.DisplayName;
                this.Range = range;
                this.BasePrice = item.Price;
                this.SellPrice = (int)(item.Price * item.SellBackModifier);
                this.Purchaseable = item.CanBeSold;
                this.Stacks = item.MaxStack;
                this.HideFromAll = !item.UsableInStore;
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Item" /> class.
            /// </summary>
            /// <param name="id">The item identifier.</param>
            /// <param name="range">The range.</param>
            /// <exception cref="MissingMemberException">Thrown when we were unable to find the item with same id.</exception>
            public Item(ItemId id, float range)
            {
                var item = ItemData.Entries.FirstOrDefault(x => x.Id == id);

                if (item == null)
                {
                    throw new MissingMemberException($"Unable to find item with the id {id}");
                }

                this.Id = (int)item.Id;
                this.Name = item.DisplayName;
                this.Range = range;
                this.BasePrice = item.Price;
                this.SellPrice = (int)(item.Price * item.SellBackModifier);
                this.Purchaseable = item.CanBeSold;
                this.Stacks = item.MaxStack;
                this.HideFromAll = !item.UsableInStore;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the base price.
            /// </summary>
            public int BasePrice { get; private set; }

            /// <summary>
            ///     Gets a value indicating whether hide from all.
            /// </summary>
            public bool HideFromAll { get; private set; }

            /// <summary>
            ///     Gets the Id of the Item.
            /// </summary>
            public int Id { get; }

            /// <summary>
            ///     Gets a value indicating whether is ready.
            /// </summary>
            public bool IsReady => CanUseItem(this.Id);

            /// <summary>
            ///     Gets the Name of the Item
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            ///     Gets a value indicating whether purchase-able.
            /// </summary>
            public bool Purchaseable { get; private set; }

            /// <summary>
            ///     Gets or sets the range.
            /// </summary>
            public float Range
            {
                get
                {
                    return this.range;
                }

                set
                {
                    this.range = value;
                    this.RangeSqr = value * value;
                }
            }

            /// <summary>
            ///     Gets the range squared.
            /// </summary>
            public float RangeSqr { get; private set; }

            /// <summary>
            ///     Gets the sell price.
            /// </summary>
            public int SellPrice { get; private set; }

            /// <summary>
            ///     Gets the Slot of the Item
            /// </summary>
            public List<SpellSlot> Slot
            {
                get
                {
                    return
                        GameObjects.Player.InventoryItems.Where(slot => slot.Id == (ItemId)this.Id)
                            .Select(slot => slot.SpellSlot)
                            .ToList();
                }
            }

            /// <summary>
            ///     Gets the maximum stacks.
            /// </summary>
            public int Stacks { get; private set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Buys the Items.
            /// </summary>
            public void Buy()
            {
                GameObjects.Player.BuyItem((ItemId)this.Id);
            }

            /// <summary>
            ///     Casts the Item.
            /// </summary>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public bool Cast()
            {
                return UseItem(this.Id);
            }

            /// <summary>
            ///     Casts the Item on a Target.
            /// </summary>
            /// <param name="target">
            ///     Target as <c>Obj_AI_Base</c>.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public bool Cast(Obj_AI_Base target)
            {
                return UseItem(this.Id, target);
            }

            /// <summary>
            ///     Casts the Item on a Position.
            /// </summary>
            /// <param name="position">
            ///     Position as Vector2.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public bool Cast(Vector2 position)
            {
                return UseItem(this.Id, position);
            }

            /// <summary>
            ///     Casts the Item on a Position.
            /// </summary>
            /// <param name="position">
            ///     Position as Vector3.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public bool Cast(Vector3 position)
            {
                return UseItem(this.Id, position);
            }

            /// <summary>
            ///     Returns if the target is in the range of the Item.
            /// </summary>
            /// <param name="target">
            ///     Target to be checked.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />
            /// </returns>
            public bool IsInRange(Obj_AI_Base target)
            {
                return this.IsInRange(target.ServerPosition);
            }

            /// <summary>
            ///     Returns if the position is in the range of the Item.
            /// </summary>
            /// <param name="position">
            ///     Position to be checked.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />
            /// </returns>
            public bool IsInRange(Vector2 position)
            {
                return this.IsInRange(position.ToVector3());
            }

            /// <summary>
            ///     Returns if the position is in the range of the Item.
            /// </summary>
            /// <param name="position">
            ///     Position to be checked.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />
            /// </returns>
            public bool IsInRange(Vector3 position)
            {
                return GameObjects.Player.DistanceSquared(position) < this.RangeSqr;
            }

            /// <summary>
            ///     Returns if the Item is owned.
            /// </summary>
            /// <param name="target">
            ///     Target as <c>Obj_AI_Hero</c>.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public bool IsOwned(Obj_AI_Hero target = null)
            {
                return HasItem(this.Id, target);
            }

            #endregion
        }
    }
}