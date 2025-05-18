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
        /// 命石列表
        /// </summary>
        public List<HeroFacetModel> Facets { get; private set; } = new List<HeroFacetModel>();

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
            this.Description = StringFormatter.FormatSpecialValues(StringFormatter.FormatPlainText(abilityData.desc_loc), abilityData.special_values);
        }

        public HeroInnateModel()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
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
            catch (System.Exception ex)
            {
                LogCourier.Log($"FormatString Error: {ex.Message}", LogCourier.LogType.Error);
            }

            return result;
        }

        public static string FormatSpecialValues(string originalString, SpecialValueData[]? specialValues)
        {
            string result = originalString;

            try
            {
                if (!string.IsNullOrWhiteSpace(originalString) && specialValues is not null)
                {
                    foreach (var specialValue in specialValues)
                    {
                        string value = "0";

                        if (specialValue.values_float is not null && specialValue.values_float?.Length > 0)
                        {
                            StringBuilder valueStringBuider = new();

                            for (int i = 0; i < specialValue.values_float.Length; i++)
                            {
                                valueStringBuider.Append(specialValue.values_float[i].ToString("f1"));
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
            catch (System.Exception ex)
            {
                LogCourier.Log($"FormatSpecialValues Error: {ex.Message}", LogCourier.LogType.Error);
            }

            return result;
        }
    }
}
