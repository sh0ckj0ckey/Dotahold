using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class Player : ViewModelBase
    {
        public long? match_id { get; set; }
        public int? player_slot { get; set; }
        public List<int?> ability_upgrades_arr { get; set; }
        public long? account_id { get; set; }
        public Additional_Units[] additional_units { get; set; }
        public int? assists { get; set; }
        public int? backpack_0 { get; set; }
        public int? backpack_1 { get; set; }
        public int? backpack_2 { get; set; }
        public int? backpack_3 { get; set; }
        public int? camps_stacked { get; set; }
        public int? creeps_stacked { get; set; }
        public int? deaths { get; set; }
        public int? denies { get; set; }
        //public int? gold { get; set; }
        public int? gold_per_min { get; set; }
        public int? gold_spent { get; set; }
        public List<double> gold_t { get; set; }
        public int? hero_damage { get; set; }
        public int? hero_healing { get; set; }
        public int? hero_id { get; set; }
        public int? item_0 { get; set; }
        public int? item_1 { get; set; }
        public int? item_2 { get; set; }
        public int? item_3 { get; set; }
        public int? item_4 { get; set; }
        public int? item_5 { get; set; }
        public int? item_neutral { get; set; }
        public int? kills { get; set; }
        public int? last_hits { get; set; }
        public int? level { get; set; }
        public int? net_worth { get; set; }
        public int? obs_placed { get; set; } = 0;
        public int? sen_placed { get; set; } = 0;
        public long? party_id { get; set; }
        public int? party_size { get; set; }
        public List<Permanent_Buffs> permanent_buffs { get; set; }
        public int? pings { get; set; }
        public List<Purchase_Log> purchase_log { get; set; }
        //public int? roshans_killed { get; set; }
        public List<Runes_Log> runes_log { get; set; }
        public int? tower_damage { get; set; }
        //public int? towers_killed { get; set; }
        public int? xp_per_min { get; set; }
        public List<double> xp_t { get; set; }
        public string personaname { get; set; }
        public string name { get; set; }
        public bool radiant_win { get; set; }
        public int? total_gold { get; set; }
        public int? total_xp { get; set; }
        public int? neutral_kills { get; set; }
        public int? tower_kills { get; set; }
        public int? courier_kills { get; set; }
        public int? observer_kills { get; set; } = 0;
        public int? sentry_kills { get; set; } = 0;
        public int? roshan_kills { get; set; }
        public int? buyback_count { get; set; }
        public int? rank_tier { get; set; }
        public bool? isRadiant { get; set; } = null;
        public bool? randomed { get; set; }
        public int? purchase_tpscroll { get; set; } = null;
        public Dictionary<string, Benchmark> benchmarks { get; set; }

        // 物品购买记录
        [JsonIgnore]
        private ObservableCollection<Purchase_Log> _vPurchaseLog = null;
        [JsonIgnore]
        public ObservableCollection<Purchase_Log> vPurchaseLog
        {
            get { return _vPurchaseLog; }
            set { Set("vPurchaseLog", ref _vPurchaseLog, value); }
        }

        // 数据排行
        [JsonIgnore]
        private ObservableCollection<Benchmark> _vBenchmarks = null;
        [JsonIgnore]
        public ObservableCollection<Benchmark> vBenchmarks
        {
            get { return _vBenchmarks; }
            set { Set("vBenchmarks", ref _vBenchmarks, value); }
        }

    }

    public class Purchase_Log : ViewModelBase
    {
        public int? time { get; set; }
        public string key { get; set; }
        public int? charges { get; set; }

        [JsonIgnore]
        public string PurchaseTime { get; set; }

        [JsonIgnore]
        public string ItemCharges { get; set; } = string.Empty;

        [JsonIgnore]
        private BitmapImage _ItemImageSource = null;
        [JsonIgnore]
        public BitmapImage ItemImageSource
        {
            get { return _ItemImageSource; }
            private set { Set("ItemImageSource", ref _ItemImageSource, value); }
        }

        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                if (this.ItemImageSource != null || string.IsNullOrWhiteSpace(this.key) || !Uri.IsWellFormedUriString(this.key, UriKind.Absolute)) return;

                var imageSource = await ImageCourier.GetImageAsync(this.key, decodeWidth, 0);
                if (imageSource != null)
                {
                    this.ItemImageSource = imageSource;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }

    public class Runes_Log
    {
        public long? time { get; set; }
        public int? key { get; set; }
    }

    public class Benchmark
    {
        public double raw { get; set; }
        public double pct { get; set; }

        [JsonIgnore]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public double BarWidth { get; set; } = 0;
    }
}
