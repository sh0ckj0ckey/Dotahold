using System;
using System.Collections.Generic;
using System.Text;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class HeroDataModel
    {
        public DotaHeroDataModel DotaHeroData { get; private set; }

        /// <summary>
        /// 先天技能
        /// </summary>
        public HeroInnateModel Innate { get; private set; }

        /// <summary>
        /// 天赋树
        /// </summary>
        public HeroTalentsModel Talents { get; private set; }

        /// <summary>
        /// 命石列表
        /// </summary>
        public List<HeroFacetModel> Facets { get; private set; } = [];

        public HeroDataModel(DotaHeroDataModel heroData)
        {
            this.DotaHeroData = heroData;

            // Innate
            this.Innate = new HeroInnateModel(this.DotaHeroData.abilities);

            // Talents
            this.Talents = new HeroTalentsModel(this.DotaHeroData.talents, this.DotaHeroData.abilities, this.DotaHeroData.facet_abilities);

            // Facets
            if (this.DotaHeroData.facets is not null)
            {
                for (int i = 0; i < this.DotaHeroData.facets.Length; i++)
                {
                    var facet = this.DotaHeroData.facets[i];
                    var facetAbility = this.DotaHeroData.facet_abilities?.Length > i ? this.DotaHeroData.facet_abilities[i] : null;
                    this.Facets.Add(new HeroFacetModel(facet, this.DotaHeroData.talents, this.DotaHeroData.abilities, facetAbility));
                }
            }
        }
    }

    public class HeroInnateModel
    {
        public string Name { get; private set; } = string.Empty;

        public string Description { get; private set; } = string.Empty;

        public HeroInnateModel(AbilityData[]? abilities)
        {
            if (abilities is not null && abilities.Length > 0)
            {
                foreach (var ability in abilities)
                {
                    if (ability.ability_is_innate)
                    {
                        this.Name = ability.name_loc;
                        this.Description = StringFormatter.FormatPlainText(StringFormatter.FormatAbilitySpecialValues(ability.desc_loc, ability.special_values));
                        break;
                    }
                }
            }
        }
    }

    public class HeroTalentsModel
    {
        public string TalentNameLeftLevel10 { get; private set; } = string.Empty;

        public string TalentNameRightLevel10 { get; private set; } = string.Empty;

        public string TalentNameLeftLevel15 { get; private set; } = string.Empty;

        public string TalentNameRightLevel15 { get; private set; } = string.Empty;

        public string TalentNameLeftLevel20 { get; private set; } = string.Empty;

        public string TalentNameRightLevel20 { get; private set; } = string.Empty;

        public string TalentNameLeftLevel25 { get; private set; } = string.Empty;

        public string TalentNameRightLevel25 { get; private set; } = string.Empty;

        public HeroTalentsModel(AbilityData[]? talents, AbilityData[]? abilities, FacetAbilityData[]? facetAbilities)
        {
            if (talents is not null && talents.Length > 0)
            {
                foreach (var talent in talents)
                {
                    talent.name_loc = StringFormatter.FormatPlainText(StringFormatter.FormatTalentSpecialValues(talent.name_loc, talent.name, talent.special_values, abilities, facetAbilities));
                }

                this.TalentNameLeftLevel10 = talents.Length > 0 ? (talents[0]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameRightLevel10 = talents.Length > 1 ? (talents[1]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameLeftLevel15 = talents.Length > 2 ? (talents[2]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameRightLevel15 = talents.Length > 3 ? (talents[3]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameLeftLevel20 = talents.Length > 4 ? (talents[4]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameRightLevel20 = talents.Length > 5 ? (talents[5]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameLeftLevel25 = talents.Length > 6 ? (talents[6]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameRightLevel25 = talents.Length > 7 ? (talents[7]?.name_loc ?? string.Empty) : string.Empty;
            }
        }
    }

    public class HeroFacetModel
    {
        /// <summary>
        /// 默认玩家头像图片
        /// </summary>
        public static BitmapImage? DefaultFacetImageSource72 = null;

        public AsyncImage IconImage { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string Description { get; private set; } = string.Empty;

        public HeroFacetModel(FacetData facetData, AbilityData[]? talents, AbilityData[]? abilities, FacetAbilityData? facetAbility)
        {
            DefaultFacetImageSource72 ??= new BitmapImage(new Uri("ms-appx:///Assets/icon_dota2.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 72,
            };

            this.IconImage = new AsyncImage($"{ConstantsCourier.ImageSourceDomain}/apps/dota2/images/dota_react/icons/facets/{facetData.icon}.png", 0, 72, DefaultFacetImageSource72);
            this.Name = facetData.title_loc;
            this.Description = StringFormatter.FormatPlainText(StringFormatter.FormatFacetSpecialValues(facetData.description_loc, facetData, talents, abilities, facetAbility));
        }
    }

    public static partial class StringFormatter
    {
        [System.Text.RegularExpressions.GeneratedRegex("<.*?>")]
        private static partial System.Text.RegularExpressions.Regex RegexHTMLTags();

        [System.Text.RegularExpressions.GeneratedRegex("&[^;]+;")]
        private static partial System.Text.RegularExpressions.Regex RegexSpecialCharacters();


        [System.Text.RegularExpressions.GeneratedRegex(@"\{s:[^}]+\}")]
        private static partial System.Text.RegularExpressions.Regex RegexPlaceholder();

        public static string FormatPlainText(string originalString)
        {
            string result = originalString;

            try
            {
                if (!string.IsNullOrWhiteSpace(originalString))
                {
                    result = RegexHTMLTags().Replace(originalString, string.Empty);
                    result = RegexSpecialCharacters().Replace(result, string.Empty);

                    result = result.Replace("\t", "")
                                   .Replace("\r", "\n")
                                   .TrimStart('\n')
                                   .TrimEnd('\n');
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"FormatString Error: {ex.Message}", LogCourier.LogType.Error);
            }

            return result;
        }

        public static string FormatAbilitySpecialValues(string originalString, SpecialValueData[]? specialValues)
        {
            string result = originalString;

            try
            {
                if (!string.IsNullOrWhiteSpace(result) && specialValues is not null)
                {
                    foreach (var specialValue in specialValues)
                    {
                        string value = "?";

                        if (specialValue.values_float is not null && specialValue.values_float.Length > 0)
                        {
                            StringBuilder valueStringBuider = new();

                            for (int i = 0; i < specialValue.values_float.Length; i++)
                            {
                                valueStringBuider.Append(Math.Floor(100 * specialValue.values_float[i]) / 100);
                                if (i < specialValue.values_float.Length - 1)
                                {
                                    valueStringBuider.Append('/');
                                }
                            }

                            value = valueStringBuider.ToString();
                        }

                        result = result.Replace($"%{specialValue.name}%", value)
                                       .Replace($"%{specialValue.name.ToLower()}%", value);
                        result = result.Replace("%%", "%");

                        result = RegexPlaceholder().Replace(result, "?");
                    }
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"FormatAbilitySpecialValues Error: {ex.Message}", LogCourier.LogType.Error);
            }

            return result;
        }

        public static string FormatTalentSpecialValues(string originalString, string talentName, SpecialValueData[]? specialValues, AbilityData[]? abilities, FacetAbilityData[]? facetAbilities)
        {
            string result = originalString;

            try
            {
                if (!string.IsNullOrWhiteSpace(result))
                {
                    if (specialValues is not null)
                    {
                        foreach (var specialValue in specialValues)
                        {
                            string value = "?";

                            if (specialValue.values_float is not null && specialValue.values_float.Length > 0)
                            {
                                StringBuilder valueStringBuider = new();

                                for (int i = 0; i < specialValue.values_float.Length; i++)
                                {
                                    valueStringBuider.Append(Math.Floor(100 * specialValue.values_float[i]) / 100);
                                    if (i < specialValue.values_float.Length - 1)
                                    {
                                        valueStringBuider.Append('/');
                                    }
                                }

                                value = valueStringBuider.ToString();
                            }

                            result = result.Replace($"{{s:{specialValue.name}}}", value)
                                           .Replace($"{{s:{specialValue.name.ToLower()}}}", value);
                        }
                    }

                    if (abilities is not null)
                    {
                        foreach (var ability in abilities)
                        {
                            if (ability.special_values is not null)
                            {
                                foreach (var specialValue in ability.special_values)
                                {
                                    if (specialValue.bonuses is not null)
                                    {
                                        foreach (var bonus in specialValue.bonuses)
                                        {
                                            if (bonus.name == talentName)
                                            {
                                                string value = (Math.Floor(100 * bonus.value) / 100).ToString();
                                                result = result.Replace($"{{s:bonus_{specialValue.name}}}", value)
                                                               .Replace($"{{s:bonus_{specialValue.name.ToLower()}}}", value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (facetAbilities is not null)
                    {
                        foreach (var facetAbility in facetAbilities)
                        {
                            if (facetAbility.abilities is not null)
                            {
                                foreach (var ability in facetAbility.abilities)
                                {
                                    if (ability.special_values is not null)
                                    {
                                        foreach (var specialValue in ability.special_values)
                                        {
                                            if (specialValue.bonuses is not null)
                                            {
                                                foreach (var bonus in specialValue.bonuses)
                                                {
                                                    if (bonus.name == talentName)
                                                    {
                                                        string value = (Math.Floor(100 * bonus.value) / 100).ToString();
                                                        result = result.Replace($"{{s:bonus_{specialValue.name}}}", value)
                                                                       .Replace($"{{s:bonus_{specialValue.name.ToLower()}}}", value);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    result = result.Replace("%%", "%");

                    result = RegexPlaceholder().Replace(result, "?");
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"FormatTalentSpecialValues Error: {ex.Message}", LogCourier.LogType.Error);
            }

            return result;
        }

        public static string FormatFacetSpecialValues(string originalString, FacetData facet, AbilityData[]? talents, AbilityData[]? abilities, FacetAbilityData? facetAbility)
        {
            string result = originalString;

            try
            {
                if (!string.IsNullOrWhiteSpace(result))
                {
                    if (talents is not null)
                    {
                        foreach (var talent in talents)
                        {
                            if (talent.special_values is not null)
                            {
                                foreach (var specialValue in talent.special_values)
                                {
                                    if (specialValue.facet_bonus is not null)
                                    {
                                        if (specialValue.facet_bonus.name == facet.name)
                                        {
                                            if (specialValue.facet_bonus.values is not null && specialValue.facet_bonus.values.Length > 0)
                                            {
                                                StringBuilder valueStringBuider = new();

                                                for (int i = 0; i < specialValue.facet_bonus.values.Length; i++)
                                                {
                                                    valueStringBuider.Append(Math.Floor(100 * specialValue.facet_bonus.values[i]) / 100);
                                                    if (i < specialValue.facet_bonus.values.Length - 1)
                                                    {
                                                        valueStringBuider.Append('/');
                                                    }
                                                }

                                                string value = valueStringBuider.ToString();

                                                result = result.Replace($"{{s:bonus_{specialValue.name}}}", value)
                                                               .Replace($"{{s:bonus_{specialValue.name.ToLower()}}}", value)
                                                               .Replace($"%bonus_{specialValue.name}%", value)
                                                               .Replace($"%bonus_{specialValue.name.ToLower()}%", value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (abilities is not null)
                    {
                        foreach (var ability in abilities)
                        {
                            if (ability.special_values is not null)
                            {
                                foreach (var specialValue in ability.special_values)
                                {
                                    if (specialValue.facet_bonus is not null)
                                    {
                                        if (specialValue.facet_bonus.name == facet.name)
                                        {
                                            if (specialValue.facet_bonus.values is not null && specialValue.facet_bonus.values.Length > 0)
                                            {
                                                StringBuilder valueStringBuider = new();

                                                for (int i = 0; i < specialValue.facet_bonus.values.Length; i++)
                                                {
                                                    valueStringBuider.Append(Math.Floor(100 * specialValue.facet_bonus.values[i]) / 100);
                                                    if (i < specialValue.facet_bonus.values.Length - 1)
                                                    {
                                                        valueStringBuider.Append('/');
                                                    }
                                                }

                                                string value = valueStringBuider.ToString();

                                                result = result.Replace($"{{s:bonus_{specialValue.name}}}", value)
                                                               .Replace($"{{s:bonus_{specialValue.name.ToLower()}}}", value)
                                                               .Replace($"%bonus_{specialValue.name}%", value)
                                                               .Replace($"%bonus_{specialValue.name.ToLower()}%", value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (facetAbility is not null)
                    {
                        if (facetAbility.abilities is not null)
                        {
                            foreach (var ability in facetAbility.abilities)
                            {
                                if (ability.special_values is not null)
                                {
                                    foreach (var specialValue in ability.special_values)
                                    {
                                        if (specialValue.facet_bonus is not null)
                                        {
                                            if (specialValue.facet_bonus.name == facet.name)
                                            {
                                                if (specialValue.facet_bonus.values is not null && specialValue.facet_bonus.values.Length > 0)
                                                {
                                                    StringBuilder valueStringBuider = new();

                                                    for (int i = 0; i < specialValue.facet_bonus.values.Length; i++)
                                                    {
                                                        valueStringBuider.Append(Math.Floor(100 * specialValue.facet_bonus.values[i]) / 100);
                                                        if (i < specialValue.facet_bonus.values.Length - 1)
                                                        {
                                                            valueStringBuider.Append('/');
                                                        }
                                                    }

                                                    string value = valueStringBuider.ToString();

                                                    result = result.Replace($"{{s:bonus_{specialValue.name}}}", value)
                                                                   .Replace($"{{s:bonus_{specialValue.name.ToLower()}}}", value)
                                                                   .Replace($"%bonus_{specialValue.name}%", value)
                                                                   .Replace($"%bonus_{specialValue.name.ToLower()}%", value);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (facetAbility is not null)
                    {
                        if (facetAbility.abilities is not null)
                        {
                            foreach (var ability in facetAbility.abilities)
                            {
                                if (ability.special_values is not null)
                                {
                                    foreach (var specialValue in ability.special_values)
                                    {
                                        if (specialValue.values_float is not null && specialValue.values_float.Length > 0)
                                        {
                                            StringBuilder valueStringBuider = new();

                                            for (int i = 0; i < specialValue.values_float.Length; i++)
                                            {
                                                valueStringBuider.Append(Math.Floor(100 * specialValue.values_float[i]) / 100);
                                                if (i < specialValue.values_float.Length - 1)
                                                {
                                                    valueStringBuider.Append('/');
                                                }
                                            }

                                            string value = valueStringBuider.ToString();

                                            result = result.Replace($"{{s:bonus_{specialValue.name}}}", value)
                                                           .Replace($"{{s:bonus_{specialValue.name.ToLower()}}}", value)
                                                           .Replace($"%bonus_{specialValue.name}%", value)
                                                           .Replace($"%bonus_{specialValue.name.ToLower()}%", value)
                                                           .Replace($"{{s:{specialValue.name}}}", value)
                                                           .Replace($"{{s:{specialValue.name.ToLower()}}}", value)
                                                           .Replace($"%{specialValue.name}%", value)
                                                           .Replace($"%{specialValue.name.ToLower()}%", value);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    result = result.Replace("%facet_ability_name%", facet.title_loc)
                                   .Replace("{s:facet_ability_name}", facet.title_loc)
                                   .Replace("%%", "%");

                    result = RegexPlaceholder().Replace(result, "?");
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"FormatFacetSpecialValues Error: {ex.Message}", LogCourier.LogType.Error);
            }

            return result;
        }
    }
}
