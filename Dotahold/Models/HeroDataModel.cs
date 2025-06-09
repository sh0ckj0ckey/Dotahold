using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;
using Dotahold.Helpers;
using Windows.UI.Xaml.Media;
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

        /// <summary>
        /// 技能列表
        /// </summary>
        public List<HeroAbilityModel> Abilities { get; private set; } = [];

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
                    this.Facets.Add(new HeroFacetModel(this.DotaHeroData.facets[i], this.DotaHeroData.talents, this.DotaHeroData.abilities, (this.DotaHeroData.facet_abilities?.Length > i ? this.DotaHeroData.facet_abilities[i] : null)));
                }
            }

            // Abilities
            if (this.DotaHeroData.abilities is not null)
            {
                for (int i = 0; i < this.DotaHeroData.abilities.Length; i++)
                {
                    this.Abilities.Add(new HeroAbilityModel(this.DotaHeroData.abilities[i], this.Facets));
                }
            }

            // Facet Abilities
            if (this.DotaHeroData.facet_abilities is not null)
            {
                for (int i = 0; i < this.DotaHeroData.facet_abilities.Length; i++)
                {
                    DotaHeroFacetAbility facetAbility = this.DotaHeroData.facet_abilities[i];

                    if (facetAbility.abilities is not null)
                    {
                        foreach (var ability in facetAbility.abilities)
                        {
                            this.Abilities.Add(new HeroAbilityModel(ability, this.Facets, this.Facets.Count > i ? this.Facets[i] : null));
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 命石先天技能
    /// </summary>
    public class HeroInnateModel
    {
        /// <summary>
        /// 先天技能名称
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// 先天技能描述
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        public HeroInnateModel(DotaHeroAbility[]? abilities)
        {
            if (abilities is not null && abilities.Length > 0)
            {
                foreach (var ability in abilities)
                {
                    if (ability.ability_is_innate)
                    {
                        this.Name = ability.name_loc;
                        this.Description = AbilityStringFormatter.FormatPlainText(AbilityStringFormatter.FormatAbilitySpecialValues(ability.desc_loc, ability.special_values));
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 英雄天赋树
    /// </summary>
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

        public HeroTalentsModel(DotaHeroAbility[]? talents, DotaHeroAbility[]? abilities, DotaHeroFacetAbility[]? facetAbilities)
        {
            if (talents is not null && talents.Length > 0)
            {
                foreach (var talent in talents)
                {
                    talent.name_loc = AbilityStringFormatter.FormatPlainText(AbilityStringFormatter.FormatTalentSpecialValues(talent.name_loc, talent.name, talent.special_values, abilities, facetAbilities));
                }

                this.TalentNameRightLevel10 = talents.Length > 0 ? (talents[0]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameLeftLevel10 = talents.Length > 1 ? (talents[1]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameRightLevel15 = talents.Length > 2 ? (talents[2]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameLeftLevel15 = talents.Length > 3 ? (talents[3]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameRightLevel20 = talents.Length > 4 ? (talents[4]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameLeftLevel20 = talents.Length > 5 ? (talents[5]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameRightLevel25 = talents.Length > 6 ? (talents[6]?.name_loc ?? string.Empty) : string.Empty;
                this.TalentNameLeftLevel25 = talents.Length > 7 ? (talents[7]?.name_loc ?? string.Empty) : string.Empty;
            }
        }
    }

    /// <summary>
    /// 英雄命石
    /// </summary>
    public class HeroFacetModel
    {
        /// <summary>
        /// 默认命石图标
        /// </summary>
        private static BitmapImage? _defaultFacetImageSource72 = null;

        /// <summary>
        /// 命石图标
        /// </summary>
        public AsyncImage IconImage { get; private set; }

        /// <summary>
        /// 命石名称
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// 命石描述
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// 命石背景色
        /// </summary>
        public LinearGradientBrush BackgroundBrush { get; private set; }

        /// <summary>
        /// 命石索引
        /// </summary>
        public int Index { get; private set; } = -1;

        public HeroFacetModel(DotaHeroFacet facetData, DotaHeroAbility[]? talents, DotaHeroAbility[]? abilities, DotaHeroFacetAbility? facetAbility)
        {
            _defaultFacetImageSource72 ??= new BitmapImage(new Uri("ms-appx:///Assets/icon_dota2.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 72,
            };

            this.IconImage = new AsyncImage($"{ConstantsCourier.ImageSourceDomain}/apps/dota2/images/dota_react/icons/facets/{facetData.icon}.png", 0, 72, _defaultFacetImageSource72);
            this.Name = facetData.title_loc;
            this.Description = AbilityStringFormatter.FormatPlainText(AbilityStringFormatter.FormatFacetSpecialValues(facetData.description_loc, facetData, talents, abilities, facetAbility));
            this.BackgroundBrush = ColorHelper.GetFacetGradientBrush($"FacetColor{ColorHelper.GetFacetColorName(facetData.color)}{facetData.gradient_id}");
            this.Index = facetData.index;
        }
    }

    /// <summary>
    /// 英雄技能
    /// </summary>
    public class HeroAbilityModel
    {
        /// <summary>
        /// 默认技能图标
        /// </summary>
        private static BitmapImage? _defaultAbilityImageSource128 = null;

        /// <summary>
        /// 技能图标
        /// </summary>
        public AsyncImage IconImage { get; private set; }

        /// <summary>
        /// 技能名称
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// 技能描述
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// 技能背景故事
        /// </summary>
        public string Lore { get; private set; } = string.Empty;

        /// <summary>
        /// 技能备注信息
        /// </summary>
        public string Notes { get; private set; } = string.Empty;

        /// <summary>
        /// 技能受到魔晶增强效果的描述
        /// </summary>
        public string ShardDescription { get; private set; } = string.Empty;

        /// <summary>
        /// 技能受到神杖增强效果的描述
        /// </summary>
        public string ScepterDescription { get; private set; } = string.Empty;

        /// <summary>
        /// 技能受到命石增强效果的描述
        /// </summary>
        public List<HeroAbilityFacetUpgradeModel> FacetsDescriptions { get; private set; } = [];

        /// <summary>
        /// 技能施法行为描述
        /// </summary>
        public string BehaviorDescription { get; private set; } = string.Empty;

        /// <summary>
        /// 技能目标描述
        /// </summary>
        public string TargetDescription { get; private set; } = string.Empty;

        /// <summary>
        /// 技能伤害类型
        /// </summary>
        public string DamageType { get; private set; } = string.Empty;

        /// <summary>
        /// 技能伤害类型的字体颜色
        /// </summary>
        public SolidColorBrush DamageTypeForeground { get; private set; }

        /// <summary>
        /// 技能对减益免疫的描述
        /// </summary>
        public string ImmunityDescription { get; private set; } = string.Empty;

        /// <summary>
        /// 技能驱散类型描述
        /// </summary>
        public string DispellableDescription { get; private set; } = string.Empty;

        /// <summary>
        /// 技能施法范围
        /// </summary>
        public string CastRanges { get; private set; } = string.Empty;

        /// <summary>
        /// 技能冷却时间
        /// </summary>
        public string Cooldowns { get; private set; } = string.Empty;

        /// <summary>
        /// 技能魔法消耗
        /// </summary>
        public string ManaCosts { get; private set; } = string.Empty;

        /// <summary>
        /// 技能生命消耗
        /// </summary>
        public string HealthCosts { get; private set; } = string.Empty;

        /// <summary>
        /// 技能的一些数值
        /// </summary>
        public string SpecialValueDescription { get; private set; } = string.Empty;

        /// <summary>
        /// 技能来自神杖
        /// </summary>
        public bool IsGrantedByScepter { get; private set; }

        /// <summary>
        /// 技能来自魔晶
        /// </summary>
        public bool IsGrantedByShard { get; private set; }

        /// <summary>
        /// 技能是否是先天技能
        /// </summary>
        public bool IsInnateAbility { get; private set; }

        /// <summary>
        /// 技能来自命石
        /// </summary>
        public bool IsGrantedByFacet { get; private set; }

        /// <summary>
        /// 技能来源的命石
        /// </summary>
        public HeroFacetModel? GrantedFacet { get; private set; }

        public HeroAbilityModel(DotaHeroAbility abilityData, List<HeroFacetModel> facets, HeroFacetModel? grantedFacet = null)
        {
            _defaultAbilityImageSource128 ??= new BitmapImage(new Uri("ms-appx:///Assets/img_placeholder_square.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 128,
            };

            this.IconImage = new AsyncImage(!abilityData.ability_is_innate ? $"{ConstantsCourier.ImageSourceDomain}/apps/dota2/images/dota_react/abilities/{abilityData.name}.png" : string.Empty, 0, 72, _defaultAbilityImageSource128);
            this.Name = abilityData.name_loc;
            this.Description = AbilityStringFormatter.FormatPlainText(AbilityStringFormatter.FormatAbilitySpecialValues(abilityData.desc_loc, abilityData.special_values));
            this.Lore = AbilityStringFormatter.FormatPlainText(abilityData.lore_loc);
            this.Notes = AbilityStringFormatter.FormatPlainText(AbilityStringFormatter.FormatAbilitySpecialValues(abilityData.notes_loc?.Length > 0 ? string.Join("\n", abilityData.notes_loc) : string.Empty, abilityData.special_values));
            this.ShardDescription = abilityData.ability_has_shard ? AbilityStringFormatter.FormatPlainText(AbilityStringFormatter.FormatAbilitySpecialValues(abilityData.shard_loc, abilityData.special_values)) : string.Empty;
            this.ScepterDescription = abilityData.ability_has_scepter ? AbilityStringFormatter.FormatPlainText(AbilityStringFormatter.FormatAbilitySpecialValues(abilityData.scepter_loc, abilityData.special_values)) : string.Empty;

            if (abilityData.facets_loc?.Length > 0)
            {
                for (int i = 0; i < abilityData.facets_loc.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(abilityData.facets_loc[i]))
                    {
                        var facet = facets.FirstOrDefault(x => x.Index == i);
                        if (facet is not null)
                        {
                            this.FacetsDescriptions.Add(new HeroAbilityFacetUpgradeModel(facet, AbilityStringFormatter.FormatPlainText(AbilityStringFormatter.FormatAbilitySpecialValues(abilityData.facets_loc[i], abilityData.special_values))));
                        }
                    }
                }
            }

            if (long.TryParse(abilityData.behavior, out long behavior))
            {
                this.BehaviorDescription = 0 != (65536 & behavior) ? "Aura"
                                         : 0 != (4 & behavior) ? "No Target"
                                         : 0 != (8 & behavior) ? "Unit Ttarget"
                                         : 0 != (16 & behavior) ? "Point Target"
                                         : 0 != (32 & behavior) ? "Point Aoe"
                                         : 0 != (128 & behavior) ? "Channeled"
                                         : 0 != (512 & behavior) ? "Toggle"
                                         : 0 != (4096 & behavior) ? "Autocast"
                                         : 0 != (2 & behavior) ? "Passive" : string.Empty;
            }

            this.TargetDescription = abilityData.target_team switch
            {
                1 => 7 == (7 & abilityData.target_type) ? "Allied Units And Buildings"
                   : 3 == (3 & abilityData.target_type) ? "Allied Units"
                   : 5 == (5 & abilityData.target_type) ? "Allied Heroes And Buildings"
                   : 1 == (1 & abilityData.target_type) ? "Allied Heroes"
                   : 2 == (2 & abilityData.target_type) ? "Allied Creeps" : "Allies",
                2 => 7 == (7 & abilityData.target_type) ? "Enemy Units And Buildings"
                   : 3 == (3 & abilityData.target_type) ? "Enemy Units"
                   : 5 == (5 & abilityData.target_type) ? "Enemy Heroes And Buildings"
                   : 1 == (1 & abilityData.target_type) ? "Enemy Heroes"
                   : 2 == (2 & abilityData.target_type) ? "Enemy Creeps" : "Enemies",
                3 => 1 == (1 & abilityData.target_type) ? "Heroes" : "Units",
                _ => string.Empty,
            };

            this.DamageType = abilityData.damage switch
            {
                1 => "Physical",
                2 => "Magical",
                4 => "Pure",
                8 => "HP Removal",
                _ => string.Empty,
            };

            this.DamageTypeForeground = ColorHelper.GetDamageTypeColor(abilityData.damage);

            this.ImmunityDescription = abilityData.immunity switch
            {
                1 => "Yes",
                2 => "No",
                3 => "Yes",
                4 => "No",
                5 => "Allies Yes Enemies No",
                _ => string.Empty,
            };

            this.DispellableDescription = abilityData.dispellable switch
            {
                1 => "Strong",
                2 => "Yes",
                3 => "No",
                _ => string.Empty,
            };

            this.CastRanges = (abilityData.cast_ranges?.Length > 0 && !(abilityData.cast_ranges.Length == 1 && abilityData.cast_ranges[0] == 0)) ? string.Join("/", Array.ConvertAll(abilityData.cast_ranges, x => Math.Floor(100 * x) / 100)) : string.Empty;
            this.Cooldowns = (abilityData.cooldowns?.Length > 0 && !(abilityData.cooldowns.Length == 1 && abilityData.cooldowns[0] == 0)) ? string.Join("/", Array.ConvertAll(abilityData.cooldowns, x => Math.Floor(100 * x) / 100)) : string.Empty;
            this.ManaCosts = (abilityData.mana_costs?.Length > 0 && !(abilityData.mana_costs.Length == 1 && abilityData.mana_costs[0] == 0)) ? string.Join("/", Array.ConvertAll(abilityData.mana_costs, x => Math.Floor(100 * x) / 100)) : string.Empty;
            this.HealthCosts = (abilityData.health_costs?.Length > 0 && !(abilityData.health_costs.Length == 1 && abilityData.health_costs[0] == 0)) ? string.Join("/", Array.ConvertAll(abilityData.health_costs, x => Math.Floor(100 * x) / 100)) : string.Empty;

            if (abilityData.special_values?.Length > 0)
            {
                var specialValuesDescriptionStringBuilder = new StringBuilder();

                for (int i = 0; i < abilityData.special_values.Length; i++)
                {
                    DotaHeroSpecialValue specialValue = abilityData.special_values[i];

                    if (specialValue.name.StartsWith('#') && string.IsNullOrEmpty(specialValue.heading_loc))
                    {
                        specialValue.heading_loc = AbilityStringFormatter.FormatSpecialValueNameWithHashSign(specialValue.name);
                    }

                    if (!string.IsNullOrEmpty(specialValue.heading_loc))
                    {
                        specialValuesDescriptionStringBuilder.Append(specialValue.heading_loc.Replace("\n", "").Trim()).Append(' ');

                        if (specialValue.values_float?.Length > 0)
                        {
                            specialValuesDescriptionStringBuilder.Append(string.Join("/", Array.ConvertAll(specialValue.values_float, x => Math.Floor(100 * x) / 100)));

                            if (specialValue.is_percentage)
                            {
                                specialValuesDescriptionStringBuilder.Append('%');
                            }
                        }

                        specialValuesDescriptionStringBuilder.Append('\n');
                    }
                }

                this.SpecialValueDescription = AbilityStringFormatter.FormatPlainText(specialValuesDescriptionStringBuilder.ToString().TrimEnd('\n'));
            }

            this.IsGrantedByScepter = abilityData.ability_is_granted_by_scepter;
            this.IsGrantedByShard = abilityData.ability_is_granted_by_shard;
            this.IsInnateAbility = abilityData.ability_is_innate;
            this.IsGrantedByFacet = grantedFacet is not null;
            this.GrantedFacet = grantedFacet;
        }
    }

    /// <summary>
    /// 技能受到命石增强效果
    /// </summary>
    /// <param name="facet"></param>
    /// <param name="description"></param>
    public class HeroAbilityFacetUpgradeModel(HeroFacetModel facet, string description)
    {
        /// <summary>
        /// 增强技能的命石
        /// </summary>
        public HeroFacetModel Facet { get; private set; } = facet;

        /// <summary>
        /// 命石对技能增强的描述
        /// </summary>
        public string Description { get; private set; } = description;
    }

    public static partial class AbilityStringFormatter
    {
        [System.Text.RegularExpressions.GeneratedRegex("<.*?>")]
        private static partial System.Text.RegularExpressions.Regex RegexHTMLTags();

        [System.Text.RegularExpressions.GeneratedRegex("&[^;]+;")]
        private static partial System.Text.RegularExpressions.Regex RegexSpecialCharacters();

        [System.Text.RegularExpressions.GeneratedRegex(@"\{s:[^}]+\}")]
        private static partial System.Text.RegularExpressions.Regex RegexValuePlaceholder();

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
                LogCourier.Log($"FormatString Error: {ex.Message}, original string is {originalString}", LogCourier.LogType.Error);
            }

            return result;
        }

        public static string FormatAbilitySpecialValues(string originalString, DotaHeroSpecialValue[]? specialValues)
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

                        result = RegexValuePlaceholder().Replace(result, "?");
                    }
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"FormatAbilitySpecialValues Error: {ex.Message}, original string is {originalString}", LogCourier.LogType.Error);
            }

            return result;
        }

        public static string FormatTalentSpecialValues(string originalString, string talentName, DotaHeroSpecialValue[]? specialValues, DotaHeroAbility[]? abilities, DotaHeroFacetAbility[]? facetAbilities)
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

                    result = RegexValuePlaceholder().Replace(result, "?");
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"FormatTalentSpecialValues Error: {ex.Message}, original string is {originalString}", LogCourier.LogType.Error);
            }

            return result;
        }

        public static string FormatFacetSpecialValues(string originalString, DotaHeroFacet facet, DotaHeroAbility[]? talents, DotaHeroAbility[]? abilities, DotaHeroFacetAbility? facetAbility)
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

                    result = RegexValuePlaceholder().Replace(result, "?");
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"FormatFacetSpecialValues Error: {ex.Message}, original string is {originalString}", LogCourier.LogType.Error);
            }

            return result;
        }

        public static string FormatSpecialValueNameWithHashSign(string originalString)
        {
            string result = originalString;

            try
            {
                if (string.IsNullOrWhiteSpace(result))
                {
                    return result;
                }

                var words = result[1..].Replace('_', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].Length > 0)
                    {
                        words[i] = char.ToUpper(words[i][0]) + words[i][1..];
                    }
                }

                result = string.Join(" ", words);
            }
            catch (Exception ex)
            {
                LogCourier.Log($"FormatSpecialValueName Error: {ex.Message}, original string is {originalString}", LogCourier.LogType.Error);
            }

            return result;
        }
    }
}
