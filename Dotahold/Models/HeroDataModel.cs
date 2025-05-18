using System;
using System.Collections.Generic;
using System.Text;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;

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
        public HeroTalentModel Talents { get; private set; }

        /// <summary>
        /// 命石列表
        /// </summary>
        public List<HeroFacetModel> Facets { get; private set; } = [];

        public HeroDataModel(DotaHeroDataModel heroData)
        {
            this.DotaHeroData = heroData;

            // Innate

            if (this.DotaHeroData.abilities is not null && this.DotaHeroData.abilities.Length > 0)
            {
                foreach (var ability in this.DotaHeroData.abilities)
                {
                    if (ability.ability_is_innate)
                    {
                        this.Innate = new HeroInnateModel(ability);
                        break;
                    }
                }
            }

            this.Innate ??= new HeroInnateModel();

            // Talents

            if (this.DotaHeroData.talents is not null && this.DotaHeroData.talents.Length > 0)
            {
                this.Talents = new HeroTalentModel(this.DotaHeroData.talents, this.DotaHeroData.abilities);
            }

            this.Talents ??= new HeroTalentModel();

            // Facets

            if (this.DotaHeroData.facets is not null)
            {
                foreach (var facet in this.DotaHeroData.facets)
                {
                    this.Facets.Add(new HeroFacetModel(facet));
                }
            }
        }
    }

    public class HeroInnateModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public HeroInnateModel(AbilityData abilityData)
        {
            this.Name = abilityData.name_loc;
            this.Description = StringFormatter.FormatAbilitySpecialValues(StringFormatter.FormatPlainText(abilityData.desc_loc), abilityData.special_values);
        }

        public HeroInnateModel()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
        }
    }

    public class HeroTalentModel
    {
        public string TalentNameLeftLevel10 { get; set; } = string.Empty;

        public string TalentNameRightLevel10 { get; set; } = string.Empty;

        public string TalentNameLeftLevel15 { get; set; } = string.Empty;

        public string TalentNameRightLevel15 { get; set; } = string.Empty;

        public string TalentNameLeftLevel20 { get; set; } = string.Empty;

        public string TalentNameRightLevel20 { get; set; } = string.Empty;

        public string TalentNameLeftLevel25 { get; set; } = string.Empty;

        public string TalentNameRightLevel25 { get; set; } = string.Empty;

        public HeroTalentModel(AbilityData[] talents, AbilityData[]? abilities)
        {
            foreach (var talent in talents)
            {
                talent.name_loc = StringFormatter.FormatTalentSpecialValues(StringFormatter.FormatPlainText(talent.name_loc), talent.name, talent.special_values, abilities);
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

        public HeroTalentModel()
        {
            this.TalentNameLeftLevel10 = string.Empty;
            this.TalentNameRightLevel10 = string.Empty;
            this.TalentNameLeftLevel15 = string.Empty;
            this.TalentNameRightLevel15 = string.Empty;
            this.TalentNameLeftLevel20 = string.Empty;
            this.TalentNameRightLevel20 = string.Empty;
            this.TalentNameLeftLevel25 = string.Empty;
            this.TalentNameRightLevel25 = string.Empty;
        }
    }

    public class HeroFacetModel(FacetData facetData)
    {
        public AsyncImage IconImage { get; private set; } = new AsyncImage($"{ConstantsCourier.ImageSourceDomain}/apps/dota2/images/dota_react/icons/facets/{facetData.icon}.png", 0, 72);
    }

    public static partial class StringFormatter
    {
        [System.Text.RegularExpressions.GeneratedRegex("<.*?>")]
        private static partial System.Text.RegularExpressions.Regex RegexHTMLTags();

        [System.Text.RegularExpressions.GeneratedRegex("&[^;]+;")]
        private static partial System.Text.RegularExpressions.Regex RegexSpecialCharacters();

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
                if (!string.IsNullOrWhiteSpace(originalString) && specialValues is not null)
                {
                    foreach (var specialValue in specialValues)
                    {
                        string value = "0";

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
                    }
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"FormatAbilitySpecialValues Error: {ex.Message}", LogCourier.LogType.Error);
            }

            return result;
        }

        public static string FormatTalentSpecialValues(string originalString, string talentName, SpecialValueData[]? specialValues, AbilityData[]? abilities)
        {
            string result = originalString;

            try
            {
                if (!string.IsNullOrWhiteSpace(originalString))
                {
                    if (specialValues is not null)
                    {
                        foreach (var specialValue in specialValues)
                        {
                            string value = "0";

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
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"FormatTalentSpecialValues Error: {ex.Message}", LogCourier.LogType.Error);
            }

            return result;
        }
    }
}
