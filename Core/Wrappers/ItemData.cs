// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemData.cs" company="LeagueSharp">
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
//   Provides information about champions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Wrappers
{
    using System;
    using System.Text;

    using LeagueSharp.SDK.Properties;

    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     Provides information about champions.
    /// </summary>
    public class ItemData
    {
        #region Fields

        /// <summary>
        ///     Effect token used to parse the effect wrapper
        /// </summary>
        private readonly JToken effectToken;

        /// <summary>
        ///     Gold token used to parse the gold wrapper
        /// </summary>
        private readonly JToken goldToken;

        /// <summary>
        ///     Item token used to parse the item wrapper
        /// </summary>
        private readonly JToken itemToken;

        /// <summary>
        ///     Stats token used to parse the stats wrapper
        /// </summary>
        private readonly JToken statsToken;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ItemData" /> class.
        ///     Creates a new instance, getting information about that item.
        /// </summary>
        /// <param name="itemId">
        ///     Item Id
        /// </param>
        public ItemData(int itemId)
        {
            var damageFile = JObject.Parse(Encoding.UTF8.GetString(Resources.ChampionData));

            this.itemToken = damageFile["data"][itemId];
            this.goldToken = damageFile["data"][itemId]["gold"];
            this.statsToken = damageFile["data"][itemId]["stats"];
            this.effectToken = damageFile["data"][itemId]["effect"];

            if (this.itemToken == null || this.statsToken == null || this.goldToken == null || this.effectToken == null)
            {
                throw new ArgumentException("Item not found!");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the base price.
        /// </summary>
        public int BasePrice
        {
            get
            {
                return this.goldToken["base"].ToObject<int>();
            }
        }

        /// <summary>
        ///     Gets the from.
        /// </summary>
        public int[] From
        {
            get
            {
                return this.itemToken["from"].ToObject<int[]>();
            }
        }

        /// <summary>
        ///     Gets the Id of the Item
        /// </summary>
        public int Id
        {
            get
            {
                return (int)this.itemToken;
            }
        }

        /// <summary>
        ///     Gets the into.
        /// </summary>
        public int[] Into
        {
            get
            {
                return this.itemToken["into"].ToObject<int[]>();
            }
        }

        /// <summary>
        ///     Gets the Name of the Item
        /// </summary>
        public string Name
        {
            get
            {
                return this.itemToken["name"].ToObject<string>();
            }
        }

        /// <summary>
        ///     Gets the plaintext Description of the Item
        /// </summary>
        public string PlaintextDescription
        {
            get
            {
                return this.itemToken["plaintext"].ToObject<string>();
            }
        }

        /// <summary>
        ///     Gets a value indicating whether purchaseable.
        /// </summary>
        public bool Purchaseable
        {
            get
            {
                return this.goldToken["purchaseable"].ToObject<bool>();
            }
        }

        /// <summary>
        ///     Gets the sell price.
        /// </summary>
        public int SellPrice
        {
            get
            {
                return this.goldToken["sell"].ToObject<int>();
            }
        }

        /// <summary>
        ///     Gets the stacks.
        /// </summary>
        public int Stacks
        {
            get
            {
                return this.itemToken["stacks"].ToObject<int>();
            }
        }

        /// <summary>
        ///     Gets the tags.
        /// </summary>
        public string[] Tags
        {
            get
            {
                return this.itemToken["tags"].ToObject<string[]>();
            }
        }

        /// <summary>
        ///     Gets the total price.
        /// </summary>
        public int TotalPrice
        {
            get
            {
                return this.goldToken["total"].ToObject<int>();
            }
        }

        #endregion
    }
}