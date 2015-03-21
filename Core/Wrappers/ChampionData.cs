using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.CommonEx.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace LeagueSharp.CommonEx.Core.Wrappers
{
    /// <summary>
    ///     Provides information about champions.
    /// </summary>
    public class ChampionData
    {
        private readonly JToken championToken;

        /// <summary>
        ///     Creates a new instance, using the Player's Champion Name.
        /// </summary>
        public ChampionData() : this(ObjectManager.Player.ChampionName) {}

        /// <summary>
        ///     Creates a new instance, getting information about that champion.
        /// </summary>
        /// <param name="championName">Champion Name</param>
        public ChampionData(string championName)
        {
            var damageFile =
                JObject.Parse(
                    Encoding.UTF8.GetString(
                        Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream("LeagueSharp.CommonEx.ChampionData.json")
                            .GetAllBytes()));

            championToken = damageFile["data"][championName];

            if (championToken == null)
            {
                throw new ArgumentException("Champion does not exist.");
            }
        }

        /// <summary>
        ///     Gets the ID of the champion.
        /// </summary>
        /// <returns>ID</returns>
        public int GetId()
        {
            return championToken["id"].ToObject<int>();
        }

        /// <summary>
        ///     Gets the title of the champion.
        /// </summary>
        /// <example>"Ezreal" - "the Prodigal Explorer"</example>
        /// <returns></returns>
        public string GetTitle()
        {
            return championToken["title"].ToObject<string>();
        }

        /// <summary>
        ///     Gets a <see cref="JsonSpellWrapper" />, which contains hefty information about the spell.
        /// </summary>
        /// <param name="slot">Spell Slot. (Q - R)</param>
        /// <returns>
        ///     <see cref="JsonSpellWrapper" />
        /// </returns>
        public JsonSpellWrapper GetSpell(SpellSlot slot)
        {
            return new JsonSpellWrapper(championToken["spells"].Children().ToArray()[(int) slot]);
        }
    }

    /// <summary>
    ///     Wraps the "spells" section of the JSON file.
    /// </summary>
    public class JsonSpellWrapper
    {
        private readonly JToken spellToken;

        internal JsonSpellWrapper(JToken spellToken)
        {
            this.spellToken = spellToken;
        }

        /// <summary>
        ///     Gets the name of the spell.
        /// </summary>
        /// <returns>Name of the spell.</returns>
        public string GetName()
        {
            return spellToken["name"].ToObject<string>();
        }

        /// <summary>
        ///     Gets the description of the spell.
        /// </summary>
        /// <returns>Description</returns>
        public string GetDescription()
        {
            return spellToken["description"].ToObject<string>();
        }

        /// <summary>
        ///     Gets the sanitized description of the spell.
        /// </summary>
        /// <returns>Sanitized Description</returns>
        public string GetSanitizedDescription()
        {
            return spellToken["sanitizedDescription"].ToObject<string>();
        }

        /// <summary>
        ///     Gets the string showed when you hover over the ability.
        /// </summary>
        /// <returns>Tool tip</returns>
        public string GetToolTip()
        {
            return spellToken["tooltip"].ToObject<string>();
        }

        /// <summary>
        ///     Gets the sanitized string showed when you hover over the ability.
        /// </summary>
        /// <returns>Sanitized Tool Tip</returns>
        public string GetSanitizedToolTip()
        {
            return spellToken["sanitizedTooltip"].ToObject<string>();
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
                            spellToken["image"]["full"].ToObject<string>()));

            return new Bitmap(new MemoryStream(data));
        }

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
                        spellToken["image"]["full"].ToObject<string>())).Result;

            return new Bitmap(new MemoryStream(data));
        }

        /// <summary>
        ///     Gets the max rank which you can no longed upgrade the spell.
        /// </summary>
        /// <returns>Max Rank of the spell</returns>
        public int GetMaxRank()
        {
            return spellToken["maxrank"].ToObject<int>();
        }

        /// <summary>
        ///     Gets an array, containing the cost of the spell. The array matches the level of the spell.
        /// </summary>
        /// <returns>Int[] of spell clost.</returns>
        public int[] GetCost()
        {
            return spellToken["cost"].ToObject<int[]>();
        }

        /// <summary>
        ///     Gets the type of cost needed to cast the spell.
        /// </summary>
        /// <example>Mana</example>
        /// <returns>Type of cost</returns>
        public string GetCostType()
        {
            return spellToken["costType"].ToObject<string>();
        }

        /// <summary>
        ///     Gets the cost of the spell, with a '/' between each cost.
        /// </summary>
        /// <example>1/2/3/4/5</example>
        /// <returns>Cost of the spell as a string.</returns>
        public string GetCostString()
        {
            return spellToken["costBurn"].ToObject<string>();
        }

        /// <summary>
        ///     Gets the cooldown of the spell as an array. The array matches the level of the spell.
        /// </summary>
        /// <returns>Cooldown of the spell.</returns>
        public float[] GetCooldown()
        {
            return spellToken["cooldown"].ToObject<float[]>();
        }

        /// <summary>
        ///     Gets the cooldown of the spell, with a '/' between each cooldown.
        /// </summary>
        /// <example>1/2/3/4/5</example>
        /// <returns>Cooldown of the spell as a string.</returns>
        public string GetCooldownString()
        {
            return spellToken["cooldownBurn"].ToObject<string>();
        }

        /// <summary>
        ///     Gets the damage of the spell as an array. The array matches the level of the spell.
        /// </summary>
        /// <returns>Damage of the spell.</returns>
        public int[] GetDamage()
        {
            return spellToken["effect"].Children().ToArray()[1].ToObject<int[]>();
        }

        /// <summary>
        ///     Gets the damage of the spell, with a '/' between each damage.
        /// </summary>
        /// <example>1/2/3/4/5</example>
        /// <returns>Damage of the spell as a string.</returns>
        public string GetDamageString()
        {
            return spellToken["effectBurn"].Children().ToArray()[1].ToObject<string>();
        }

        /// <summary>
        ///     Gets the range of the spell as an array. The array matches the level of the spell.
        /// </summary>
        /// <returns>Range of the spell.</returns>
        public int[] GetRangeArray()
        {
            return spellToken["range"].ToObject<int[]>();
        }

        /// <summary>
        ///     Gets the range of the spell.
        /// </summary>
        /// <returns>Range</returns>
        public int GetRange()
        {
            return Convert.ToInt32(spellToken["rangeBurn"].ToObject<string>());
        }

        /// <summary>
        ///     Gets the name of the spell.
        /// </summary>
        /// <returns>Name of the spel.</returns>
        public string GetKey()
        {
            return spellToken["key"].ToObject<string>();
        }
    }
}