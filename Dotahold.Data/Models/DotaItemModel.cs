using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaItemModel
    {
        /// <summary>
        /// 物品的 id
        /// </summary>
        [JsonConverter(typeof(SafeIntConverter))]
        public int id { get; set; }

        /// <summary>
        /// https://steamcdn-a.akamaihd.net/ + apps/dota2/images/items/{img}.png
        /// </summary>
        public string? img { get; set; } = string.Empty;

        /// <summary>
        /// 显示的名字
        /// </summary>
        public string? dname { get; set; } = string.Empty;

        /// <summary>
        /// 物品分类, "common", "rare"等等
        /// </summary>
        public string? qual { get; set; } = string.Empty;

        /// <summary>
        /// 价格
        /// </summary>
        [JsonConverter(typeof(SafeIntConverter))]
        public int cost { get; set; }

        /// <summary>
        /// 行为, "Unit Target", "Instant Cast", "No Target", "AOE" 等等
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string behavior { get; set; } = string.Empty;

        /// <summary>
        /// 伤害类型, "Physical", "Magical" 等等
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string dmg_type { get; set; } = string.Empty;

        /// <summary>
        /// 使用的介绍, 可能有多条, 例如第一条是主动效果, 第二条是被动
        /// </summary>
        public string[]? hint { get; set; } = [];

        /// <summary>
        /// 备注, 比如风杖的 notes 是你不能对友军使用
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string notes { get; set; } = string.Empty;

        /// <summary>
        /// 是否无视BKB
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string bkbpierce { get; set; } = string.Empty;

        /// <summary>
        /// mana cost
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string mc { get; set; } = string.Empty;

        /// <summary>
        /// health cost
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string hc { get; set; } = string.Empty;

        /// <summary>
        /// cooldown time
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string cd { get; set; } = string.Empty;

        /// <summary>
        /// 背景介绍
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string lore { get; set; } = string.Empty;

        /// <summary>
        /// 中立物品等级
        /// </summary>
        [JsonConverter(typeof(SafeIntConverter))]
        public int tier { get; set; }

        /// <summary>
        /// 合成配方
        /// </summary>
        public string[]? components { get; set; } = [];

        /// <summary>
        /// 目标阵营, "Enemy", "Both" 等等
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string target_team { get; set; } = string.Empty;

        /// <summary>
        /// 目标类型, "Basic", "Hero" 等等
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string target_type { get; set; } = string.Empty;

        /// <summary>
        /// 驱散类型, "Strong Dispels Only", "Yes" 等等
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string dispellable { get; set; } = string.Empty;

        /// <summary>
        /// 属性加成详情
        /// </summary>
        public Attrib[]? attrib { get; set; } = [];

        /// <summary>
        /// 物品技能详情
        /// </summary>
        public Ability[]? abilities { get; set; } = [];
    }

    public class Attrib
    {
        /// <summary>
        /// 物品加成的属性名字，例如 "bonus_int", 需要二次加工用于显示
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string key { get; set; } = string.Empty;

        /// <summary>
        /// 如果这个属性有值，则使用 value 替换 "{value}" 后显示该内容
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string display { get; set; } = string.Empty;

        /// <summary>
        /// 如果display没有值，则直接显示这个值
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string value { get; set; } = string.Empty;
    }

    public class Ability
    {
        /// <summary>
        /// 技能类型, 例如 "active"
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string type { get; set; } = string.Empty;

        /// <summary>
        /// 技能的名字
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string title { get; set; } = string.Empty;

        /// <summary>
        /// 技能的描述
        /// </summary>
        [JsonConverter(typeof(SafeStringConverter))]
        public string description { get; set; } = string.Empty;

    }
}
