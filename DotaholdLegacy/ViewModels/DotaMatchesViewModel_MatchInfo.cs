using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dotahold.Data.DataShop;
using Dotahold.Models;

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
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

                if (matchInfo != null && matchInfo.match_id == CurrentMatchId)
                {
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
                                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

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
                                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

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
                            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
                            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
                        catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                    }
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

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
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

                    CurrentMatchInfo = matchInfo;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

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
                                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                            }
                            foreach (var buff in player.permanent_buffs)
                            {
                                try
                                {
                                    buff?.LoadBuffImageAsync(36);
                                }
                                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                            }
                        }
                    }
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

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
                                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                                }
                                foreach (var ability in player.vAbilitiesUpgrade)
                                {
                                    try
                                    {
                                        ability?.LoadAbilityImageAsync(48);
                                    }
                                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

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
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

                    // runes
                    try
                    {
                        if (player.runes_log == null) player.runes_log = new List<Runes_Log>();
                    }
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

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
                                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                            }
                        }
                    }
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                }
                this.CurrentMatchPlayer = player;
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        #endregion

    }
}
