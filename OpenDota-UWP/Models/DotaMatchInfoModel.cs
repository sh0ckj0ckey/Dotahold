using OpenDota_UWP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenDota_UWP.Models
{
    public class DotaMatchInfoModel
    {
        public long? match_id { get; set; }
        public int? dire_score { get; set; }
        public long? duration { get; set; }
        public long? first_blood_time { get; set; }
        public int? game_mode { get; set; }
        public int? lobby_type { get; set; }
        public List<Picks_Bans> picks_bans { get; set; }
        public List<double> radiant_gold_adv { get; set; }
        public int? radiant_score { get; set; }
        public bool? radiant_win { get; set; }
        public List<double> radiant_xp_adv { get; set; }
        public int? skill { get; set; }
        public long? start_time { get; set; }
        public Team radiant_team { get; set; } = null;
        public Team dire_team { get; set; } = null;
        public List<Player> players { get; set; }
    }

    public class Picks_Bans : ViewModels.ViewModelBase
    {
        public bool? is_pick { get; set; }
        public int? hero_id { get; set; }
        public int? team { get; set; }  //0-radiant 1-dire

        //public int order { get; set; }
        //public int ord { get; set; }
        //public long match_id { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string sHeroImage { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string sHeroName { get; set; }

        // 比赛英雄图片
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ImageSource = null;
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            set { Set("ImageSource", ref _ImageSource, value); }
        }
        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                if (ImageSource != null) return;

                ImageSource = await ImageLoader.LoadImageAsync(sHeroImage);
                ImageSource.DecodePixelType = DecodePixelType.Logical;
                ImageSource.DecodePixelWidth = decodeWidth;
            }
            catch { }
        }
    }

    public class Player : ViewModels.ViewModelBase
    {
        public long? match_id { get; set; }
        public int? player_slot { get; set; }
        public List<int> ability_upgrades_arr { get; set; }
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
        public int? gold { get; set; }
        public int? gold_per_min { get; set; }
        public int? gold_spent { get; set; }
        public List<int> gold_t { get; set; }
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
        public int? obs_placed { get; set; }
        public long? party_id { get; set; }
        public int? party_size { get; set; }
        public List<Permanent_Buffs> permanent_buffs { get; set; }
        public int? pings { get; set; }
        public List<Purchase_Log> purchase_log { get; set; }
        public int? roshans_killed { get; set; }
        public List<Runes_Log> runes_log { get; set; }
        public int? tower_damage { get; set; }
        public int? towers_killed { get; set; }
        public int? xp_per_min { get; set; }
        public List<int> xp_t { get; set; }
        public string personaname { get; set; }
        public string name { get; set; }
        public bool radiant_win { get; set; }
        public int? total_gold { get; set; }
        public int? total_xp { get; set; }
        public int? neutral_kills { get; set; }
        public int? tower_kills { get; set; }
        public int? courier_kills { get; set; }
        public int? observer_kills { get; set; }
        public int? sentry_kills { get; set; }
        public int? roshan_kills { get; set; }
        public int? buyback_count { get; set; }
        public int? rank_tier { get; set; }
        public bool? isRadiant { get; set; } = null;
        public bool? randomed { get; set; }
        public Benchmarks benchmarks { get; set; }

        // 是否是当前登录玩家
        [Newtonsoft.Json.JsonIgnore]
        public bool bIsCurrentPlayer { get; set; } = false;


        [Newtonsoft.Json.JsonIgnore]
        public string sHeroImage { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string sHeroName { get; set; }

        // 英雄图片
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ImageSource = null;
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            set { Set("ImageSource", ref _ImageSource, value); }
        }
        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                if (ImageSource != null) return;

                ImageSource = await ImageLoader.LoadImageAsync(sHeroImage);
                ImageSource.DecodePixelType = DecodePixelType.Logical;
                ImageSource.DecodePixelWidth = decodeWidth;
            }
            catch { }
        }

        [Newtonsoft.Json.JsonIgnore]
        public string sNameItem0 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sNameItem1 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sNameItem2 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sNameItem3 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sNameItem4 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sNameItem5 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sNameItemB0 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sNameItemB1 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sNameItemB2 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sNameItemN { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonIgnore]
        public string sItem0 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sItem1 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sItem2 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sItem3 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sItem4 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sItem5 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sItemB0 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sItemB1 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sItemB2 { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        public string sItemN { get; set; } = string.Empty;
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _Item0ImageSource = null;
        public BitmapImage Item0ImageSource
        {
            get { return _Item0ImageSource; }
            set { Set("Item0ImageSource", ref _Item0ImageSource, value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _Item1ImageSource = null;
        public BitmapImage Item1ImageSource
        {
            get { return _Item1ImageSource; }
            set { Set("Item1ImageSource", ref _Item1ImageSource, value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _Item2ImageSource = null;
        public BitmapImage Item2ImageSource
        {
            get { return _Item2ImageSource; }
            set { Set("Item2ImageSource", ref _Item2ImageSource, value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _Item3ImageSource = null;
        public BitmapImage Item3ImageSource
        {
            get { return _Item3ImageSource; }
            set { Set("Item3ImageSource", ref _Item3ImageSource, value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _Item4ImageSource = null;
        public BitmapImage Item4ImageSource
        {
            get { return _Item4ImageSource; }
            set { Set("Item4ImageSource", ref _Item4ImageSource, value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _Item5ImageSource = null;
        public BitmapImage Item5ImageSource
        {
            get { return _Item5ImageSource; }
            set { Set("Item5ImageSource", ref _Item5ImageSource, value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ItemB0ImageSource = null;
        public BitmapImage ItemB0ImageSource
        {
            get { return _ItemB0ImageSource; }
            set { Set("ItemB0ImageSource", ref _ItemB0ImageSource, value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ItemB1ImageSource = null;
        public BitmapImage ItemB1ImageSource
        {
            get { return _ItemB1ImageSource; }
            set { Set("ItemB1ImageSource", ref _ItemB1ImageSource, value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ItemB2ImageSource = null;
        public BitmapImage ItemB2ImageSource
        {
            get { return _ItemB2ImageSource; }
            set { Set("ItemB2ImageSource", ref _ItemB2ImageSource, value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ItemNImageSource = null;
        public BitmapImage ItemNImageSource
        {
            get { return _ItemNImageSource; }
            set { Set("ItemNImageSource", ref _ItemNImageSource, value); }
        }

        public async Task LoadItemsImageAsync(int itemDecodeWidth, int backpackDecodeWidth, int neutralDecodeWidth)
        {
            try
            {
                if (Item0ImageSource == null && !string.IsNullOrEmpty(sItem0))
                {
                    Item0ImageSource = await ImageLoader.LoadImageAsync(sItem0);
                    Item0ImageSource.DecodePixelType = DecodePixelType.Logical;
                    Item0ImageSource.DecodePixelWidth = itemDecodeWidth;
                }
                if (Item1ImageSource == null && !string.IsNullOrEmpty(sItem1))
                {
                    Item1ImageSource = await ImageLoader.LoadImageAsync(sItem1);
                    Item1ImageSource.DecodePixelType = DecodePixelType.Logical;
                    Item1ImageSource.DecodePixelWidth = itemDecodeWidth;
                }
                if (Item2ImageSource == null && !string.IsNullOrEmpty(sItem2))
                {
                    Item2ImageSource = await ImageLoader.LoadImageAsync(sItem2);
                    Item2ImageSource.DecodePixelType = DecodePixelType.Logical;
                    Item2ImageSource.DecodePixelWidth = itemDecodeWidth;
                }
                if (Item3ImageSource == null && !string.IsNullOrEmpty(sItem3))
                {
                    Item3ImageSource = await ImageLoader.LoadImageAsync(sItem3);
                    Item3ImageSource.DecodePixelType = DecodePixelType.Logical;
                    Item3ImageSource.DecodePixelWidth = itemDecodeWidth;
                }
                if (Item4ImageSource == null && !string.IsNullOrEmpty(sItem4))
                {
                    Item4ImageSource = await ImageLoader.LoadImageAsync(sItem4);
                    Item4ImageSource.DecodePixelType = DecodePixelType.Logical;
                    Item4ImageSource.DecodePixelWidth = itemDecodeWidth;
                }
                if (Item5ImageSource == null && !string.IsNullOrEmpty(sItem5))
                {
                    Item5ImageSource = await ImageLoader.LoadImageAsync(sItem5);
                    Item5ImageSource.DecodePixelType = DecodePixelType.Logical;
                    Item5ImageSource.DecodePixelWidth = itemDecodeWidth;
                }
                if (ItemB0ImageSource == null && !string.IsNullOrEmpty(sItemB0))
                {
                    ItemB0ImageSource = await ImageLoader.LoadImageAsync(sItemB0);
                    ItemB0ImageSource.DecodePixelType = DecodePixelType.Logical;
                    ItemB0ImageSource.DecodePixelWidth = backpackDecodeWidth;
                }
                if (ItemB1ImageSource == null && !string.IsNullOrEmpty(sItemB1))
                {
                    ItemB1ImageSource = await ImageLoader.LoadImageAsync(sItemB1);
                    ItemB1ImageSource.DecodePixelType = DecodePixelType.Logical;
                    ItemB1ImageSource.DecodePixelWidth = backpackDecodeWidth;
                }
                if (ItemB2ImageSource == null && !string.IsNullOrEmpty(sItemB2))
                {
                    ItemB2ImageSource = await ImageLoader.LoadImageAsync(sItemB2);
                    ItemB2ImageSource.DecodePixelType = DecodePixelType.Logical;
                    ItemB2ImageSource.DecodePixelWidth = backpackDecodeWidth;
                }
                if (ItemNImageSource == null && !string.IsNullOrEmpty(sItemN))
                {
                    ItemNImageSource = await ImageLoader.LoadImageAsync(sItemN);
                    ItemNImageSource.DecodePixelType = DecodePixelType.Logical;
                    ItemNImageSource.DecodePixelWidth = neutralDecodeWidth;
                }
            }
            catch { }
        }
    }

    public class Team
    {
        public string team_id { get; set; }
        public string name { get; set; }
    }

    public class Benchmarks
    {
        public Gold_Per_Min gold_per_min { get; set; }
        public Xp_Per_Min xp_per_min { get; set; }
        public Kills_Per_Min kills_per_min { get; set; }
        public Last_Hits_Per_Min last_hits_per_min { get; set; }
        public Hero_Damage_Per_Min hero_damage_per_min { get; set; }
        public Hero_Healing_Per_Min hero_healing_per_min { get; set; }
        public Tower_Damage tower_damage { get; set; }
        public Stuns_Per_Min stuns_per_min { get; set; }
        public Lhten lhten { get; set; }
    }

    public class Gold_Per_Min
    {
        public int raw { get; set; }
        public float pct { get; set; }
    }

    public class Xp_Per_Min
    {
        public int raw { get; set; }
        public float pct { get; set; }
    }

    public class Kills_Per_Min
    {
        public float raw { get; set; }
        public float pct { get; set; }
    }

    public class Last_Hits_Per_Min
    {
        public float raw { get; set; }
        public float pct { get; set; }
    }

    public class Hero_Damage_Per_Min
    {
        public float raw { get; set; }
        public float pct { get; set; }
    }

    public class Hero_Healing_Per_Min
    {
        public float raw { get; set; }
        public float pct { get; set; }
    }

    public class Tower_Damage
    {
        public int raw { get; set; }
        public float pct { get; set; }
    }

    public class Stuns_Per_Min
    {
        public float raw { get; set; }
        public float pct { get; set; }
    }

    public class Lhten
    {
        public int raw { get; set; }
        public float pct { get; set; }
    }

    public class Additional_Units
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
    }

    public class Permanent_Buffs
    {
        public int? permanent_buff { get; set; }
        public int? stack_count { get; set; }
    }

    public class Purchase_Log
    {
        public int? time { get; set; }
        public string key { get; set; }
        public int? charges { get; set; }
    }

    public class Runes_Log
    {
        public int? time { get; set; }
        public int? key { get; set; }
    }

}
