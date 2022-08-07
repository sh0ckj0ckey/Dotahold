using Newtonsoft.Json;
using OpenDota_UWP.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.ViewModels
{
    public class DotaHeroesViewModel : ViewModelBase
    {
        private static Lazy<DotaHeroesViewModel> _lazyVM = new Lazy<DotaHeroesViewModel>(() => new DotaHeroesViewModel());
        public static DotaHeroesViewModel Instance => _lazyVM.Value;

        // 所有英雄
        public Dictionary<string, Models.DotaHeroModel> dictAllHeroes { get; set; } = new Dictionary<string, Models.DotaHeroModel>();

        // 列表展示的英雄
        public ObservableCollection<Models.DotaHeroModel> vStrHeroesList { get; set; } = new ObservableCollection<Models.DotaHeroModel>();
        public ObservableCollection<Models.DotaHeroModel> vAgiHeroesList { get; set; } = new ObservableCollection<Models.DotaHeroModel>();
        public ObservableCollection<Models.DotaHeroModel> vIntHeroesList { get; set; } = new ObservableCollection<Models.DotaHeroModel>();

        // 缓存拉取过的英雄详情
        private Dictionary<int, Models.DotaHeroInfoModel> dictHeroInfos { get; set; } = new Dictionary<int, Models.DotaHeroInfoModel>();

        // 缓存拉取过的英雄排行榜
        private Dictionary<int, Models.DotaHeroRankingModel> dictHeroRankings { get; set; } = new Dictionary<int, Models.DotaHeroRankingModel>();


        // 是否正在加载英雄列表
        private bool _bLoadingHeroes = false;
        public bool bLoadingHeroes
        {
            get { return _bLoadingHeroes; }
            set { Set("bLoadingHeroes", ref _bLoadingHeroes, value); }
        }

        // 英雄列表加载是否失败
        private bool _bFailedHeroes = false;
        public bool bFailedHeroes
        {
            get { return _bFailedHeroes; }
            set { Set("bFailedHeroes", ref _bFailedHeroes, value); }
        }


        // 是否正在加载英雄详情
        private bool _bLoadingHeroInfo = false;
        public bool bLoadingHeroInfo
        {
            get { return _bLoadingHeroInfo; }
            set { Set("bLoadingHeroInfo", ref _bLoadingHeroInfo, value); }
        }

        // 英雄详情加载是否失败
        private bool _bFailedHeroInfo = false;
        public bool bFailedHeroInfo
        {
            get { return _bFailedHeroInfo; }
            set { Set("bFailedHeroInfo", ref _bFailedHeroInfo, value); }
        }


        // 是否正在加载英雄排行榜
        private bool _bLoadingHeroRanking = false;
        public bool bLoadingHeroRanking
        {
            get { return _bLoadingHeroRanking; }
            set { Set("bLoadingHeroRanking", ref _bLoadingHeroRanking, value); }
        }

        // 英雄排行榜加载是否失败
        private bool _bFailedHeroRanking = false;
        public bool bFailedHeroRanking
        {
            get { return _bFailedHeroRanking; }
            set { Set("bFailedHeroRanking", ref _bFailedHeroRanking, value); }
        }

        // 英雄属性Tab
        private int _iHeroAttrTabIndex = 0;
        public int iHeroAttrTabIndex
        {
            get { return _iHeroAttrTabIndex; }
            set { Set("iHeroAttrTabIndex", ref _iHeroAttrTabIndex, value); }
        }

        // 当前选中的英雄
        private Models.DotaHeroModel _CurrentHero = null;
        public Models.DotaHeroModel CurrentHero
        {
            get { return _CurrentHero; }
            private set { Set("CurrentHero", ref _CurrentHero, value); }
        }

        // 当前选中的英雄的详情
        private Models.Hero _CurrentHeroInfo = null;
        public Models.Hero CurrentHeroInfo
        {
            get { return _CurrentHeroInfo; }
            private set { Set("CurrentHeroInfo", ref _CurrentHeroInfo, value); }
        }

        // 当前选中的英雄的英雄榜
        private ObservableCollection<Models.RankingPlayer> _vRankingPlayers = null;
        public ObservableCollection<Models.RankingPlayer> vRankingPlayers
        {
            get { return _vRankingPlayers; }
            private set { Set("vRankingPlayers", ref _vRankingPlayers, value); }
        }

        // 获取到英雄详情后触发动画
        public Action ActPopInHeroInfoGrid { get; set; } = null;
        public Action ActShowHeroInfoButton { get; set; } = null;

        // 是否已经成功加载过英雄列表
        private bool _bLoadedDotaHeroes = false;

        private Windows.Web.Http.HttpClient heroInfoHttpClient = new Windows.Web.Http.HttpClient();
        private Windows.Web.Http.HttpClient heroRankingHttpClient = new Windows.Web.Http.HttpClient();

        public DotaHeroesViewModel()
        {
            //LoadDotaHeroes();
        }

        // 固定返回true，用来等待返回值的
        public async Task<bool> LoadDotaHeroes()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Loading Heroes ---> " + DateTime.Now.Ticks);

                if (_bLoadedDotaHeroes)
                    return true;

                _bLoadedDotaHeroes = true;

                bLoadingHeroes = true;

                dictAllHeroes?.Clear();
                vStrHeroesList?.Clear();
                vAgiHeroesList?.Clear();
                vIntHeroesList?.Clear();
                dictAllHeroes = await ConstantsHelper.Instance.GetHeroesConstant();

                if (dictAllHeroes == null || dictAllHeroes.Count <= 0)
                {
                    bLoadingHeroes = false;
                    _bLoadedDotaHeroes = false;
                    return true;
                }

                // 处理图片下载等流程，然后逐个添加到列表里面
                foreach (var item in dictAllHeroes)
                {
                    item.Value.img = "https://cdn.cloudflare.steamstatic.com" + item.Value.img;
                    item.Value.icon = "https://cdn.cloudflare.steamstatic.com" + item.Value.icon;
                    string attr = item.Value.primary_attr.ToLower();
                    if (attr.Contains("str"))
                    {
                        vStrHeroesList.Add(item.Value);
                    }
                    else if (attr.Contains("agi"))
                    {
                        vAgiHeroesList.Add(item.Value);
                    }
                    else if (attr.Contains("int"))
                    {
                        vIntHeroesList.Add(item.Value);
                    }
                }
            }
            catch { _bLoadedDotaHeroes = false; }
            finally { bLoadingHeroes = false; }
            return true;
        }

        /// <summary>
        /// 选中英雄并拉取其详细信息
        /// </summary>
        /// <param name="selectedHero"></param>
        public async void PickHero(Models.DotaHeroModel selectedHero)
        {
            try
            {
                this.CurrentHero = selectedHero;

                string language = "english";
                if (DotaViewModel.Instance.iLanguageIndex == 1)
                {
                    language = "schinese";
                }
                else if (DotaViewModel.Instance.iLanguageIndex == 2)
                {
                    language = "russian";
                }

                var info = await ReqHeroInfo(this.CurrentHero.id, language);

                CurrentHeroInfo = info?.result?.data?.heroes?.Length > 0 ? info?.result?.data?.heroes[0] : null;

                if (CurrentHeroInfo == null)
                {
                    bFailedHeroInfo = true;
                }
                else
                {
                    bFailedHeroInfo = false;
                    OrganizeHeroTalents(CurrentHeroInfo);
                    OrganizeHeroAbilities(CurrentHeroInfo);
                }

                ActPopInHeroInfoGrid?.Invoke();
                ActShowHeroInfoButton?.Invoke();
            }
            catch { bFailedHeroInfo = true; }
        }

        /// <summary>
        /// 请求英雄详细数据信息
        /// </summary>
        /// <param name="heroId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        private async Task<Models.DotaHeroInfoModel> ReqHeroInfo(int heroId, string language = "english")
        {
            try
            {
                bLoadingHeroInfo = true;
                bFailedHeroInfo = false;

                if (dictHeroInfos.ContainsKey(heroId))
                {
                    await Task.Delay(600);
                    return dictHeroInfos[heroId];
                }

                string url = string.Format("https://www.dota2.com/datafeed/herodata?language={0}&hero_id={1}", language, heroId);

                try
                {
                    var response = await heroInfoHttpClient.GetAsync(new Uri(url));
                    string jsonMessage = await response.Content.ReadAsStringAsync();
                    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                    };

                    var infoModel = JsonConvert.DeserializeObject<Models.DotaHeroInfoModel>(jsonMessage, jsonSerializerSettings);

                    if (infoModel != null)
                    {
                        dictHeroInfos.Add(heroId, infoModel);
                        return infoModel;
                    }
                }
                catch { }
            }
            catch { }
            finally { bLoadingHeroInfo = false; }
            return null;
        }

        /// <summary>
        /// 整理英雄信息中的天赋树相关信息
        /// </summary>
        /// <param name="info"></param>
        private void OrganizeHeroTalents(Models.Hero info)
        {
            try
            {
                if (info?.talents == null) return;

                foreach (var talent in info.talents)
                {
                    string talentNameLoc = talent.name_loc;
                    foreach (var v in talent.special_values)
                    {
                        try
                        {
                            if (v.values_float.Length > 0)
                            {
                                string value = (Math.Floor(100 * v.values_float[0]) / 100).ToString();
                                talentNameLoc = talentNameLoc.Replace($"{{s:{v.name}}}", value);
                            }
                        }
                        catch { }
                    }
                    foreach (var abi in info.abilities)
                    {
                        foreach (var v in abi.special_values)
                        {
                            if (v.bonuses != null && v.bonuses.Length > 0)
                            {
                                foreach (var bonus in v.bonuses)
                                {
                                    try
                                    {
                                        if (bonus.name == talent.name)
                                        {
                                            string value = (Math.Floor(100 * bonus.value) / 100).ToString();
                                            talentNameLoc = talentNameLoc.Replace($"{{s:bonus_{v.name}}}", value);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }

                    talent.name_loc = talentNameLoc;
                }

                info.sTalentNameLoc10L = info?.talents[0]?.name_loc;
                info.sTalentNameLoc10R = info?.talents[1]?.name_loc;
                info.sTalentNameLoc15L = info?.talents[2]?.name_loc;
                info.sTalentNameLoc15R = info?.talents[3]?.name_loc;
                info.sTalentNameLoc20L = info?.talents[4]?.name_loc;
                info.sTalentNameLoc20R = info?.talents[5]?.name_loc;
                info.sTalentNameLoc25L = info?.talents[6]?.name_loc;
                info.sTalentNameLoc25R = info?.talents[7]?.name_loc;
            }
            catch { }

            #region JavaScriptCode from dota2.com 2022/8/7 main.js?v=7ovgxfp53jKo&l=english&_cdn=cloudflare
            // Ri = function(e, t)
            // {
            //     var talent = info.talents[t],
            //       talentNameLoc = e.talents[t].name_loc;
            //     return (
            //       talent.special_values.forEach(function(e) {
            //         if (e.values_float.length > 0)
            //         {
            //             var t = e.values_float[0];
            //             (t = Math.floor(100 * t) / 100),
            //           (talentNameLoc = talentNameLoc.replace("{s:" + e.name + "}", "" + t));
            //         }
            //     }),
            //     info.abilities.forEach(function(e) {
            //         info.special_values.forEach(function(e) {
            //             var t;
            //             null == (t = e.bonuses) ||
            //               void 0 == t ||
            //               t.forEach(function(t) {
            //                 if (t.name == talent.name)
            //                 {
            //                     var r = t.value;
            //                     (r = Math.floor(100 * r) / 100),
            //                 (talentNameLoc = talentNameLoc.replace("{s:bonus_" + e.name + "}", "" + r));
            //                 }
            //             });
            //         });
            //     }),
            //     n
            //   );
            // }
            #endregion
        }

        /// <summary>
        /// 整理英雄信息中的技能信息
        /// </summary>
        /// <param name="info"></param>
        private void OrganizeHeroAbilities(Models.Hero info)
        {
            try
            {
                if (info?.abilities == null) return;

                foreach (var ability in info.abilities)
                {
                    // 技能图片和描述
                    ability.sAbilityImageUrl = "https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/abilities/" + ability.name + ".png";
                    ability.desc_loc = OrganizeLocString(ability.desc_loc, ability.special_values);
                    ability.lore_loc = OrganizeLocString(ability.lore_loc, ability.special_values);

                    // 备注
                    try
                    {
                        if (ability.notes_loc != null && ability.notes_loc.Length > 0)
                        {
                            StringBuilder notesLocSb = new StringBuilder();
                            for (int i = 0; i < ability.notes_loc.Length; i++)
                            {
                                notesLocSb.Append(ability.notes_loc[i]);
                                if (i < ability.notes_loc.Length - 1)
                                {
                                    notesLocSb.Append("\n");
                                }
                            }
                            ability.notesStr = notesLocSb.ToString();
                        }
                    }
                    catch { }

                    // 技能是否有神杖效果
                    try
                    {
                        if (ability.ability_has_scepter)
                        {
                            if (string.IsNullOrEmpty(ability.scepter_loc))
                            {
                                //ability.scepter_loc = "Upgraded by Scepter";
                                ability.ability_has_scepter = false;
                            }
                            else
                            {
                                ability.scepter_loc = OrganizeLocString(ability.scepter_loc, ability.special_values);
                            }
                        }
                    }
                    catch { }

                    // 技能是否有魔晶效果
                    try
                    {
                        if (ability.ability_has_shard)
                        {
                            if (string.IsNullOrEmpty(ability.shard_loc))
                            {
                                //ability.shard_loc = "Upgraded by Shard";
                                ability.ability_has_shard = false;
                            }
                            else
                            {
                                ability.shard_loc = OrganizeLocString(ability.shard_loc, ability.special_values);
                            }
                        }
                    }
                    catch { }

                    // 技能伤害类型
                    try
                    {
                        switch (ability.damage)
                        {
                            case 1:
                                ability.damageStr = "Physical";
                                ability.damageForeground = Models.Ability.AbilityDamageTypePhysicalColor;
                                break;
                            case 2:
                                ability.damageStr = "Magical";
                                ability.damageForeground = Models.Ability.AbilityDamageTypeMagicalColor;
                                break;
                            case 4:
                                ability.damageStr = "Pure";
                                ability.damageForeground = Models.Ability.AbilityDamageTypePureColor;
                                break;
                            case 8:
                                ability.damageStr = "HP Removal";
                                ability.damageForeground = Models.Ability.AbilityDamageTypeHPRemovalColor;
                                break;
                        }
                    }
                    catch { }

                    // 技能是否可以驱散
                    try
                    {
                        switch (ability.dispellable)
                        {
                            case 1:
                                ability.dispellableStr = "Strong";
                                break;
                            case 2:
                                ability.dispellableStr = "Yes";
                                break;
                            case 3:
                                ability.dispellableStr = "No";
                                break;
                        }
                    }
                    catch { }

                    // 技能是否无视魔免
                    try
                    {
                        switch (ability.immunity)
                        {
                            case 1:
                            case 3:
                                ability.immunityStr = "Yes";
                                break;
                            case 2:
                            case 4:
                                ability.immunityStr = "No";
                                break;
                            case 5:
                                ability.immunityStr = "Allies Yes Enemies No";
                                break;
                        }
                    }
                    catch { }

                    // 技能作用目标
                    try
                    {
                        switch (ability.target_team)
                        {
                            case 1:
                                ability.targetStr = 7 == (7 & ability.target_type)
                                                    ? "Allied Units And Buildings"
                                                    : 3 == (3 & ability.target_type)
                                                    ? "Allied Units"
                                                    : 5 == (5 & ability.target_type)
                                                    ? "Allied Heroes And Buildings"
                                                    : 1 == (1 & ability.target_type)
                                                    ? "Allied Heroes"
                                                    : 2 == (2 & ability.target_type)
                                                    ? "Allied Creeps" : "Allies";
                                break;
                            case 2:
                                ability.targetStr = 7 == (7 & ability.target_type)
                                                    ? "Enemy Units And Buildings"
                                                    : 3 == (3 & ability.target_type)
                                                    ? "Enemy Units"
                                                    : 5 == (5 & ability.target_type)
                                                    ? "Enemy Heroes And Buildings"
                                                    : 1 == (1 & ability.target_type)
                                                    ? "Enemy Heroes"
                                                    : 2 == (2 & ability.target_type)
                                                    ? "Enemy Creeps" : "Enemies";
                                break;
                            case 3:
                                ability.targetStr = 1 == (1 & ability.target_type)
                                                    ? "Heroes" : "Units";
                                break;
                        }
                    }
                    catch { }

                    // 技能类型
                    try
                    {
                        long behavior;
                        bool parse = long.TryParse(ability.behavior, out behavior);
                        if (parse)
                        {
                            ability.behaviorStr = 0 != (65536 & behavior)
                                                ? "Aura"
                                                : 0 != (4 & behavior)
                                                ? "No Target"
                                                : 0 != (8 & behavior)
                                                ? "Unit Ttarget"
                                                : 0 != (16 & behavior)
                                                ? "Point Target"
                                                : 0 != (32 & behavior)
                                                ? "Point Aoe"
                                                : 0 != (128 & behavior)
                                                ? "Channeled"
                                                : 0 != (512 & behavior)
                                                ? "Toggle"
                                                : 0 != (4096 & behavior)
                                                ? "Autocast"
                                                : 0 != (2 & behavior)
                                                ? "Passive" : string.Empty;
                        }
                    }
                    catch { }

                    //// 技能范围
                    //try
                    //{
                    //    if (ability.cast_ranges != null && ability.cast_ranges.Length > 0)
                    //    {
                    //        StringBuilder castRangesSb = new StringBuilder();
                    //        bool haveCastRanges = false;
                    //        for (int i = 0; i < ability.cast_ranges.Length; i++)
                    //        {
                    //            double append = Math.Floor(ability.cast_ranges[i] * 10000) / 10000;
                    //            castRangesSb.Append(append);

                    //            if (i < ability.cast_ranges.Length - 1)
                    //            {
                    //                castRangesSb.Append("/");
                    //            }

                    //            if (append > 0)
                    //            {
                    //                haveCastRanges = true;
                    //            }
                    //        }
                    //        if (haveCastRanges)
                    //        {
                    //            ability.castRangesStr = castRangesSb.ToString();
                    //        }
                    //    }
                    //}
                    //catch { }

                    //// 技能间隔
                    //try
                    //{
                    //    if (ability.cast_points != null && ability.cast_points.Length > 0)
                    //    {
                    //        StringBuilder castPointsSb = new StringBuilder();
                    //        bool haveCastPoints = false;
                    //        for (int i = 0; i < ability.cast_points.Length; i++)
                    //        {
                    //            double append = Math.Floor(ability.cast_points[i] * 10000) / 10000;
                    //            castPointsSb.Append(append);

                    //            if (i < ability.cast_points.Length - 1)
                    //            {
                    //                castPointsSb.Append("/");
                    //            }

                    //            if (append > 0)
                    //            {
                    //                haveCastPoints = true;
                    //            }
                    //        }
                    //        if (haveCastPoints)
                    //        {
                    //            ability.castPointsStr = castPointsSb.ToString();
                    //        }
                    //    }
                    //}
                    //catch { }

                    // 技能其他的数值
                    try
                    {
                        //Dictionary<string, string> SpecialValuesNames = new Dictionary<string, string>()
                        //{
                        //    {"#hero_ability", "Ability"}, {"#HeroAbility", "Ability"},
                        //    {"#hero_affects", "Affects"}, {"#HeroAffects", "Affects"},
                        //    {"#ability_damage", "Damage"}, {"#AbilityDamage", "Damage"},
                        //    {"#hero_spell_immunity", "Immunity"}, {"#heroSpellImmunity", "Immunity"},
                        //    {"#hero_dispellable", "Dispellable"}, {"#HeroDispellable", "Dispellable"},
                        //    {"#hero_damage", "Damage"}, {"#HeroDamage", "Damage"}
                        //};
                        StringBuilder specialValuesSb = new StringBuilder();
                        for (int j = 0; j < ability.special_values.Count; j++)
                        {
                            var specialValue = ability.special_values[j];

                            if ((specialValue.name == "#AbilityDamage" || specialValue.name == "#HeroDamage") && string.IsNullOrEmpty(specialValue.heading_loc))
                            {
                                specialValue.heading_loc = "DAMAGE:";
                            }

                            if (!string.IsNullOrEmpty(specialValue.heading_loc) && specialValue.values_float != null && specialValue.values_float.Length > 0)
                            {
                                StringBuilder specialValueSb = new StringBuilder();
                                specialValueSb.Append(specialValue.heading_loc.Replace("\n", "").Trim());
                                specialValueSb.Append(" ");
                                for (int i = 0; i < specialValue.values_float.Length; i++)
                                {
                                    if (i > 0)
                                    {
                                        specialValueSb.Append("/");
                                    }
                                    specialValueSb.Append(specialValue.values_float[i].ToString("f1").Replace("\n", ""));
                                    if (specialValue.is_percentage)
                                    {
                                        specialValueSb.Append("%");
                                    }
                                }
                                specialValuesSb.Append(specialValueSb);
                                specialValuesSb.Append("\n");
                            }
                        }
                        ability.specialValuesStr = specialValuesSb.ToString().TrimEnd('\n');
                        ability.specialValuesStr = OrganizeLocString(ability.specialValuesStr, null);
                    }
                    catch { }
                }
            }
            catch { }

            #region JavaScriptCode from dota2.com 2022/8/7 main.js?v=7ovgxfp53jKo&l=english&_cdn=cloudflare
            /*
                 Ui = Object(d.a)(function (e) {
                  var t = e.heroData,
                    a = Object(o.useState)(!1),
                    n = a[0],
                    r = a[1];
                  if (!t) return null;
                  var i = Xl.getSelectedAbilityIndex(),
                    l =
                      t.abilities.filter(function (e) {
                        return e.ability_is_granted_by_shard;
                      })[0] || void 0,
                    c =
                      t.abilities.filter(function (e) {
                        return 602 != e.id && e.ability_is_granted_by_scepter;
                      })[0] || void 0;
                  l ||
                    (l =
                      t.abilities.filter(function (e) {
                        return e.ability_has_shard;
                      })[0] || void 0),
                    c ||
                      (c =
                        t.abilities.filter(function (e) {
                          return e.ability_has_scepter;
                        })[0] || void 0);
                  var u = void 0,
                    d = !1,
                    m = !1;
                  if (
                    (-1 == i
                      ? ((d = !0), (u = l))
                      : -2 == i
                      ? ((m = !0), (u = c))
                      : (u = t.abilities[i]),
                    !u)
                  )
                    return null;
                  var p = void 0,
                    _ = void 0,
                    h = void 0,
                    f = void 0,
                    b = void 0;
                  65536 & u.behavior
                    ? (p = "#ability_behavior_aura")
                    : 4 & u.behavior
                    ? (p = "#ability_behavior_no_target")
                    : 8 & u.behavior
                    ? (p = "#ability_behavior_unit_target")
                    : 16 & u.behavior
                    ? (p = "#ability_behavior_point_target")
                    : 32 & u.behavior
                    ? (p = "#ability_behavior_point_aoe")
                    : 128 & u.behavior
                    ? (p = "#ability_behavior_channeled")
                    : 512 & u.behavior
                    ? (p = "#ability_behavior_toggle")
                    : 4096 & u.behavior
                    ? (p = "#ability_behavior_autocast")
                    : 2 & u.behavior && (p = "#ability_behavior_passive"),
                    1 == u.immunity || 3 == u.immunity
                      ? (_ = "#yes")
                      : 2 == u.immunity || 4 == u.immunity
                      ? (_ = "#no")
                      : 5 == u.immunity && (_ = "#ability_immunity_alliesyesenemiesno"),
                    1 == u.target_team
                      ? (h =
                          7 == (7 & u.target_type)
                            ? "#ability_target_alliedunitsandubildings"
                            : 3 == (3 & u.target_type)
                            ? "#ability_target_alliedunits"
                            : 5 == (5 & u.target_type)
                            ? "#ability_target_alliedheroesandbuildings"
                            : 1 == (1 & u.target_type)
                            ? "#ability_target_alliedheroes"
                            : 2 == (2 & u.target_type)
                            ? "#ability_target_alliedcreeps"
                            : "#ability_target_allies")
                      : 2 == u.target_team
                      ? (h =
                          7 == (7 & u.target_type)
                            ? "#ability_target_enemyunitsandubildings"
                            : 3 == (3 & u.target_type)
                            ? "#ability_target_enemyunits"
                            : 5 == (5 & u.target_type)
                            ? "#ability_target_enemyheroesandbuildings"
                            : 1 == (1 & u.target_type)
                            ? "#ability_target_enemyheroes"
                            : 2 == (2 & u.target_type)
                            ? "#ability_target_enemycreeps"
                            : "#ability_target_enemies")
                      : 3 == u.target_team &&
                        (h =
                          1 == (1 & u.target_type)
                            ? "#ability_target_heroes"
                            : "#ability_target_units"),
                    1 == u.damage
                      ? (f = "#ability_damage_physical")
                      : 2 == u.damage
                      ? (f = "#ability_damage_magical")
                      : 4 == u.damage
                      ? (f = "#ability_damage_pure")
                      : 8 == u.damage && (f = "#ability_damage_hpremoval"),
                    1 == u.dispellable
                      ? (b = "#ability_dispellable_strong")
                      : 2 == u.dispellable
                      ? (b = "#yes")
                      : 3 == u.dispellable && (b = "#no");
                  var y = u.desc_loc,
                    S = u.scepter_loc,
                    O = u.shard_loc;
                  u.special_values.forEach(function (e) {
                    var t =
                      e.values_float.length > 0 ? e.values_float[0].toFixed(1) : "0";
                    (y = y.replace(
                      new RegExp("%" + e.name.toLowerCase() + "%", "g"),
                      t
                    )),
                      (S = S.replace(
                        new RegExp("%" + e.name.toLowerCase() + "%", "g"),
                        t
                      )),
                      (O = O.replace(
                        new RegExp("%" + e.name.toLowerCase() + "%", "g"),
                        t
                      ));
                  }),
                    (y = y.replace(/\%\%/g, "%")),
                    (S = S.replace(/\%\%/g, "%")),
                    (O = O.replace(/\%\%/g, "%"));
                  var C = t.name.replace("npc_dota_hero_", ""),
                    I = u.name;
                  d && (I = C + "_aghanims_shard"), m && (I = C + "_aghanims_scepter");
                  var N = y;
                  d && !u.ability_is_granted_by_shard && (N = O),
                    m && !u.ability_is_granted_by_scepter && (N = S);
                  var T = function (e) {
                      Xl.setSelectedAbilityIndex(e), r(!0);
                    },
                    w = m && u.ability_has_scepter && !u.ability_is_granted_by_scepter,
                    D = d && u.ability_has_shard && !u.ability_is_granted_by_shard,
                    L = m && u.ability_is_granted_by_scepter,
                    k = d && u.ability_is_granted_by_shard;
                  return s.a.createElement(
                    "div",
                    { className: Di.a.HeroAbilities },
                    s.a.createElement(
                      "div",
                      { className: Di.a.AbilityLeft },
                      s.a.createElement(
                        "div",
                        { className: Di.a.VideoContainer },
                        s.a.createElement("div", {
                          className: Object(v.a)(Di.a.FadeUp, n && Di.a.DoFadeAnim),
                          onAnimationEnd: function () {
                            return r(!1);
                          },
                        }),
                        s.a.createElement(
                          "video",
                          {
                            key: I,
                            className: Di.a.HeroPortrait,
                            autoPlay: !0,
                            preload: "auto",
                            muted: !0,
                            loop: !0,
                            playsInline: !0,
                            poster: g.a.VIDEO_URL + "abilities/" + C + "/" + I + ".jpg",
                          },
                          s.a.createElement("source", {
                            type: "video/webm",
                            src: g.a.VIDEO_URL + "abilities/" + C + "/" + I + ".webm",
                          }),
                          s.a.createElement("source", {
                            type: "video/mp4",
                            src: g.a.VIDEO_URL + "abilities/" + C + "/" + I + ".mp4",
                          })
                        )
                      ),
                      s.a.createElement(
                        "div",
                        { className: Di.a.AbilitySelector },
                        t.abilities.map(function (e, t) {
                          return e.ability_is_granted_by_scepter ||
                            e.ability_is_granted_by_shard ||
                            602 == e.id ||
                            322 == e.id
                            ? null
                            : s.a.createElement("div", {
                                key: e.name,
                                className: Object(v.a)(
                                  Di.a.AbilitySelectable,
                                  t != i && Di.a.NotSelected
                                ),
                                style: {
                                  backgroundImage:
                                    "url( " +
                                    g.a.IMG_URL +
                                    "abilities/" +
                                    e.name +
                                    ".png )",
                                },
                                onClick: function () {
                                  return T(t);
                                },
                              });
                        }),
                        l &&
                          s.a.createElement(
                            "div",
                            {
                              key: l.name + "_shard",
                              className: Object(v.a)(
                                Di.a.AbilitySelectable,
                                Di.a.Shard,
                                -1 != i && Di.a.NotSelected
                              ),
                              style: {
                                backgroundImage:
                                  "url( " +
                                  g.a.IMG_URL +
                                  "abilities/" +
                                  l.name +
                                  ".png )",
                              },
                              onClick: function () {
                                return T(-1);
                              },
                            },
                            s.a.createElement("div", {
                              className: Di.a.SubIcon,
                              style: {
                                backgroundImage:
                                  "url( " +
                                  g.a.IMG_URL +
                                  "heroes/stats/aghs_shard.png )",
                              },
                            })
                          ),
                        c &&
                          s.a.createElement(
                            "div",
                            {
                              key: c.name + "_scepter",
                              className: Object(v.a)(
                                Di.a.AbilitySelectable,
                                Di.a.Scepter,
                                -2 != i && Di.a.NotSelected
                              ),
                              style: {
                                backgroundImage:
                                  "url( " +
                                  g.a.IMG_URL +
                                  "abilities/" +
                                  c.name +
                                  ".png )",
                              },
                              onClick: function () {
                                return T(-2);
                              },
                            },
                            s.a.createElement("div", {
                              className: Di.a.SubIcon,
                              style: {
                                backgroundImage:
                                  "url( " +
                                  g.a.IMG_URL +
                                  "heroes/stats/aghs_scepter.png )",
                              },
                            })
                          )
                      )
                    ),
                    s.a.createElement(
                      "div",
                      { className: Di.a.AbilityRight },
                      s.a.createElement(
                        "div",
                        { className: Di.a.AbilityInfoContainer },
                        s.a.createElement(
                          "div",
                          { className: Di.a.AbilityMain },
                          s.a.createElement("img", {
                            className: Di.a.AbilityImage,
                            src: g.a.IMG_URL + "abilities/" + u.name + ".png",
                          }),
                          s.a.createElement(
                            "div",
                            { className: Di.a.AbilityInfo },
                            s.a.createElement(
                              "div",
                              { className: Di.a.AbilityName },
                              u.name_loc
                            ),
                            w &&
                              s.a.createElement(
                                "div",
                                { className: Di.a.AghType },
                                Object(E.k)("#ability_upgrade_scepter")
                              ),
                            D &&
                              s.a.createElement(
                                "div",
                                { className: Di.a.AghType },
                                Object(E.k)("#ability_upgrade_shard")
                              ),
                            L &&
                              s.a.createElement(
                                "div",
                                { className: Di.a.AghType },
                                Object(E.k)("#ability_new_scepter")
                              ),
                            k &&
                              s.a.createElement(
                                "div",
                                { className: Di.a.AghType },
                                Object(E.k)("#ability_new_shard")
                              ),
                            s.a.createElement(
                              "div",
                              { className: Di.a.AbilityDesc },
                              Object(E.k)(N)
                            )
                          )
                        ),
                        s.a.createElement(
                          "div",
                          { className: Di.a.AbilityDetails },
                          s.a.createElement(
                            "div",
                            {
                              className: Object(v.a)(
                                Di.a.GenericValues,
                                (w || D) && Di.a.Hidden
                              ),
                            },
                            s.a.createElement(
                              "div",
                              { className: Di.a.Column },
                              s.a.createElement(
                                "div",
                                { className: Di.a.DetailsValues },
                                p &&
                                  s.a.createElement(
                                    "div",
                                    { className: Di.a.ValueElement },
                                    Object(E.k)("#hero_ability"),
                                    ":",
                                    s.a.createElement(
                                      "div",
                                      { className: Di.a.ValueValue },
                                      Object(E.k)(p)
                                    )
                                  ),
                                h &&
                                  s.a.createElement(
                                    "div",
                                    { className: Di.a.ValueElement },
                                    Object(E.k)("#hero_affects"),
                                    ":",
                                    s.a.createElement(
                                      "div",
                                      { className: Di.a.ValueValue },
                                      Object(E.k)(h)
                                    )
                                  ),
                                f &&
                                  s.a.createElement(
                                    "div",
                                    { className: Di.a.ValueElement },
                                    Object(E.k)("#ability_damage"),
                                    ":",
                                    s.a.createElement(
                                      "div",
                                      { className: Di.a.ValueValue },
                                      Object(E.k)(f)
                                    )
                                  )
                              )
                            ),
                            s.a.createElement(
                              "div",
                              { className: Di.a.Column },
                              s.a.createElement(
                                "div",
                                { className: Di.a.DetailsValues },
                                _ &&
                                  s.a.createElement(
                                    "div",
                                    { className: Di.a.ValueElement },
                                    Object(E.k)("#hero_spell_immunity"),
                                    ":",
                                    s.a.createElement(
                                      "div",
                                      { className: Di.a.ValueValue },
                                      Object(E.k)(_)
                                    )
                                  ),
                                b &&
                                  s.a.createElement(
                                    "div",
                                    { className: Di.a.ValueElement },
                                    Object(E.k)("#hero_dispellable"),
                                    ":",
                                    s.a.createElement(
                                      "div",
                                      { className: Di.a.ValueValue },
                                      Object(E.k)(b)
                                    )
                                  )
                              )
                            )
                          ),
                          s.a.createElement(
                            "div",
                            {
                              className: Object(v.a)(
                                Di.a.SpecificValues,
                                (w || D) && Di.a.Hidden
                              ),
                            },
                            u.damages.reduce(function (e, t) {
                              return e + t;
                            }) > 0 &&
                              s.a.createElement(
                                "div",
                                { className: Di.a.SpecialElement },
                                Object(E.k)("#hero_damage"),
                                s.a.createElement(
                                  "div",
                                  { className: Di.a.SpecialValue },
                                  u.damages.map(function (e, t) {
                                    return (t > 0 ? " / " : "") + e.toFixed(0);
                                  })
                                )
                              ),
                            u.special_values.map(function (e) {
                              return 0 == e.heading_loc.length
                                ? null
                                : s.a.createElement(
                                    "div",
                                    { key: e.name, className: Di.a.SpecialElement },
                                    Object(E.k)(e.heading_loc),
                                    s.a.createElement(
                                      "div",
                                      { className: Di.a.SpecialValue },
                                      e.values_float.map(function (t, a) {
                                        return (
                                          (a > 0 ? " / " : "") +
                                          t.toFixed(1) +
                                          (e.is_percentage ? "%" : "")
                                        );
                                      })
                                    )
                                  );
                            })
                          ),
                          (u.cooldowns.reduce(function (e, t) {
                            return e + t;
                          }) > 0 ||
                            u.mana_costs.reduce(function (e, t) {
                              return e + t;
                            }) > 0) &&
                            s.a.createElement(
                              "div",
                              {
                                className: Object(v.a)(
                                  Di.a.BottomValues,
                                  (D || D) && Di.a.Hidden
                                ),
                              },
                              s.a.createElement(
                                "div",
                                { className: Di.a.CooldownContainer },
                                s.a.createElement("div", {
                                  className: Di.a.CooldownIcon,
                                  style: {
                                    backgroundImage:
                                      "url( " + g.a.IMG_URL + "icons/cooldown.png )",
                                  },
                                }),
                                s.a.createElement(
                                  "div",
                                  { className: Di.a.CooldownText },
                                  u.cooldowns.map(function (e, t) {
                                    return (t > 0 ? " / " : "") + e.toFixed(1);
                                  })
                                )
                              ),
                              s.a.createElement(
                                "div",
                                { className: Di.a.ManaContainer },
                                s.a.createElement("div", { className: Di.a.ManaIcon }),
                                s.a.createElement(
                                  "div",
                                  { className: Di.a.ManaText },
                                  u.mana_costs.map(function (e, t) {
                                    return (t > 0 ? " / " : "") + e.toFixed(0);
                                  })
                                )
                              )
                            ),
                          s.a.createElement(
                            "div",
                            {
                              className: Object(v.a)(
                                Di.a.Lore,
                                (D || D) && Di.a.Hidden
                              ),
                            },
                            u.lore_loc
                          )
                        )
                      )
                    )
                  );
                }),
                 */
            #endregion
        }

        /// <summary>
        /// 加载英雄榜列表
        /// </summary>
        /// <param name="heroId"></param>
        public async void FetchHeroRanking(int heroId)
        {
            try
            {
                vRankingPlayers?.Clear();
                vRankingPlayers = new ObservableCollection<Models.RankingPlayer>();

                var ranking = await ReqHeroRanking(heroId);

                if (ranking == null || ranking.rankings == null || ranking.hero_id != this.CurrentHero.id.ToString())
                {
                    bFailedHeroInfo = true;
                }
                else
                {
                    bFailedHeroInfo = false;

                    int rank = 1;
                    foreach (var item in ranking.rankings)
                    {
                        try
                        {
                            item.iRank = rank;
                            rank++;
                            int dotIndex = item.score.IndexOf('.');
                            string score = dotIndex <= 0 ? item.score : item.score.Substring(0, dotIndex);
                            item.score = score;
                            vRankingPlayers.Add(item);
                        }
                        catch { }
                    }
                }
            }
            catch { bFailedHeroInfo = true; }
        }

        /// <summary>
        /// 请求英雄榜列表
        /// </summary>
        /// <param name="heroId"></param>
        private async Task<Models.DotaHeroRankingModel> ReqHeroRanking(int heroId)
        {
            try
            {
                bLoadingHeroRanking = true;
                bFailedHeroRanking = false;

                if (dictHeroRankings.ContainsKey(heroId))
                {
                    await Task.Delay(600);
                    return dictHeroRankings[heroId];
                }

                string url = string.Format("https://api.opendota.com/api/rankings?hero_id={0}", heroId);

                try
                {
                    var response = await heroRankingHttpClient.GetAsync(new Uri(url));
                    string jsonMessage = await response.Content.ReadAsStringAsync();
                    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                    };

                    var rankingModel = JsonConvert.DeserializeObject<Models.DotaHeroRankingModel>(jsonMessage, jsonSerializerSettings);

                    if (rankingModel != null)
                    {
                        dictHeroRankings.Add(heroId, rankingModel);
                        return rankingModel;
                    }
                }
                catch { }
            }
            catch { }
            finally { bLoadingHeroRanking = false; }
            return null;
        }

        /// <summary>
        /// 去掉HTML标签等额外的文字，并将占位符替换为相应的special_values
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string OrganizeLocString(string str, List<Models.Special_Values> specialValues)
        {
            try
            {
                string strText = System.Text.RegularExpressions.Regex.Replace(str, "<[^>]+>", "");
                strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
                strText = strText.Replace("\t", "");
                strText = strText.Replace("\r", "\n");
                strText = strText.TrimStart('\n').TrimEnd('\n');
                if (specialValues != null)
                {
                    foreach (var specialValue in specialValues)
                    {
                        string value = specialValue.values_float?.Length > 0 ? specialValue.values_float[0].ToString("f1") : "0";
                        string specialValuePlaceholder = "%" + specialValue.name.ToLower() + "%";
                        strText = strText.Replace(specialValuePlaceholder, value);
                        strText = strText.Replace("%%", "%");
                    }
                }
                return strText;
            }
            catch { }
            return str;
        }
    }
}
