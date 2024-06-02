using Dotahold.Core.Models;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotahold.Models;
using Dotahold.Core.DataShop;

namespace Dotahold.ViewModels
{
    public partial class DotaMatchesViewModel : ViewModelBase
    {
        // 当前正在查看的比赛编号
        public long CurrentMatchId { get; set; } = 0;

        // 当前正在查看的比赛信息
        private DotaMatchInfoModel _CurrentMatchInfo = null;
        public DotaMatchInfoModel CurrentMatchInfo
        {
            get { return _CurrentMatchInfo; }
            set { Set("CurrentMatchInfo", ref _CurrentMatchInfo, value); }
        }

        // 当前正在查看的玩家数据
        private Player _CurrentMatchPlayer = null;
        public Player CurrentMatchPlayer
        {
            get { return _CurrentMatchPlayer; }
            set { Set("CurrentMatchPlayer", ref _CurrentMatchPlayer, value); }
        }

        // 当前比赛的占比数据对比
        public ObservableCollection<DotaMatchPerformCompareModel> CurrentMatchDamagePerformCompare { get; set; } = new ObservableCollection<DotaMatchPerformCompareModel>();
        public ObservableCollection<DotaMatchPerformCompareModel> CurrentMatchTeamfightPerformCompare { get; set; } = new ObservableCollection<DotaMatchPerformCompareModel>();

        // 请求过的比赛缓存起来
        private Dictionary<long, DotaMatchInfoModel> _matchesInfoCache = new Dictionary<long, DotaMatchInfoModel>();

        /// <summary>
        /// 加载指定比赛
        /// </summary>
        public async void GetMatchInfoAsync(long matchId)
        {
            try
            {
                if (matchId == 0 || (CurrentMatchId == matchId && CurrentMatchInfo != null)) return;

                CurrentMatchId = matchId;
                CurrentMatchInfo = null;
                bLoadingOneMatchInfo = true;

                bHaveRadiantAdv = false;
                bHavePlayersGoldSeries = false;
                bHavePlayersXpSeries = false;

                string url = string.Format("https://api.opendota.com/api/matches/{0}", matchId);    //e.g.3792271763
                DotaMatchInfoModel matchInfo = null;

                try
                {
                    if (_matchesInfoCache.ContainsKey(matchId))
                    {
                        matchInfo = _matchesInfoCache[matchId];
                    }
                    else
                    {
                        matchInfo = await GetResponseAsync<DotaMatchInfoModel>(url, _matchInfoHttpClient);
                        if (matchInfo != null)
                        {
                            _matchesInfoCache[matchId] = matchInfo;
                            if (_matchesInfoCache.Count > 100)
                            {
                                var removing = _matchesInfoCache.ElementAt(0);
                                _matchesInfoCache.Remove(removing.Key);
                            }
                        }
                    }
                    #region
                    //    "players": [
                    //      {
                    //        "match_id": 6706352286,
                    //        "player_slot": 0,
                    //        "ability_upgrades_arr": [
                    //          5162, 5163, 5163, 5162, 5163, 5165, 5163, 5162, 5162, 768, 5164, 5165,
                    //          5164, 5164, 6412, 5164, 5165, 6505
                    //        ],
                    //        "account_id": 198161112,
                    //        "additional_units": [
                    //          {
                    //            "unitname": "spirit_bear",
                    //            "item_0": 168,
                    //            "item_1": 50,
                    //            "item_2": 172,
                    //            "item_3": 116,
                    //            "item_4": 143,
                    //            "item_5": 112,
                    //            "backpack_0": 0,
                    //            "backpack_1": 0,
                    //            "backpack_2": 0,
                    //            "item_neutral": 0
                    //          }
                    //        ],
                    //        "assists": 23,
                    //        "backpack_0": 0,
                    //        "backpack_1": 188,
                    //        "backpack_2": 0,
                    //        "backpack_3": null,
                    //        "camps_stacked": 3,
                    //        "creeps_stacked": 14,
                    //        "deaths": 11,
                    //        "denies": 5,
                    //        "gold": 3322,
                    //        "gold_per_min": 373,
                    //        "gold_spent": 15625,
                    //        "gold_t": [
                    //          0, 219, 319, 454, 808, 983, 1129, 1359, 1740, 1919, 2158, 2329, 2512,
                    //          2854, 3069, 3220, 4172, 4528, 4837, 5294, 5646, 6307, 6591, 6920, 7159,
                    //          7449, 7561, 7741, 8038, 8966, 9511, 9878, 10143, 10775, 11164, 11709,
                    //          12066, 13015, 13215, 13579, 14030, 15157, 15367, 15640, 15760, 15989,
                    //          16266, 16780, 17115, 17374, 18161, 19126
                    //        ],
                    //        "hero_damage": 30193,
                    //        "hero_healing": 18628,
                    //        "hero_id": 37,
                    //        "item_0": 908,
                    //        "item_1": 190,
                    //        "item_2": 254,
                    //        "item_3": 269,
                    //        "item_4": 218,
                    //        "item_5": 29,
                    //        "item_neutral": 676,
                    //        "kills": 3,
                    //        "last_hits": 142,
                    //        "level": 24,
                    //        "net_worth": 16447,
                    //        "obs_placed": 10,
                    //        "party_id": 0,
                    //        "party_size": 10,
                    //        "permanent_buffs": [
                    //          { "permanent_buff": 6, "stack_count": 2 },
                    //          { "permanent_buff": 12, "stack_count": 0 }
                    //        ],
                    //        "pings": 34,
                    //        "purchase_log": [
                    //          { "time": -89, "key": "tango" },
                    //          { "time": -89, "key": "magic_stick" },
                    //          { "time": -89, "key": "branches" },
                    //          { "time": -89, "key": "branches" },
                    //          { "time": -89, "key": "enchanted_mango", "charges": 2 },
                    //          { "time": -89, "key": "branches" },
                    //          { "time": 4, "key": "ward_sentry" },
                    //          { "time": 4, "key": "ward_sentry" },
                    //          { "time": 225, "key": "boots" },
                    //          { "time": 226, "key": "clarity" },
                    //          { "time": 226, "key": "clarity" },
                    //          { "time": 385, "key": "sobi_mask" },
                    //          { "time": 385, "key": "ring_of_basilius" },
                    //          { "time": 387, "key": "ward_sentry" },
                    //          { "time": 483, "key": "ring_of_protection" },
                    //          { "time": 483, "key": "buckler" },
                    //          { "time": 609, "key": "tpscroll" },
                    //          { "time": 611, "key": "magic_wand" },
                    //          { "time": 613, "key": "ward_observer" },
                    //          { "time": 613, "key": "ward_sentry" },
                    //          { "time": 614, "key": "smoke_of_deceit" },
                    //          { "time": 720, "key": "blades_of_attack" },
                    //          { "time": 813, "key": "tpscroll" },
                    //          { "time": 857, "key": "ward_observer" },
                    //          { "time": 857, "key": "ward_observer" },
                    //          { "time": 857, "key": "ward_sentry" },
                    //          { "time": 857, "key": "ward_sentry" },
                    //          { "time": 954, "key": "lifesteal" },
                    //          { "time": 976, "key": "vladmir" },
                    //          { "time": 992, "key": "tpscroll" },
                    //          { "time": 1035, "key": "ward_sentry" },
                    //          { "time": 1035, "key": "ward_sentry" },
                    //          { "time": 1100, "key": "tpscroll" },
                    //          { "time": 1102, "key": "smoke_of_deceit" },
                    //          { "time": 1102, "key": "ward_sentry" },
                    //          { "time": 1205, "key": "tpscroll" },
                    //          { "time": 1213, "key": "tome_of_knowledge" },
                    //          { "time": 1237, "key": "wraith_pact" },
                    //          { "time": 1237, "key": "point_booster" },
                    //          { "time": 1317, "key": "ward_sentry" },
                    //          { "time": 1317, "key": "ward_observer" },
                    //          { "time": 1317, "key": "dust" },
                    //          { "time": 1318, "key": "smoke_of_deceit" },
                    //          { "time": 1318, "key": "ward_sentry" },
                    //          { "time": 1420, "key": "ward_observer" },
                    //          { "time": 1420, "key": "ward_sentry" },
                    //          { "time": 1666, "key": "aghanims_shard" },
                    //          { "time": 1672, "key": "ward_observer" },
                    //          { "time": 1672, "key": "ward_sentry" },
                    //          { "time": 1672, "key": "ward_sentry" },
                    //          { "time": 1752, "key": "ward_sentry" },
                    //          { "time": 1752, "key": "dust" },
                    //          { "time": 1753, "key": "smoke_of_deceit" },
                    //          { "time": 1753, "key": "ward_sentry" },
                    //          { "time": 1857, "key": "tome_of_knowledge" },
                    //          { "time": 1891, "key": "sobi_mask" },
                    //          { "time": 1891, "key": "ring_of_basilius" },
                    //          { "time": 1892, "key": "crown" },
                    //          { "time": 1901, "key": "veil_of_discord" },
                    //          { "time": 1985, "key": "ward_observer" },
                    //          { "time": 1986, "key": "ward_sentry" },
                    //          { "time": 2225, "key": "shadow_amulet" },
                    //          { "time": 2225, "key": "cloak" },
                    //          { "time": 2225, "key": "glimmer_cape" },
                    //          { "time": 2234, "key": "ward_sentry" },
                    //          { "time": 2340, "key": "ward_sentry" },
                    //          { "time": 2340, "key": "ward_observer" },
                    //          { "time": 2341, "key": "ward_sentry" },
                    //          { "time": 2502, "key": "ward_observer" },
                    //          { "time": 2503, "key": "ward_sentry" },
                    //          { "time": 2503, "key": "ward_sentry" },
                    //          { "time": 2541, "key": "ring_of_regen" },
                    //          { "time": 2541, "key": "headdress" },
                    //          { "time": 2541, "key": "fluffy_hat" },
                    //          { "time": 2549, "key": "energy_booster" },
                    //          { "time": 2553, "key": "holy_locket" },
                    //          { "time": 2715, "key": "ward_observer" },
                    //          { "time": 2715, "key": "ward_sentry" },
                    //          { "time": 2926, "key": "ward_observer" },
                    //          { "time": 2926, "key": "ward_observer" },
                    //          { "time": 2926, "key": "ward_sentry" },
                    //          { "time": 2926, "key": "ward_sentry" },
                    //          { "time": 2927, "key": "ward_sentry" },
                    //          { "time": 2927, "key": "ward_sentry" },
                    //          { "time": 2927, "key": "ward_sentry" }
                    //        ],
                    //        "randomed": false,
                    //        "roshans_killed": 0,
                    //        "runes_log": [
                    //          { "time": 0, "key": 5 },
                    //          { "time": 361, "key": 5 },
                    //          { "time": 1468, "key": 5 },
                    //          { "time": 2708, "key": 5 }
                    //        ],
                    //        "tower_damage": 932,
                    //        "towers_killed": 1,
                    //        "xp_per_min": 543,
                    //        "xp_t": [
                    //          0, 118, 356, 444, 642, 866, 991, 1121, 1410, 1751, 2291, 2555, 2932,
                    //          3177, 3692, 4052, 4492, 6046, 6876, 7595, 7959, 8878, 9125, 9410, 9526,
                    //          10159, 10209, 10594, 10813, 12551, 13163, 13379, 14755, 14831, 15130,
                    //          16991, 18015, 18087, 18839, 19448, 20880, 22020, 22102, 22423, 22890,
                    //          23326, 23708, 24192, 25297, 25453, 25551, 27835
                    //        ],
                    //        "personaname": "euphoria",
                    //        "name": "detachment",
                    //        "radiant_win": false,
                    //        "isRadiant": true,
                    //        "total_gold": 19184,
                    //        "total_xp": 27928,
                    //        "neutral_kills": 26,
                    //        "tower_kills": 1,
                    //        "courier_kills": 0,
                    //        "observer_kills": 3,
                    //        "sentry_kills": 3,
                    //        "roshan_kills": 0,
                    //        "buyback_count": 0,
                    //        "rank_tier": 80,
                    //        "benchmarks": {
                    //          "gold_per_min": { "raw": 373, "pct": 0.8095238095238095 },
                    //          "xp_per_min": { "raw": 543, "pct": 0.8571428571428571 },
                    //          "kills_per_min": {
                    //            "raw": 0.05832793259883344,
                    //            "pct": 0.19047619047619047
                    //          },
                    //          "last_hits_per_min": {
                    //            "raw": 2.7608554763447826,
                    //            "pct": 0.8571428571428571
                    //          },
                    //          "hero_damage_per_min": {
                    //            "raw": 587.0317563188594,
                    //            "pct": 0.9047619047619048
                    //          },
                    //          "hero_healing_per_min": {
                    //            "raw": 362.17757615035646,
                    //            "pct": 0.8095238095238095
                    //          },
                    //          "tower_damage": { "raw": 932, "pct": 0.5714285714285714 },
                    //          "stuns_per_min": {
                    //            "raw": 0.33382604018146467,
                    //            "pct": 0.47619047619047616
                    //          },
                    //          "lhten": { "raw": 12, "pct": 0.8571428571428571 }
                    //        }
                    //      }
                    //    ]
                    //}
                    #endregion
                }
                catch { }

                if (matchInfo != null && matchInfo.match_id == CurrentMatchId)
                {
                    // BanPick列表
                    try
                    {
                        if (matchInfo.picks_bans == null)
                        {
                            matchInfo.picks_bans = new List<Picks_Bans>();
                        }
                        foreach (var bp in matchInfo.picks_bans)
                        {
                            if (DotaHeroesViewModel.Instance.dictAllHeroes?.ContainsKey(bp.hero_id.ToString()) == true)
                            {
                                bp.sHeroImage = DotaHeroesViewModel.Instance.dictAllHeroes[bp.hero_id.ToString()].img;
                                bp.sHeroName = DotaHeroesViewModel.Instance.dictAllHeroes[bp.hero_id.ToString()].localized_name;
                                await bp.LoadImageAsync(86);
                            }
                        }
                    }
                    catch { }

                    // 玩家列表
                    try
                    {
                        // 用于整理玩家开黑编号
                        Dictionary<long, int> playersPartyDict = new Dictionary<long, int>();
                        int partyId = 0;

                        List<Player> leftPlayers = new List<Player>();
                        List<Player> rightPlayers = new List<Player>();

                        for (int i = 0; i < matchInfo.players.Count; i++)
                        {
                            try
                            {
                                var player = matchInfo.players[i];

                                // 玩家使用的英雄
                                try
                                {
                                    if (DotaHeroesViewModel.Instance.dictAllHeroes?.ContainsKey(player.hero_id.ToString()) == true)
                                    {
                                        player.sHeroName = DotaHeroesViewModel.Instance.dictAllHeroes[player.hero_id.ToString()].localized_name;
                                        player.sHeroImage = DotaHeroesViewModel.Instance.dictAllHeroes[player.hero_id.ToString()].img;
                                    }
                                }
                                catch { }

                                // 玩家的物品
                                try
                                {
                                    player.sItem0 = GetItemImgById(player.item_0?.ToString());
                                    player.sItem1 = GetItemImgById(player.item_1?.ToString());
                                    player.sItem2 = GetItemImgById(player.item_2?.ToString());
                                    player.sItem3 = GetItemImgById(player.item_3?.ToString());
                                    player.sItem4 = GetItemImgById(player.item_4?.ToString());
                                    player.sItem5 = GetItemImgById(player.item_5?.ToString());
                                    player.sItemB0 = GetItemImgById(player.backpack_0?.ToString());
                                    player.sItemB1 = GetItemImgById(player.backpack_1?.ToString());
                                    player.sItemB2 = GetItemImgById(player.backpack_2?.ToString());
                                    player.sItemN = GetItemImgById(player.item_neutral?.ToString());
                                    player.sNameItem0 = GetItemNameById(player.item_0?.ToString());
                                    player.sNameItem1 = GetItemNameById(player.item_1?.ToString());
                                    player.sNameItem2 = GetItemNameById(player.item_2?.ToString());
                                    player.sNameItem3 = GetItemNameById(player.item_3?.ToString());
                                    player.sNameItem4 = GetItemNameById(player.item_4?.ToString());
                                    player.sNameItem5 = GetItemNameById(player.item_5?.ToString());
                                    player.sNameItemB0 = GetItemNameById(player.backpack_0?.ToString());
                                    player.sNameItemB1 = GetItemNameById(player.backpack_1?.ToString());
                                    player.sNameItemB2 = GetItemNameById(player.backpack_2?.ToString());
                                    player.sNameItemN = GetItemNameById(player.item_neutral?.ToString());
                                }
                                catch { }

                                // 判断是天辉还是夜魇
                                try
                                {
                                    if (player.isRadiant == null)
                                    {
                                        player.isRadiant = player.player_slot < 128 ? true : false;
                                    }
                                }
                                catch { }

                                // 判断是否是当前绑定账号的玩家
                                try
                                {
                                    if (player.account_id.ToString() == sSteamId)
                                    {
                                        player.bIsCurrentPlayer = true;
                                    }
                                }
                                catch { }

                                // 神杖和魔晶
                                try
                                {
                                    player.bHaveAghanimScepter = false;
                                    player.bHaveAghanimShard = false;
                                    if (player?.permanent_buffs != null)
                                    {
                                        foreach (var buff in player.permanent_buffs)
                                        {
                                            if (buff.permanent_buff == 2)
                                            {
                                                player.bHaveAghanimScepter = true;
                                            }
                                            if (buff.permanent_buff == 12)
                                            {
                                                player.bHaveAghanimShard = true;
                                            }
                                        }
                                    }
                                }
                                catch { }

                                // 玩家的开黑编号
                                try
                                {
                                    player.iPartyId = 0;
                                    if (player.party_id != null && player.party_size != null && player.party_size > 1 && player.party_size < 10)
                                    {
                                        long id = (long)player.party_id;
                                        if (!playersPartyDict.ContainsKey(id))
                                        {
                                            partyId++;
                                            playersPartyDict.Add(id, partyId);
                                        }
                                        player.iPartyId = playersPartyDict[id];
                                    }
                                }
                                catch { }

                                // 额外单位，目前只处理德鲁伊(hero_id=80)的熊灵
                                try
                                {
                                    player.SpiritBear = null;
                                    if (player.hero_id == 80 && player.additional_units != null && player.additional_units.Length > 0)
                                    {
                                        foreach (var item in player.additional_units)
                                        {
                                            if (item.unitname.Contains("spirit_bear"))
                                            {
                                                player.SpiritBear = item;
                                                break;
                                            }
                                        }

                                        if (player.SpiritBear != null)
                                        {
                                            player.SpiritBear.sItem0 = GetItemImgById(player.SpiritBear.item_0?.ToString());
                                            player.SpiritBear.sItem1 = GetItemImgById(player.SpiritBear.item_1?.ToString());
                                            player.SpiritBear.sItem2 = GetItemImgById(player.SpiritBear.item_2?.ToString());
                                            player.SpiritBear.sItem3 = GetItemImgById(player.SpiritBear.item_3?.ToString());
                                            player.SpiritBear.sItem4 = GetItemImgById(player.SpiritBear.item_4?.ToString());
                                            player.SpiritBear.sItem5 = GetItemImgById(player.SpiritBear.item_5?.ToString());
                                            player.SpiritBear.sItemB0 = GetItemImgById(player.SpiritBear.backpack_0?.ToString());
                                            player.SpiritBear.sItemB1 = GetItemImgById(player.SpiritBear.backpack_1?.ToString());
                                            player.SpiritBear.sItemB2 = GetItemImgById(player.SpiritBear.backpack_2?.ToString());
                                            player.SpiritBear.sItemN = GetItemImgById(player.SpiritBear.item_neutral?.ToString());
                                            player.SpiritBear.sNameItem0 = GetItemNameById(player.SpiritBear.item_0?.ToString());
                                            player.SpiritBear.sNameItem1 = GetItemNameById(player.SpiritBear.item_1?.ToString());
                                            player.SpiritBear.sNameItem2 = GetItemNameById(player.SpiritBear.item_2?.ToString());
                                            player.SpiritBear.sNameItem3 = GetItemNameById(player.SpiritBear.item_3?.ToString());
                                            player.SpiritBear.sNameItem4 = GetItemNameById(player.SpiritBear.item_4?.ToString());
                                            player.SpiritBear.sNameItem5 = GetItemNameById(player.SpiritBear.item_5?.ToString());
                                            player.SpiritBear.sNameItemB0 = GetItemNameById(player.SpiritBear.backpack_0?.ToString());
                                            player.SpiritBear.sNameItemB1 = GetItemNameById(player.SpiritBear.backpack_1?.ToString());
                                            player.SpiritBear.sNameItemB2 = GetItemNameById(player.SpiritBear.backpack_2?.ToString());
                                            player.SpiritBear.sNameItemN = GetItemNameById(player.SpiritBear.item_neutral?.ToString());
                                        }
                                    }
                                }
                                catch { }

                                // 处理表现对比
                                if (player.isRadiant != false)
                                {
                                    leftPlayers.Add(player);
                                }
                                else
                                {
                                    rightPlayers.Add(player);
                                }
                            }
                            catch { }
                        }
                        foreach (var player in matchInfo.players)
                        {
                            try
                            {
                                await player.LoadImageAsync(86);
                                await player.LoadItemsImageAsync(44, 35, 134);
                                if (player.SpiritBear != null)
                                {
                                    await player.SpiritBear?.LoadItemsImageAsync(44, 35, 134);
                                }
                            }
                            catch { }
                        }

                        // 玩家表现对比
                        try
                        {
                            CurrentMatchDamagePerformCompare.Clear();
                            CurrentMatchTeamfightPerformCompare.Clear();

                            int count = Math.Max(leftPlayers.Count, rightPlayers.Count);

                            // 双方阵营伤害和击杀总和
                            double leftTotalDamage = 0;
                            double rightTotalDamage = 0;
                            double leftTotalTeamfight = 0;
                            double rightTotalTeamfight = 0;
                            if (leftPlayers != null)
                            {
                                foreach (var item in leftPlayers)
                                {
                                    leftTotalDamage += item.hero_damage ?? 0;
                                    leftTotalTeamfight += item.kills ?? 0;
                                }
                            }
                            if (rightPlayers != null)
                            {
                                foreach (var item in rightPlayers)
                                {
                                    rightTotalDamage += item.hero_damage ?? 0;
                                    rightTotalTeamfight += item.kills ?? 0;
                                }
                            }

                            // 输出率计算
                            leftPlayers = leftPlayers.OrderByDescending(p => p?.hero_damage ?? 0).ToList();
                            rightPlayers = rightPlayers.OrderByDescending(p => p?.hero_damage ?? 0).ToList();
                            for (int i = 0; i < count; i++)
                            {
                                Player leftPlayer = leftPlayers.Count > i ? leftPlayers[i] : null;
                                Player rightPlayer = rightPlayers.Count > i ? rightPlayers[i] : null;
                                DotaMatchPerformCompareModel compare = new DotaMatchPerformCompareModel();
                                if (leftPlayer != null)
                                {
                                    compare.LeftValue = leftTotalDamage > 0 ? ((leftPlayer.hero_damage ?? 0) / leftTotalDamage) : 0;
                                    compare.LeftPlayerSlot = leftPlayer.player_slot;
                                    compare.LeftImageSource = leftPlayer.ImageSource;

                                    compare.LeftValue = (Math.Floor(1000 * compare.LeftValue) / 10);
                                }
                                if (rightPlayer != null)
                                {
                                    compare.RightValue = rightTotalDamage > 0 ? ((rightPlayer.hero_damage ?? 0) / rightTotalDamage) : 0;
                                    compare.RightPlayerSlot = rightPlayer.player_slot;
                                    compare.RightImageSource = rightPlayer.ImageSource;

                                    compare.RightValue = Math.Floor(1000 * compare.RightValue) / 10;
                                }
                                CurrentMatchDamagePerformCompare.Add(compare);
                            }

                            // 团战率计算
                            leftPlayers = leftPlayers.OrderByDescending(p => (p?.kills ?? 0) + (p?.assists ?? 0)).ToList();
                            rightPlayers = rightPlayers.OrderByDescending(p => (p?.kills ?? 0) + (p?.assists ?? 0)).ToList();
                            for (int i = 0; i < count; i++)
                            {
                                Player leftPlayer = leftPlayers.Count > i ? leftPlayers[i] : null;
                                Player rightPlayer = rightPlayers.Count > i ? rightPlayers[i] : null;
                                DotaMatchPerformCompareModel compare = new DotaMatchPerformCompareModel();
                                if (leftPlayer != null)
                                {
                                    compare.LeftValue = leftTotalTeamfight > 0 ? (((leftPlayer.kills ?? 0) + (leftPlayer.assists ?? 0)) / leftTotalTeamfight) : 0;
                                    compare.LeftPlayerSlot = leftPlayer.player_slot;
                                    compare.LeftImageSource = leftPlayer.ImageSource;

                                    compare.LeftValue = (Math.Floor(1000 * compare.LeftValue) / 10);
                                }
                                if (rightPlayer != null)
                                {
                                    compare.RightValue = rightTotalTeamfight > 0 ? (((rightPlayer.kills ?? 0) + (rightPlayer.assists ?? 0)) / rightTotalTeamfight) : 0;
                                    compare.RightPlayerSlot = rightPlayer.player_slot;
                                    compare.RightImageSource = rightPlayer.ImageSource;

                                    compare.RightValue = Math.Floor(1000 * compare.RightValue) / 10;
                                }
                                CurrentMatchTeamfightPerformCompare.Add(compare);
                            }
                        }
                        catch { }
                    }
                    catch { }

                    // 双方职业战队
                    try
                    {
                        if (matchInfo.radiant_team != null)
                        {
                            if (string.IsNullOrEmpty(matchInfo.radiant_team.name))
                            {
                                matchInfo.radiant_team.name = matchInfo.radiant_team.team_id;
                            }
                        }
                        if (matchInfo.dire_team != null)
                        {
                            if (string.IsNullOrEmpty(matchInfo.dire_team.name))
                            {
                                matchInfo.dire_team.name = matchInfo.dire_team.team_id;
                            }
                        }
                    }
                    catch { }

                    CurrentMatchInfo = matchInfo;
                }
            }
            catch { }
            finally { bLoadingOneMatchInfo = false; }
        }

        /// <summary>
        /// 分析当前选择玩家的数据
        /// </summary>
        /// <returns></returns>
        public async void AnalyzePlayerInfo(Models.Player player)
        {
            try
            {
                if (player != null)
                {
                    // kda
                    try
                    {
                        if (string.IsNullOrEmpty(player.sKDA))
                        {
                            double ka = (player.kills ?? 0) + (player.assists ?? 0);
                            double d = ((player.deaths ?? 0) <= 0) ? 1.0 : (double)player.deaths;
                            double kda = ka / d;
                            player.sKDA = (Math.Floor(100 * kda) / 100).ToString("f2");
                        }
                    }
                    catch { }

                    // buffs
                    try
                    {
                        if (player.permanent_buffs == null) player.permanent_buffs = new List<Permanent_Buffs>();

                        if (player.permanent_buffs.Count > 0)
                        {
                            Dictionary<string, string> dictBuffs = await ConstantsCourier.Instance.GetPermanentBuffsConstant();
                            foreach (var buff in player.permanent_buffs)
                            {
                                try
                                {
                                    if (buff == null) continue;

                                    if (buff?.permanent_buff != null && dictBuffs.ContainsKey(buff.permanent_buff.ToString()))
                                    {
                                        buff.sBuff = dictBuffs[buff.permanent_buff.ToString()];
                                    }
                                    else
                                    {
                                        // 字典里没有这个buff，则显示默认图
                                        buff.permanent_buff = -99;
                                        buff.sBuff = "buff_placeholder";
                                    }
                                }
                                catch { }
                            }
                            foreach (var buff in player.permanent_buffs)
                            {
                                try
                                {
                                    buff?.LoadBuffImageAsync(36);
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }

                    // abilities
                    try
                    {
                        if (player.vAbilitiesUpgrade == null) player.vAbilitiesUpgrade = new ObservableCollection<AbilityUpgrade>();

                        if (player.vAbilitiesUpgrade.Count <= 0)
                        {
                            if (player.ability_upgrades_arr != null && player.ability_upgrades_arr.Count > 0)
                            {
                                player.vAbilitiesUpgrade.Clear();
                                Dictionary<string, string> dictAbilities = await ConstantsCourier.Instance.GetAbilityIDsConstant();
                                foreach (var ability in player.ability_upgrades_arr)
                                {
                                    try
                                    {
                                        if (ability != null && dictAbilities.ContainsKey(ability.ToString()))
                                        {
                                            var abilityUp = new AbilityUpgrade();
                                            string abiName = dictAbilities[ability.ToString()];

                                            if (abiName.StartsWith("special_bonus_"))
                                                abilityUp.bIsTalent = true;
                                            else
                                                abilityUp.sAbilityUrl = string.Format("https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/abilities/{0}.png", abiName);

                                            abilityUp.sAbilityName = abiName.Replace('_', ' ').ToUpper();

                                            player.vAbilitiesUpgrade.Add(abilityUp);
                                        }
                                    }
                                    catch { }
                                }
                                foreach (var ability in player.vAbilitiesUpgrade)
                                {
                                    try
                                    {
                                        ability?.LoadAbilityImageAsync(48);
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch { }

                    // benchmarks
                    try
                    {
                        if (player.vBenchmarks == null) player.vBenchmarks = new ObservableCollection<Benchmark>();

                        var benchmarks = player.benchmarks;
                        if (benchmarks != null && benchmarks.Count > 0 && player.vBenchmarks.Count <= 0)
                        {
                            player.vBenchmarks.Clear();
                            foreach (var benchmark in benchmarks)
                            {
                                if (benchmark.Value == null)
                                    continue;

                                string name = benchmark.Key;
                                StringBuilder nameSb = new StringBuilder();
                                name = name.Replace('_', ' ');
                                name = name.ToUpper();
                                benchmark.Value.Name = name;

                                benchmark.Value.raw = Math.Floor(benchmark.Value.raw * 100) / 100;
                                benchmark.Value.pct = Math.Floor(benchmark.Value.pct * 100);

                                benchmark.Value.BarWidth = 1.2/*进度条长度120*/* benchmark.Value.pct;
                                if (benchmark.Value.BarWidth < 0) benchmark.Value.BarWidth = 0;
                                if (benchmark.Value.BarWidth > 120) benchmark.Value.BarWidth = 120;

                                player.vBenchmarks.Add(benchmark.Value);
                            }
                        }
                    }
                    catch { }

                    // runes
                    try
                    {
                        if (player.runes_log == null) player.runes_log = new List<Runes_Log>();
                    }
                    catch { }

                    // purchase log
                    try
                    {
                        if (player.vPurchaseLog == null) player.vPurchaseLog = new ObservableCollection<Purchase_Log>();

                        var purchaseLog = player.purchase_log;
                        if (purchaseLog != null && purchaseLog.Count > 0 && player.vPurchaseLog.Count <= 0)
                        {
                            player.vPurchaseLog.Clear();
                            foreach (var purchase in purchaseLog)
                            {
                                if (purchase == null)
                                    continue;

                                int time = 0;
                                if (purchase.time != null && purchase.time > 0) time = purchase.time ?? 0;
                                purchase.PurchaseTime = (time / 60) + ":" + (time % 60).ToString("00");

                                purchase.ItemCharges = purchase.charges == null ? string.Empty : purchase.charges?.ToString();

                                string itemUrl = GetItemImgByName(purchase.key);
                                purchase.key = itemUrl;

                                player.vPurchaseLog.Add(purchase);
                            }
                            foreach (var purchase in player.vPurchaseLog)
                            {
                                try
                                {
                                    purchase?.LoadImageAsync(44);
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
                this.CurrentMatchPlayer = player;
            }
            catch { }
        }

        #region 折线图

        // 天辉优势走势图(金钱和经验)
        private LiveChartsCore.ISeries[] _RadiantAdvantageSeries = new LiveChartsCore.ISeries[]
        {
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                GeometryStroke = new SolidColorPaint(SKColors.Gold, 2),
                GeometrySize = 2,
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.Gold, 2),
                Name = "Radiant Gold Adv"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                GeometryStroke = new SolidColorPaint(SKColors.MediumOrchid, 2),
                GeometrySize = 2,
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.MediumOrchid, 2),
                Name = "Radiant XP Adv"
            }
        };
        public LiveChartsCore.ISeries[] RadiantAdvantageSeries
        {
            get { return _RadiantAdvantageSeries; }
            set { Set("RadiantAdvantageSeries", ref _RadiantAdvantageSeries, value); }
        }

        // 玩家经济走势
        private LiveChartsCore.ISeries[] _PlayersGoldSeries = new LiveChartsCore.ISeries[]
        {
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(51,117,255,255), 2),
                Name = "Player1"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(102,255,191,255), 2),
                Name = "Player2"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(191,0,191,255), 2),
                Name = "Player3"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(243,240,11,255), 2),
                Name = "Player4"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(255,107,0,255), 2),
                Name = "Player5"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(254,134,194,255), 2),
                Name = "Player6"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(161,180,71,255), 2),
                Name = "Player7"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(101,217,247,255), 2),
                Name = "Player8"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(0,131,33,255), 2),
                Name = "Player9"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(164,105,0,255), 2),
                Name = "Player10"
            },
        };
        public LiveChartsCore.ISeries[] PlayersGoldSeries
        {
            get { return _PlayersGoldSeries; }
            set { Set("PlayersGoldSeries", ref _PlayersGoldSeries, value); }
        }

        // 玩家经验走势
        private LiveChartsCore.ISeries[] _PlayersXpSeries = new LiveChartsCore.ISeries[]
        {
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(51,117,255,255), 2),
                Name = "Player1"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(102,255,191,255), 2),
                Name = "Player2"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(191,0,191,255), 2),
                Name = "Player3"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(243,240,11,255), 2),
                Name = "Player4"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(255,107,0,255), 2),
                Name = "Player5"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(254,134,194,255), 2),
                Name = "Player6"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(161,180,71,255), 2),
                Name = "Player7"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(101,217,247,255), 2),
                Name = "Player8"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(0,131,33,255), 2),
                Name = "Player9"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(164,105,0,255), 2),
                Name = "Player10"
            },
        };
        public LiveChartsCore.ISeries[] PlayersXpSeries
        {
            get { return _PlayersXpSeries; }
            set { Set("PlayersXpSeries", ref _PlayersXpSeries, value); }
        }

        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Gray),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = SKColors.LightGray,
                    StrokeThickness = 2
                }
            }
        };

        public Axis[] YAxes { get; set; } =
        {
            new Axis
            {
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Gray),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = SKColors.LightGray,
                    StrokeThickness = 2,
                    PathEffect = new LiveChartsCore.SkiaSharpView.Painting.Effects.DashEffect(new float[] { 3, 3 })
                }
            }
        };

        public RectangularSection[] RadiantAdvSections { get; set; } =
        {
            new RectangularSection
            {
                Yi = 0,
                Yj = 500000,
                Fill = new SolidColorPaint
                {
                    Color = new SKColor(68, 112, 78, 255)
                }
            },
            new RectangularSection
            {
                Yi = 0,
                Yj = -500000,
                Fill = new SolidColorPaint
                {
                    Color = new SKColor(145, 56, 63, 255)
                }
            }
        };

        public RectangularSection[] PlayersGoldXPSections { get; set; } =
        {
            new RectangularSection
            {
                Yi = 0,
                Yj = 500000,
                Fill = new SolidColorPaint
                {
                    Color = new SKColor(34, 35, 50, 180)
                }
            },
            new RectangularSection
            {
                Yi = 0,
                Yj = -5000,
                Fill = new SolidColorPaint
                {
                    Color = new SKColor(34, 35, 50, 180)
                }
            }
        };

        public DrawMarginFrame Frame { get; set; } = new DrawMarginFrame()
        {
            Fill = new SolidColorPaint
            {
                Color = new SKColor(0, 0, 0, 30)
            },
            Stroke = new SolidColorPaint
            {
                Color = new SKColor(80, 80, 80),
                StrokeThickness = 2
            }
        };

        // 是否有天辉优势走势图数据
        private bool _bHaveRadiantAdv = false;
        public bool bHaveRadiantAdv
        {
            get { return _bHaveRadiantAdv; }
            set { Set("bHaveRadiantAdv", ref _bHaveRadiantAdv, value); }
        }

        // 是否有玩家Gold走势
        private bool _bHavePlayersGoldSeries = false;
        public bool bHavePlayersGoldSeries
        {
            get { return _bHavePlayersGoldSeries; }
            set { Set("bHavePlayersGoldSeries", ref _bHavePlayersGoldSeries, value); }
        }

        // 是否有玩家Xp走势
        private bool _bHavePlayersXpSeries = false;
        public bool bHavePlayersXpSeries
        {
            get { return _bHavePlayersXpSeries; }
            set { Set("bHavePlayersXpSeries", ref _bHavePlayersXpSeries, value); }
        }

        /// <summary>
        /// 生成天辉优势图
        /// </summary>
        public void LoadRadiantAdvSeries()
        {
            try
            {
                if (this.CurrentMatchInfo?.radiant_gold_adv != null && this.CurrentMatchInfo?.radiant_xp_adv != null)
                {
                    if (RadiantAdvantageSeries[0].Values is ObservableCollection<double> v1 && v1 != null)
                    {
                        v1.Clear();
                        foreach (var item in this.CurrentMatchInfo.radiant_gold_adv)
                        {
                            v1.Add(item);
                        }
                    }
                    else
                    {
                        RadiantAdvantageSeries[0].Values = new ObservableCollection<double>(this.CurrentMatchInfo.radiant_gold_adv);
                    }

                    if (RadiantAdvantageSeries[1].Values is ObservableCollection<double> v2 && v2 != null)
                    {
                        v2.Clear();
                        foreach (var item in this.CurrentMatchInfo.radiant_xp_adv)
                        {
                            v2.Add(item);
                        }
                    }
                    else
                    {
                        RadiantAdvantageSeries[1].Values = new ObservableCollection<double>(this.CurrentMatchInfo.radiant_xp_adv);
                    }

                    bHaveRadiantAdv = true;
                }
                else
                {
                    if (RadiantAdvantageSeries[0].Values is ObservableCollection<double> v1)
                    {
                        v1?.Clear();
                    }

                    if (RadiantAdvantageSeries[1].Values is ObservableCollection<double> v2)
                    {
                        v2?.Clear();
                    }

                    bHaveRadiantAdv = false;
                }
            }
            catch { }
        }

        /// <summary>
        /// 生成玩家经济走势图
        /// </summary>
        public void LoadPlayersGoldSeries()
        {
            try
            {
                if (this.CurrentMatchInfo?.players != null && this.CurrentMatchInfo?.players.Count > 0)
                {
                    bool havePlayersSeries = false;
                    for (int i = 0; i < this.CurrentMatchInfo.players.Count && i < PlayersGoldSeries.Length; i++)
                    {
                        var player = this.CurrentMatchInfo.players[i];
                        if (player == null) continue;

                        List<double> list = player.gold_t;

                        if (PlayersGoldSeries[i].Values is ObservableCollection<double> v && v != null)
                        {
                            v.Clear();
                            if (list != null && list.Count > 0)
                            {
                                foreach (var item in list)
                                {
                                    v.Add(item);
                                }
                                havePlayersSeries = true;
                            }
                        }
                        else
                        {
                            if (list != null && list.Count > 0)
                            {
                                PlayersGoldSeries[i].Values = new ObservableCollection<double>(list);
                                havePlayersSeries = true;
                            }
                            else
                            {
                                PlayersGoldSeries[i].Values = new ObservableCollection<double>();
                            }
                        }
                        PlayersGoldSeries[i].Name = player.sHeroName;
                    }

                    bHavePlayersGoldSeries = havePlayersSeries;
                }
                else
                {
                    bHavePlayersGoldSeries = false;

                    foreach (var item in PlayersGoldSeries)
                    {
                        if (item.Values is ObservableCollection<double> v)
                        {
                            v?.Clear();
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 生成玩家经验走势图
        /// </summary>
        public void LoadPlayersXpSeries()
        {
            try
            {
                if (this.CurrentMatchInfo?.players != null && this.CurrentMatchInfo?.players.Count > 0)
                {
                    bool havePlayersSeries = false;
                    for (int i = 0; i < this.CurrentMatchInfo.players.Count && i < PlayersXpSeries.Length; i++)
                    {
                        var player = this.CurrentMatchInfo.players[i];
                        if (player == null) continue;

                        List<double> list = player.xp_t;

                        if (PlayersXpSeries[i].Values is ObservableCollection<double> v && v != null)
                        {
                            v.Clear();
                            if (list != null && list.Count > 0)
                            {
                                foreach (var item in list)
                                {
                                    v.Add(item);
                                }
                                havePlayersSeries = true;
                            }
                        }
                        else
                        {
                            if (list != null && list.Count > 0)
                            {
                                PlayersXpSeries[i].Values = new ObservableCollection<double>(list);
                                havePlayersSeries = true;
                            }
                            else
                            {
                                PlayersXpSeries[i].Values = new ObservableCollection<double>();
                            }
                        }
                        PlayersXpSeries[i].Name = player.sHeroName;
                    }

                    bHavePlayersXpSeries = havePlayersSeries;
                }
                else
                {
                    bHavePlayersXpSeries = false;

                    foreach (var item in PlayersXpSeries)
                    {
                        if (item.Values is ObservableCollection<double> v)
                        {
                            v?.Clear();
                        }
                    }
                }
            }
            catch { }
        }

        #endregion

    }
}
