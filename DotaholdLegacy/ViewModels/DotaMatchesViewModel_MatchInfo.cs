namespace Dotahold.ViewModels
{
    public partial class DotaMatchesViewModel : ViewModelBase
    {
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

                            }
                            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                        }
                    }
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
    }
}
