using Dotahold.Core.DataShop;
using Dotahold.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class DotaHeroInfoModel
    {
        public Result result { get; set; }
    }

    public class Result
    {
        public Data data { get; set; }
        public int status { get; set; }
    }

    public class Data
    {
        public Hero[] heroes { get; set; }
    }

    public class Hero : ViewModelBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public int order_id { get; set; }
        public string name_loc { get; set; }
        public string bio_loc { get; set; }
        public string hype_loc { get; set; }
        public string npe_desc_loc { get; set; }
        public double str_base { get; set; }
        public double str_gain { get; set; }
        public double agi_base { get; set; }
        public double agi_gain { get; set; }
        public double int_base { get; set; }
        public double int_gain { get; set; }
        public double primary_attr { get; set; }
        public double complexity { get; set; }
        public double attack_capability { get; set; }
        public double[] role_levels { get; set; }
        public double damage_min { get; set; }
        public double damage_max { get; set; }
        public double attack_rate { get; set; }
        public double attack_range { get; set; }
        public double projectile_speed { get; set; }
        public double armor { get; set; }
        public double magic_resistance { get; set; }
        public double movement_speed { get; set; }
        public double turn_rate { get; set; }
        public double sight_range_day { get; set; }
        public double sight_range_night { get; set; }
        public double max_health { get; set; }
        public double health_regen { get; set; }
        public double max_mana { get; set; }
        public double mana_regen { get; set; }
        public List<Ability> abilities { get; set; }
        public List<Talent> talents { get; set; }


        // 经过处理之后的天赋描述
        [Newtonsoft.Json.JsonIgnore]
        private string _sTalentNameLoc10L = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sTalentNameLoc10L
        {
            get { return _sTalentNameLoc10L; }
            set { Set("sTalentNameLoc10L", ref _sTalentNameLoc10L, value); }
        }

        [Newtonsoft.Json.JsonIgnore]
        private string _sTalentNameLoc10R = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sTalentNameLoc10R
        {
            get { return _sTalentNameLoc10R; }
            set { Set("sTalentNameLoc10R", ref _sTalentNameLoc10R, value); }
        }

        [Newtonsoft.Json.JsonIgnore]
        private string _sTalentNameLoc15L = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sTalentNameLoc15L
        {
            get { return _sTalentNameLoc15L; }
            set { Set("sTalentNameLoc15L", ref _sTalentNameLoc15L, value); }
        }

        [Newtonsoft.Json.JsonIgnore]
        private string _sTalentNameLoc15R = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sTalentNameLoc15R
        {
            get { return _sTalentNameLoc15R; }
            set { Set("sTalentNameLoc15R", ref _sTalentNameLoc15R, value); }
        }

        [Newtonsoft.Json.JsonIgnore]
        private string _sTalentNameLoc20L = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sTalentNameLoc20L
        {
            get { return _sTalentNameLoc20L; }
            set { Set("sTalentNameLoc20L", ref _sTalentNameLoc20L, value); }
        }

        [Newtonsoft.Json.JsonIgnore]
        private string _sTalentNameLoc20R = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sTalentNameLoc20R
        {
            get { return _sTalentNameLoc20R; }
            set { Set("sTalentNameLoc20R", ref _sTalentNameLoc20R, value); }
        }

        [Newtonsoft.Json.JsonIgnore]
        private string _sTalentNameLoc25L = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sTalentNameLoc25L
        {
            get { return _sTalentNameLoc25L; }
            set { Set("sTalentNameLoc25L", ref _sTalentNameLoc25L, value); }
        }

        [Newtonsoft.Json.JsonIgnore]
        private string _sTalentNameLoc25R = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sTalentNameLoc25R
        {
            get { return _sTalentNameLoc25R; }
            set { Set("sTalentNameLoc25R", ref _sTalentNameLoc25R, value); }
        }

    }

    public class Ability : ViewModelBase
    {
        public static SolidColorBrush AbilityDamageTypeDefaultColor = new SolidColorBrush(Colors.Gray);
        public static SolidColorBrush AbilityDamageTypePhysicalColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        public static SolidColorBrush AbilityDamageTypeMagicalColor = new SolidColorBrush(Color.FromArgb(255, 163, 220, 238));
        public static SolidColorBrush AbilityDamageTypePureColor = new SolidColorBrush(Color.FromArgb(255, 255, 165, 0));
        public static SolidColorBrush AbilityDamageTypeHPRemovalColor = new SolidColorBrush(Color.FromArgb(255, 165, 15, 121));

        public int id { get; set; }
        public string name { get; set; }
        public string name_loc { get; set; }
        public string desc_loc { get; set; }
        public string lore_loc { get; set; }

        public string[] notes_loc { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        private string _notesStr = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string notesStr
        {
            get { return _notesStr; }
            set { Set("notesStr", ref _notesStr, value); }
        }

        public string shard_loc { get; set; }
        public string scepter_loc { get; set; }
        public double type { get; set; }

        // 技能类型，例如点目标、AOE、切换等
        public string behavior { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        private string _behaviorStr = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string behaviorStr
        {
            get { return _behaviorStr; }
            set { Set("behaviorStr", ref _behaviorStr, value); }
        }

        // 影响单位(作用单位，例如敌方单位、己方英雄等)
        public double target_team { get; set; }
        public int target_type { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        private string _targetStr = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string targetStr
        {
            get { return _targetStr; }
            set { Set("targetStr", ref _targetStr, value); }
        }

        public double flags { get; set; }

        // 伤害类型
        public double damage { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        private string _damageStr = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string damageStr
        {
            get { return _damageStr; }
            set { Set("damageStr", ref _damageStr, value); }
        }

        [Newtonsoft.Json.JsonIgnore]
        private SolidColorBrush _damageForeground = AbilityDamageTypeDefaultColor;
        [Newtonsoft.Json.JsonIgnore]
        public SolidColorBrush damageForeground
        {
            get { return _damageForeground; }
            set { Set("damageForeground", ref _damageForeground, value); }
        }

        // 是否无视魔免
        public double immunity { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        private string _immunityStr = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string immunityStr
        {
            get { return _immunityStr; }
            set { Set("immunityStr", ref _immunityStr, value); }
        }

        // 是否可驱散
        public double dispellable { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        private string _dispellableStr = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string dispellableStr
        {
            get { return _dispellableStr; }
            set { Set("dispellableStr", ref _dispellableStr, value); }
        }

        public double max_level { get; set; }

        // 范围
        public double[] cast_ranges { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        private string _castRangesStr = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string castRangesStr
        {
            get { return _castRangesStr; }
            set { Set("castRangesStr", ref _castRangesStr, value); }
        }

        // 间隔
        public double[] cast_points { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        private string _castPointsStr = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string castPointsStr
        {
            get { return _castPointsStr; }
            set { Set("castPointsStr", ref _castPointsStr, value); }
        }

        public double[] channel_times { get; set; }
        public double[] cooldowns { get; set; }
        public double[] durations { get; set; }

        public double[] damages { get; set; }

        public double[] mana_costs { get; set; }
        public double[] gold_costs { get; set; }
        public List<Special_Values> special_values { get; set; }
        public bool is_item { get; set; }
        public bool ability_has_scepter { get; set; }
        public bool ability_has_shard { get; set; }
        public bool ability_is_granted_by_scepter { get; set; }
        public bool ability_is_granted_by_shard { get; set; }
        public double item_cost { get; set; }
        public double item_initial_charges { get; set; }
        public double item_neutral_tier { get; set; }
        public double item_stock_max { get; set; }
        public double item_stock_time { get; set; }
        public double item_quality { get; set; }

        // 技能图片
        [Newtonsoft.Json.JsonIgnore]
        public string sAbilityImageUrl = "ms-appx:///Assets/Icons/item_placeholder.png";

        // 技能数值
        [Newtonsoft.Json.JsonIgnore]
        private string _specialValuesStr = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string specialValuesStr
        {
            get { return _specialValuesStr; }
            set { Set("specialValuesStr", ref _specialValuesStr, value); }
        }

        // 技能图片
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ImageSource = null;
        [Newtonsoft.Json.JsonIgnore]
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            set { Set("ImageSource", ref _ImageSource, value); }
        }
        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                ImageSource = await ImageCourier.GetImageAsync(sAbilityImageUrl);
                ImageSource.DecodePixelType = DecodePixelType.Logical;
                ImageSource.DecodePixelWidth = decodeWidth;
            }
            catch { }
        }
    }

    public class Talent
    {
        public int id { get; set; }
        public string name { get; set; }
        public string name_loc { get; set; }
        public string desc_loc { get; set; }
        public string lore_loc { get; set; }
        public string[] notes_loc { get; set; }
        public string shard_loc { get; set; }
        public string scepter_loc { get; set; }
        public double type { get; set; }
        public string behavior { get; set; }
        public double target_team { get; set; }
        public double target_type { get; set; }
        public double flags { get; set; }
        public double damage { get; set; }
        public double immunity { get; set; }
        public double dispellable { get; set; }
        public double max_level { get; set; }
        public double[] cast_ranges { get; set; }
        public double[] cast_points { get; set; }
        public double[] channel_times { get; set; }
        public double[] cooldowns { get; set; }
        public double[] durations { get; set; }
        public double[] damages { get; set; }
        public double[] mana_costs { get; set; }
        public double[] gold_costs { get; set; }
        public Special_Values[] special_values { get; set; }
        public bool is_item { get; set; }
        public bool ability_has_scepter { get; set; }
        public bool ability_has_shard { get; set; }
        public bool ability_is_granted_by_scepter { get; set; }
        public bool ability_is_granted_by_shard { get; set; }
        public double item_cost { get; set; }
        public double item_initial_charges { get; set; }
        public double item_neutral_tier { get; set; }
        public double item_stock_max { get; set; }
        public double item_stock_time { get; set; }
        public double item_quality { get; set; }
    }

    public class Special_Values
    {
        public string name { get; set; }
        public double[] values_float { get; set; }
        public bool is_percentage { get; set; }
        public string heading_loc { get; set; }
        public Bonuses[] bonuses { get; set; }
    }

    public class Bonuses
    {
        public string name { get; set; }
        public double value { get; set; }
        public double operation { get; set; }
    }
}


