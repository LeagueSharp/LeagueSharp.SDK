// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChampionData.cs" company="LeagueSharp">
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
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using LeagueSharp.SDK.Properties;

    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     Provides information about champions.
    /// </summary>
    public class ChampionData
    {
        #region Fields

        /// <summary>
        ///     TODO The champion token.
        /// </summary>
        private readonly JToken championToken;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChampionData" /> class.
        /// </summary>
        public ChampionData()
            : this(ObjectManager.Player.ChampionName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChampionData" /> class.
        /// </summary>
        /// <param name="championName">
        ///     Champion Name
        /// </param>
        public ChampionData(string championName)
        {
            var damageFile = JObject.Parse(Encoding.UTF8.GetString(Resources.ChampionData));

            this.championToken = damageFile["data"][championName];

            if (this.championToken == null)
            {
                throw new ArgumentException("Champion does not exist.");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the ID of the champion.
        /// </summary>
        /// <value>ID of the champion</value>
        public int Id
        {
            get
            {
                return this.championToken["id"].ToObject<int>();
            }
        }

        /// <summary>
        ///     Gets the title of the champion.
        /// </summary>
        /// <example>"Ezreal" - "the Prodigal Explorer"</example>
        /// <value>Title of champion</value>
        public string Title
        {
            get
            {
                return this.championToken["title"].ToObject<string>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets a <see cref="JsonSpellWrapper" />, which contains hefty information about the spell.
        /// </summary>
        /// <param name="slot">Spell Slot. (Q - R)</param>
        /// <returns>
        ///     <see cref="JsonSpellWrapper" />
        /// </returns>
        public JsonSpellWrapper GetSpell(SpellSlot slot)
        {
            return new JsonSpellWrapper(this.championToken["spells"].Children().ToArray()[(int)slot]);
        }

        #endregion
    }

    /// <summary>
    ///     Wraps the "spells" section of the JSON file.
    /// </summary>
    public class JsonSpellWrapper
    {
        #region Fields

        /// <summary>
        ///     TODO The spell token.
        /// </summary>
        private readonly JToken spellToken;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonSpellWrapper" /> class.
        /// </summary>
        /// <param name="spellToken">
        ///     TODO The spell token.
        /// </param>
        internal JsonSpellWrapper(JToken spellToken)
        {
            this.spellToken = spellToken;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the cooldown of the spell as an array. The array matches the level of the spell.
        /// </summary>
        /// <value>Cooldown of the spell.</value>
        public float[] Cooldown
        {
            get
            {
                return this.spellToken["cooldown"].ToObject<float[]>();
            }
        }

        /// <summary>
        ///     Gets the cooldown of the spell, with a '/' between each cooldown.
        /// </summary>
        /// <example>1/2/3/4/5</example>
        /// <value>Cooldown of the spell as a string.</value>
        public string CooldownString
        {
            get
            {
                return this.spellToken["cooldownBurn"].ToObject<string>();
            }
        }

        /// <summary>
        ///     Gets an array, containing the cost of the spell. The array matches the level of the spell.
        /// </summary>
        /// <value>Int[] of spell clost.</value>
        public int[] Cost
        {
            get
            {
                return this.spellToken["cost"].ToObject<int[]>();
            }
        }

        /// <summary>
        ///     Gets the cost of the spell, with a '/' between each cost.
        /// </summary>
        /// <example>1/2/3/4/5</example>
        /// <value>Cost of the spell as a string.</value>
        public string CostString
        {
            get
            {
                return this.spellToken["costBurn"].ToObject<string>();
            }
        }

        /// <summary>
        ///     Gets the type of cost needed to cast the spell.
        /// </summary>
        /// <example>Mana</example>
        /// <value>Type of cost</value>
        public string CostType
        {
            get
            {
                return this.spellToken["costType"].ToObject<string>();
            }
        }

        /// <summary>
        ///     Gets the damage of the spell as an array. The array matches the level of the spell.
        /// </summary>
        /// <value>Damage of the spell.</value>
        public int[] Damage
        {
            get
            {
                return this.spellToken["effect"].Children().ToArray()[1].ToObject<int[]>();
            }
        }

        /// <summary>
        ///     Gets the damage of the spell, with a '/' between each damage.
        /// </summary>
        /// <example>1/2/3/4/5</example>
        /// <value>Damage of the spell as a string.</value>
        public string DamageString
        {
            get
            {
                return this.spellToken["effectBurn"].Children().ToArray()[1].ToObject<string>();
            }
        }

        /// <summary>
        ///     Gets the description of the spell.
        /// </summary>
        /// <value>Description</value>
        public string Description
        {
            get
            {
                return this.spellToken["description"].ToObject<string>();
            }
        }

        /// <summary>
        ///     Gets the name of the spell.
        /// </summary>
        /// <value>Name of the spel.</value>
        public string Key
        {
            get
            {
                return this.spellToken["key"].ToObject<string>();
            }
        }

        /// <summary>
        ///     Gets the max rank which you can no longed upgrade the spell.
        /// </summary>
        /// <value>Max Rank of the spell</value>
        public int MaxRank
        {
            get
            {
                return this.spellToken["maxrank"].ToObject<int>();
            }
        }

        /// <summary>
        ///     Gets the name of the spell.
        /// </summary>
        /// <value>Name of the spell.</value>
        public string Name
        {
            get
            {
                return this.spellToken["name"].ToObject<string>();
            }
        }

        /// <summary>
        ///     Gets the range of the spell.
        /// </summary>
        /// <value>Range</value>
        public int Range
        {
            get
            {
                return Convert.ToInt32(this.spellToken["rangeBurn"].ToObject<string>());
            }
        }

        /// <summary>
        ///     Gets the range of the spell as an array. The array matches the level of the spell.
        /// </summary>
        /// <value>Range of the spell.</value>
        public int[] RangeArray
        {
            get
            {
                return this.spellToken["range"].ToObject<int[]>();
            }
        }

        /// <summary>
        ///     Gets the sanitized description of the spell.
        /// </summary>
        /// <value>Sanitized Description</value>
        public string SanitizedDescription
        {
            get
            {
                return this.spellToken["sanitizedDescription"].ToObject<string>();
            }
        }

        /// <summary>
        ///     Gets the sanitized string showed when you hover over the ability.
        /// </summary>
        /// <value>Sanitized Tool Tip</value>
        public string SanitizedToolTip
        {
            get
            {
                return this.spellToken["sanitizedTooltip"].ToObject<string>();
            }
        }

        /// <summary>
        ///     Gets the string showed when you hover over the ability.
        /// </summary>
        /// <value>Tool tip</value>
        public string ToolTip
        {
            get
            {
                return this.spellToken["tooltip"].ToObject<string>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the image of the spell synchronous. (As displayed on the HUD in game)
        /// </summary>
        /// <returns>Image of the spell.</returns>
        public Image GetImage()
        {
            // Download the PNG
            var webClient = new HttpClient();
            var data =
                webClient.GetByteArrayAsync(
                    string.Format(
                        "http://ddragon.leagueoflegends.com/cdn/5.2.1/img/spell/{0}", 
                        this.spellToken["image"]["full"].ToObject<string>())).Result;

            return new Bitmap(new MemoryStream(data));
        }

        /// <summary>
        ///     Gets the image of the spell asynchronous. (As displayed on the HUD in game)
        /// </summary>
        /// <returns>Image of the spell.</returns>
        public async Task<Image> GetImageAsync()
        {
            // Download the PNG
            var webClient = new HttpClient();
            var data =
                await
                webClient.GetByteArrayAsync(
                    string.Format(
                        "http://ddragon.leagueoflegends.com/cdn/5.2.1/img/spell/{0}", 
                        this.spellToken["image"]["full"].ToObject<string>()));

            return new Bitmap(new MemoryStream(data));
        }

        #endregion
    }
}