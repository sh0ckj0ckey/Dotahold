using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class DotaMatchInfoModel
    {
        public List<Player> players { get; set; }
    }

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

        // 技能加点顺序
        [JsonIgnore]
        private ObservableCollection<AbilityUpgrade> _vAbilitiesUpgrade = null;
        [JsonIgnore]
        public ObservableCollection<AbilityUpgrade> vAbilitiesUpgrade
        {
            get { return _vAbilitiesUpgrade; }
            set { Set("vAbilitiesUpgrade", ref _vAbilitiesUpgrade, value); }
        }

        // KDA
        [JsonIgnore]
        private string _sKDA = string.Empty;
        [JsonIgnore]
        public string sKDA
        {
            get { return _sKDA; }
            set { Set("sKDA", ref _sKDA, value); }
        }

        // 目前只处理熊灵这一个额外单位
        [JsonIgnore]
        public Additional_Units SpiritBear { get; set; } = null;

        // 开黑队伍编号
        [JsonIgnore]
        public int iPartyId { get; set; } = 0;

        // 是否是当前登录玩家
        [JsonIgnore]
        public bool bIsCurrentPlayer { get; set; } = false;

        // 是否激活神杖
        [JsonIgnore]
        public bool bHaveAghanimScepter { get; set; } = false;

        // 是否激活魔晶
        [JsonIgnore]
        public bool bHaveAghanimShard { get; set; } = false;


        [JsonIgnore]
        public string sHeroImage { get; set; }

        [JsonIgnore]
        public string sHeroName { get; set; }

        // 英雄图片
        [JsonIgnore]
        private BitmapImage _ImageSource = null;
        [JsonIgnore]
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            private set { Set("ImageSource", ref _ImageSource, value); }
        }
        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                if (this.ImageSource != null || string.IsNullOrWhiteSpace(this.sHeroImage)) return;

                var imageSource = await ImageCourier.GetImageAsync(this.sHeroImage, decodeWidth, 0);
                if (imageSource != null)
                {
                    this.ImageSource = imageSource;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        [JsonIgnore]
        public string sNameItem0 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItem1 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItem2 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItem3 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItem4 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItem5 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItemB0 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItemB1 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItemB2 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItemN { get; set; } = string.Empty;

        [JsonIgnore]
        public string sItem0 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItem1 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItem2 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItem3 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItem4 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItem5 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItemB0 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItemB1 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItemB2 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItemN { get; set; } = string.Empty;
        [JsonIgnore]
        private BitmapImage _Item0ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item0ImageSource
        {
            get { return _Item0ImageSource; }
            private set { Set("Item0ImageSource", ref _Item0ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _Item1ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item1ImageSource
        {
            get { return _Item1ImageSource; }
            private set { Set("Item1ImageSource", ref _Item1ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _Item2ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item2ImageSource
        {
            get { return _Item2ImageSource; }
            private set { Set("Item2ImageSource", ref _Item2ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _Item3ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item3ImageSource
        {
            get { return _Item3ImageSource; }
            private set { Set("Item3ImageSource", ref _Item3ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _Item4ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item4ImageSource
        {
            get { return _Item4ImageSource; }
            private set { Set("Item4ImageSource", ref _Item4ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _Item5ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item5ImageSource
        {
            get { return _Item5ImageSource; }
            private set { Set("Item5ImageSource", ref _Item5ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _ItemB0ImageSource = null;
        [JsonIgnore]
        public BitmapImage ItemB0ImageSource
        {
            get { return _ItemB0ImageSource; }
            private set { Set("ItemB0ImageSource", ref _ItemB0ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _ItemB1ImageSource = null;
        [JsonIgnore]
        public BitmapImage ItemB1ImageSource
        {
            get { return _ItemB1ImageSource; }
            private set { Set("ItemB1ImageSource", ref _ItemB1ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _ItemB2ImageSource = null;
        [JsonIgnore]
        public BitmapImage ItemB2ImageSource
        {
            get { return _ItemB2ImageSource; }
            private set { Set("ItemB2ImageSource", ref _ItemB2ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _ItemNImageSource = null;
        [JsonIgnore]
        public BitmapImage ItemNImageSource
        {
            get { return _ItemNImageSource; }
            private set { Set("ItemNImageSource", ref _ItemNImageSource, value); }
        }

        public async Task LoadItemsImageAsync(int itemDecodeWidth, int backpackDecodeWidth, int neutralDecodeWidth)
        {
            try
            {
                if (Item0ImageSource == null && !string.IsNullOrWhiteSpace(sItem0))
                {
                    Item0ImageSource = await ImageCourier.GetImageAsync(sItem0, itemDecodeWidth, 0);
                }
                if (Item1ImageSource == null && !string.IsNullOrWhiteSpace(sItem1))
                {
                    Item1ImageSource = await ImageCourier.GetImageAsync(sItem1, itemDecodeWidth, 0);
                }
                if (Item2ImageSource == null && !string.IsNullOrWhiteSpace(sItem2))
                {
                    Item2ImageSource = await ImageCourier.GetImageAsync(sItem2, itemDecodeWidth, 0);
                }
                if (Item3ImageSource == null && !string.IsNullOrWhiteSpace(sItem3))
                {
                    Item3ImageSource = await ImageCourier.GetImageAsync(sItem3, itemDecodeWidth, 0);
                }
                if (Item4ImageSource == null && !string.IsNullOrWhiteSpace(sItem4))
                {
                    Item4ImageSource = await ImageCourier.GetImageAsync(sItem4, itemDecodeWidth, 0);
                }
                if (Item5ImageSource == null && !string.IsNullOrWhiteSpace(sItem5))
                {
                    Item5ImageSource = await ImageCourier.GetImageAsync(sItem5, itemDecodeWidth, 0);
                }
                if (ItemB0ImageSource == null && !string.IsNullOrWhiteSpace(sItemB0))
                {
                    ItemB0ImageSource = await ImageCourier.GetImageAsync(sItemB0, itemDecodeWidth, 0);
                }
                if (ItemB1ImageSource == null && !string.IsNullOrWhiteSpace(sItemB1))
                {
                    ItemB1ImageSource = await ImageCourier.GetImageAsync(sItemB1, itemDecodeWidth, 0);
                }
                if (ItemB2ImageSource == null && !string.IsNullOrWhiteSpace(sItemB2))
                {
                    ItemB2ImageSource = await ImageCourier.GetImageAsync(sItemB2, itemDecodeWidth, 0);
                }
                if (ItemNImageSource == null && !string.IsNullOrWhiteSpace(sItemN))
                {
                    ItemNImageSource = await ImageCourier.GetImageAsync(sItemN, itemDecodeWidth, 0);
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }

    public class AbilityUpgrade : ViewModelBase
    {
        [JsonIgnore]
        private static BitmapImage _defaultAbilityImage = null;
        [JsonIgnore]
        private static BitmapImage _talentAbilityImage = null;

        [JsonIgnore]
        public string sAbilityUrl { get; set; }

        [JsonIgnore]
        public string sAbilityName { get; set; }

        [JsonIgnore]
        public bool bIsTalent { get; set; } = false;

        // 英雄图片
        [JsonIgnore]
        private BitmapImage _AbilityImageSource = null;
        [JsonIgnore]
        public BitmapImage AbilityImageSource
        {
            get { return _AbilityImageSource; }
            private set { Set("AbilityImageSource", ref _AbilityImageSource, value); }
        }
        public async Task LoadAbilityImageAsync(int decodeWidth)
        {
            try
            {
                if (AbilityImageSource != null) return;

                if (bIsTalent)
                {
                    if (_talentAbilityImage == null)
                    {
                        _talentAbilityImage = new BitmapImage(new System.Uri("ms-appx:///Assets/Icons/Match/icon_talent_tree.png"));
                    }
                    AbilityImageSource = _talentAbilityImage;
                    AbilityImageSource.DecodePixelType = DecodePixelType.Logical;
                    AbilityImageSource.DecodePixelWidth = decodeWidth;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(sAbilityUrl))
                    {
                        AbilityImageSource = await ImageCourier.GetImageAsync(sAbilityUrl, decodeWidth, 0);
                    }
                    else
                    {
                        if (_defaultAbilityImage == null)
                        {
                            _defaultAbilityImage = new BitmapImage(new System.Uri("ms-appx:///Assets/Icons/Match/PermanentBuffs/buff_placeholder.png"));
                        }
                        AbilityImageSource = _defaultAbilityImage;
                        AbilityImageSource.DecodePixelType = DecodePixelType.Logical;
                        AbilityImageSource.DecodePixelWidth = decodeWidth;
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }

    public class Additional_Units : ViewModelBase
    {
        public string unitname { get; set; }
        public int? item_0 { get; set; }
        public int? item_1 { get; set; }
        public int? item_2 { get; set; }
        public int? item_3 { get; set; }
        public int? item_4 { get; set; }
        public int? item_5 { get; set; }
        public int? backpack_0 { get; set; }
        public int? backpack_1 { get; set; }
        public int? backpack_2 { get; set; }
        public int? item_neutral { get; set; }

        [JsonIgnore]
        public string sNameItem0 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItem1 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItem2 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItem3 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItem4 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItem5 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItemB0 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItemB1 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItemB2 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sNameItemN { get; set; } = string.Empty;

        [JsonIgnore]
        public string sItem0 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItem1 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItem2 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItem3 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItem4 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItem5 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItemB0 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItemB1 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItemB2 { get; set; } = string.Empty;
        [JsonIgnore]
        public string sItemN { get; set; } = string.Empty;
        [JsonIgnore]
        private BitmapImage _Item0ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item0ImageSource
        {
            get { return _Item0ImageSource; }
            private set { Set("Item0ImageSource", ref _Item0ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _Item1ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item1ImageSource
        {
            get { return _Item1ImageSource; }
            private set { Set("Item1ImageSource", ref _Item1ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _Item2ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item2ImageSource
        {
            get { return _Item2ImageSource; }
            private set { Set("Item2ImageSource", ref _Item2ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _Item3ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item3ImageSource
        {
            get { return _Item3ImageSource; }
            private set { Set("Item3ImageSource", ref _Item3ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _Item4ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item4ImageSource
        {
            get { return _Item4ImageSource; }
            private set { Set("Item4ImageSource", ref _Item4ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _Item5ImageSource = null;
        [JsonIgnore]
        public BitmapImage Item5ImageSource
        {
            get { return _Item5ImageSource; }
            private set { Set("Item5ImageSource", ref _Item5ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _ItemB0ImageSource = null;
        [JsonIgnore]
        public BitmapImage ItemB0ImageSource
        {
            get { return _ItemB0ImageSource; }
            private set { Set("ItemB0ImageSource", ref _ItemB0ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _ItemB1ImageSource = null;
        [JsonIgnore]
        public BitmapImage ItemB1ImageSource
        {
            get { return _ItemB1ImageSource; }
            private set { Set("ItemB1ImageSource", ref _ItemB1ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _ItemB2ImageSource = null;
        [JsonIgnore]
        public BitmapImage ItemB2ImageSource
        {
            get { return _ItemB2ImageSource; }
            private set { Set("ItemB2ImageSource", ref _ItemB2ImageSource, value); }
        }
        [JsonIgnore]
        private BitmapImage _ItemNImageSource = null;
        [JsonIgnore]
        public BitmapImage ItemNImageSource
        {
            get { return _ItemNImageSource; }
            private set { Set("ItemNImageSource", ref _ItemNImageSource, value); }
        }

        public async Task LoadItemsImageAsync(int itemDecodeWidth, int backpackDecodeWidth, int neutralDecodeWidth)
        {
            try
            {
                if (Item0ImageSource == null && !string.IsNullOrWhiteSpace(sItem0))
                {
                    Item0ImageSource = await ImageCourier.GetImageAsync(sItem0, itemDecodeWidth, 0);
                }
                if (Item1ImageSource == null && !string.IsNullOrWhiteSpace(sItem1))
                {
                    Item1ImageSource = await ImageCourier.GetImageAsync(sItem1, itemDecodeWidth, 0);
                }
                if (Item2ImageSource == null && !string.IsNullOrWhiteSpace(sItem2))
                {
                    Item2ImageSource = await ImageCourier.GetImageAsync(sItem2, itemDecodeWidth, 0);
                }
                if (Item3ImageSource == null && !string.IsNullOrWhiteSpace(sItem3))
                {
                    Item3ImageSource = await ImageCourier.GetImageAsync(sItem3, itemDecodeWidth, 0);
                }
                if (Item4ImageSource == null && !string.IsNullOrWhiteSpace(sItem4))
                {
                    Item4ImageSource = await ImageCourier.GetImageAsync(sItem4, itemDecodeWidth, 0);
                }
                if (Item5ImageSource == null && !string.IsNullOrWhiteSpace(sItem5))
                {
                    Item5ImageSource = await ImageCourier.GetImageAsync(sItem5, itemDecodeWidth, 0);
                }
                if (ItemB0ImageSource == null && !string.IsNullOrWhiteSpace(sItemB0))
                {
                    ItemB0ImageSource = await ImageCourier.GetImageAsync(sItemB0, itemDecodeWidth, 0);
                }
                if (ItemB1ImageSource == null && !string.IsNullOrWhiteSpace(sItemB1))
                {
                    ItemB1ImageSource = await ImageCourier.GetImageAsync(sItemB1, itemDecodeWidth, 0);
                }
                if (ItemB2ImageSource == null && !string.IsNullOrWhiteSpace(sItemB2))
                {
                    ItemB2ImageSource = await ImageCourier.GetImageAsync(sItemB2, itemDecodeWidth, 0);
                }
                if (ItemNImageSource == null && !string.IsNullOrWhiteSpace(sItemN))
                {
                    ItemNImageSource = await ImageCourier.GetImageAsync(sItemN, itemDecodeWidth, 0);
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }

    public class Permanent_Buffs : ViewModelBase
    {
        [JsonIgnore]
        private static Dictionary<string, BitmapImage> _dictBuffs = new Dictionary<string, BitmapImage>();

        public int? permanent_buff { get; set; }
        public int? stack_count { get; set; }


        [JsonIgnore]
        public string sBuff { get; set; } = string.Empty;

        [JsonIgnore]
        private BitmapImage _BuffImageSource = ConstantsCourier.DefaultItemImageSource72;
        [JsonIgnore]
        public BitmapImage BuffImageSource
        {
            get { return _BuffImageSource; }
            private set { Set("BuffImageSource", ref _BuffImageSource, value); }
        }
        [JsonIgnore]
        private bool _loadedPermanentBuffImage = false;
        public async Task LoadBuffImageAsync(int imageDecodeHeight)
        {
            try
            {
                if (_loadedPermanentBuffImage || string.IsNullOrWhiteSpace(this.sBuff)) return;

                if (permanent_buff == -99) sBuff = "buff_placeholder";

                if (sBuff == "buff_placeholder" ||
                    sBuff == "moon_shard" ||
                    sBuff == "ultimate_scepter" ||
                    sBuff == "silencer_glaives_of_wisdom" ||
                    sBuff == "pudge_flesh_heap" ||
                    sBuff == "legion_commander_duel" ||
                    sBuff == "tome_of_knowledge" ||
                    sBuff == "lion_finger_of_death" ||
                    sBuff == "slark_essence_shift" ||
                    sBuff == "abyssal_underlord_atrophy_aura" ||
                    sBuff == "bounty_hunter_jinada" ||
                    sBuff == "aghanims_shard" ||
                    sBuff == "axe_culling_blade" ||
                    sBuff == "necrolyte_reapers_scythe")
                {
                    // 本地有图片资源
                    if (!_dictBuffs.ContainsKey(sBuff))
                    {
                        var img = new BitmapImage(new System.Uri(string.Format("ms-appx:///Assets/Icons/Match/PermanentBuffs/{0}.png", sBuff)));
                        img.DecodePixelType = DecodePixelType.Logical;
                        img.DecodePixelHeight = imageDecodeHeight;
                        _dictBuffs.Add(sBuff, img);
                    }
                    this.BuffImageSource = _dictBuffs[sBuff];
                }
                else
                {
                    // 从网络获取，通常是技能图标
                    this.BuffImageSource = await ImageCourier.GetImageAsync(string.Format("https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/abilities/{0}.png", this.sBuff), 0, imageDecodeHeight, true);
                }

                _loadedPermanentBuffImage = true;
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
