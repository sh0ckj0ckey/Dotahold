using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BackgroundTasks
{
    public sealed class MatchBackgroundTask : IBackgroundTask
    {
        public static LastMatch LastMatch { get; set; }
        public static string _playerID { get; set; }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            //_playerID = SharedData.Share.GetID();
            _playerID = "234359087";
            if (_playerID == null || _playerID == "")
            {
                deferral.Complete();
                return;
            }

            // Get the latest match.
            var match = await GetLastMatch();

            // Update the live tile with the match data.
            UpdateTile(match);

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        private static async Task<PlayerProfile> GetLastMatch()
        {

            string url = String.Format("https://api.opendota.com/api/players/{0}", _playerID);
            //http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={0}&steamids={1}
            HttpClient http = new HttpClient();
            BackgroundTasks.PlayerProfile player;
            try
            {
                var response = await http.GetAsync(new Uri(url));
                var jsonMessage = await response.Content.ReadAsStringAsync();

                if (jsonMessage == "{\"error\":\"rate limit exceeded\"}")
                {
                    return null;
                }

                player = JsonConvert.DeserializeObject<PlayerProfile>(jsonMessage);
                return player;
            }
            catch
            {
                return null;
            }

            //LastMatch match = null;

            //try
            //{
            //    //获取最近一场比赛数据赋值给 match
            //    string url = String.Format("https://api.opendota.com/api/players/{0}/recentMatches", _playerID);
            //    HttpClient http = new HttpClient();
            //    match = new LastMatch();
            //    try
            //    {
            //        var response = await http.GetAsync(new Uri(url));
            //        var jsonMessage = await response.Content.ReadAsStringAsync();

            //        if (jsonMessage == "{\"error\":\"rate limit exceeded\"}")
            //        {
            //            return null;
            //        }

            //        Match player_slot = Regex.Match(jsonMessage, "\\\"player_slot\\\":([\\s\\S]*?),");
            //        Match radiant_win = Regex.Match(jsonMessage, "\\\"radiant_win\\\":([\\s\\S]*?),");
            //        Match hero_id = Regex.Match(jsonMessage, "\\\"hero_id\\\":([\\s\\S]*?),");
            //        Match kills = Regex.Match(jsonMessage, "\\\"kills\\\":([\\s\\S]*?),");
            //        Match deaths = Regex.Match(jsonMessage, "\\\"deaths\\\":([\\s\\S]*?),");
            //        Match assists = Regex.Match(jsonMessage, "\\\"assists\\\":([\\s\\S]*?),");
            //        Match xp_per_min = Regex.Match(jsonMessage, "\\\"xp_per_min\\\":([\\s\\S]*?),");
            //        Match gold_per_min = Regex.Match(jsonMessage, "\\\"gold_per_min\\\":([\\s\\S]*?),");


            //        match.KDA = "K/D/A: " + kills + "/" + deaths + "/" + assists;
            //        match.GPM = "GPM: " + gold_per_min;
            //        match.XPM = "XPM: " + xp_per_min;

            //        //处理胜负
            //        if ((radiant_win.Groups[1].Value == "true" && (player_slot.Groups[1].Value == "0" || player_slot.Groups[1].Value == "1" || player_slot.Groups[1].Value == "2" || player_slot.Groups[1].Value == "3" || player_slot.Groups[1].Value == "4")) ||
            //            (radiant_win.Groups[1].Value == "false" && (player_slot.Groups[1].Value == "128" || player_slot.Groups[1].Value == "129" || player_slot.Groups[1].Value == "130" || player_slot.Groups[1].Value == "131" || player_slot.Groups[1].Value == "132")))
            //        {
            //            match.IsWin = "胜利";
            //        }
            //        else
            //        {
            //            match.IsWin = "失败";
            //        }

            //        string[] HeroID = new string[]{
            //        "None",
            //        "antimage",
            //        "axe",
            //        "bane",
            //        "bloodseeker",
            //        "crystal_maiden",
            //        "drow_ranger",
            //        "earthshaker",
            //        "juggernaut",
            //        "mirana",
            //        "morphling",
            //        "nevermore",
            //        "phantom_lancer",
            //        "puck",
            //        "pudge",
            //        "razor",
            //        "sand_king",
            //        "storm_spirit",
            //        "sven",
            //        "tiny",
            //        "vengefulspirit",
            //        "windrunner",
            //        "zuus",
            //        "kunkka",
            //        "None",
            //        "lina",
            //        "lion",
            //        "shadow_shaman",
            //        "slardar",
            //        "tidehunter",
            //        "witch_doctor",
            //        "lich",
            //        "riki",
            //        "enigma",
            //        "tinker",
            //        "sniper",
            //        "necrolyte",
            //        "warlock",
            //        "beastmaster",
            //        "queenofpain",
            //        "venomancer",
            //        "faceless_void",
            //        "skeleton_king",
            //        "death_prophet",
            //        "phantom_assassin",
            //        "pugna",
            //        "templar_assassin",
            //        "viper",
            //        "luna",
            //        "dragon_knight",
            //        "dazzle",
            //        "rattletrap",
            //        "leshrac",
            //        "furion",
            //        "life_stealer",
            //        "dark_seer",
            //        "clinkz",
            //        "omniknight",
            //        "enchantress",
            //        "huskar",
            //        "night_stalker",
            //        "broodmother",
            //        "bounty_hunter",
            //        "weaver",
            //        "jakiro",
            //        "batrider",
            //        "chen",
            //        "spectre",
            //        "ancient_apparition",
            //        "doom_bringer",
            //        "ursa",
            //        "spirit_breaker",
            //        "gyrocopter",
            //        "alchemist",
            //        "invoker",
            //        "silencer",
            //        "obsidian_destroyer",
            //        "lycan",
            //        "brewmaster",
            //        "shadow_demon",
            //        "lone_druid",
            //        "chaos_knight",
            //        "meepo",
            //        "treant",
            //        "ogre_magi",
            //        "undying",
            //        "rubick",
            //        "disruptor",
            //        "nyx_assassin",
            //        "naga_siren",
            //        "keeper_of_the_light",
            //        "wisp",
            //        "visage",
            //        "slark",
            //        "medusa",
            //        "troll_warlord",
            //        "centaur",
            //        "magnataur",
            //        "shredder",
            //        "bristleback",
            //        "tusk",
            //        "skywrath_mage",
            //        "abaddon",
            //        "elder_titan",
            //        "legion_commander",
            //        "techies",
            //        "ember_spirit",
            //        "earth_spirit",
            //        "abyssal_underlord",
            //        "terrorblade",
            //        "phoenix",
            //        "oracle",
            //        "winter_wyvern",
            //        "arc_warden",
            //        "monkey_king",
            //        "None",
            //        "None",
            //        "None",
            //        "None",
            //        "dark_willow",
            //        "pangolier",
            //        "grimstroke",
            //        "None",
            //        "None"
            //    };
            //        string[] HeroID_zh = new string[]{
            //        "None",
            //        "敌法师",
            //        "斧王",
            //        "祸乱之源",
            //        "嗜血狂魔",
            //        "水晶室女",
            //        "卓尔游侠",
            //        "撼地者",
            //        "主宰",
            //        "米拉娜",
            //        "变体精灵",
            //        "影魔",
            //        "幻影长矛手",
            //        "帕克",
            //        "帕吉",
            //        "剃刀",
            //        "沙王",
            //        "风暴之灵",
            //        "斯温",
            //        "小小",
            //        "复仇之魂",
            //        "风行者",
            //        "宙斯",
            //        "昆卡",
            //        "None",
            //        "莉娜",
            //        "莱恩",
            //        "暗影萨满",
            //        "斯拉达",
            //        "潮汐猎人",
            //        "巫医",
            //        "巫妖",
            //        "力丸",
            //        "谜团",
            //        "修补匠",
            //        "狙击手",
            //        "瘟疫法师",
            //        "术士",
            //        "兽王",
            //        "痛苦女王",
            //        "剧毒术士",
            //        "虚空假面",
            //        "冥魂大帝",
            //        "死亡先知",
            //        "幻影刺客",
            //        "帕格纳",
            //        "圣堂刺客",
            //        "冥界亚龙",
            //        "露娜",
            //        "龙骑士",
            //        "戴泽",
            //        "发条技师",
            //        "拉席克",
            //        "先知",
            //        "噬魂鬼",
            //        "黑暗贤者",
            //        "克林克兹",
            //        "全能骑士",
            //        "魅惑魔女",
            //        "哈斯卡",
            //        "暗夜魔王",
            //        "育母蜘蛛",
            //        "赏金猎人",
            //        "编织者",
            //        "杰奇洛",
            //        "蝙蝠骑士",
            //        "陈",
            //        "幽鬼",
            //        "远古冰魄",
            //        "末日使者",
            //        "熊战士",
            //        "裂魂人",
            //        "矮人直升机",
            //        "炼金术士",
            //        "祈求者",
            //        "沉默术士",
            //        "殁境神蚀者",
            //        "狼人",
            //        "酒仙",
            //        "暗影恶魔",
            //        "德鲁伊",
            //        "混沌骑士",
            //        "米波",
            //        "树精卫士",
            //        "食人魔魔法师",
            //        "不朽尸王",
            //        "拉比克",
            //        "干扰者",
            //        "司夜刺客",
            //        "娜迦海妖",
            //        "光之守卫",
            //        "艾欧",
            //        "维萨吉",
            //        "斯拉克",
            //        "美杜莎",
            //        "巨魔战将",
            //        "半人马战行者",
            //        "马格纳斯",
            //        "伐木机",
            //        "钢背兽",
            //        "巨牙海民",
            //        "天怒法师",
            //        "亚巴顿",
            //        "上古巨神",
            //        "军团指挥官",
            //        "工程师",
            //        "灰烬之灵",
            //        "大地之灵",
            //        "孽主",
            //        "恐怖利刃",
            //        "凤凰",
            //        "神谕者",
            //        "寒冬飞龙",
            //        "天穹守望者",
            //        "齐天大圣",
            //        "None",
            //        "None",
            //        "None",
            //        "None",
            //        "邪影芳灵",
            //        "石鳞剑士",
            //        "天涯墨客",
            //        "None",
            //        "None"
            //    };
            //        match.HeroName = HeroID_zh[Convert.ToInt32(hero_id)];
            //        match.HeroPicture = "ms-appx:///Assets/HeroesPhotoVert/" + HeroID[Convert.ToInt32(hero_id)] + "_vert.jpg";
            //    }
            //    catch
            //    {
            //        return null;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine(ex.ToString());
            //}

            //return match;
        }


        private static void UpdateTile(PlayerProfile player/*LastMatch match*/)
        {
            //if (match == null)
            //{
            //    return;
            //}

            //// Create a tile update manager for the specified syndication match data.
            //var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            //updater.EnableNotificationQueue(true);
            //updater.Clear();

            //TileContent content = new TileContent()
            //{
            //    Visual = new TileVisual()
            //    {
            //        TileMedium = new TileBinding()
            //        {
            //            Branding = TileBranding.Name,
            //            Content = new TileBindingContentAdaptive()
            //            {
            //                PeekImage = new TilePeekImage()
            //                {
            //                    Source = match.HeroPicture
            //                },
            //                Children =
            //                {
            //                    new AdaptiveText()
            //                    {
            //                        Text =match.HeroName,
            //                        HintStyle=AdaptiveTextStyle.Subtitle
            //                    },
            //                    new AdaptiveText()
            //                    {
            //                        Text = match.IsWin,
            //                        HintStyle = AdaptiveTextStyle.CaptionSubtle
            //                    },
            //                    new AdaptiveText()
            //                    {
            //                        Text=match.KDA,
            //                        HintStyle=AdaptiveTextStyle.CaptionSubtle
            //                    }
            //                }
            //            }
            //        },

            //        TileWide = new TileBinding()
            //        {
            //            Branding = TileBranding.Name,
            //            Content = new TileBindingContentAdaptive()
            //            {
            //                PeekImage = new TilePeekImage()
            //                {
            //                    Source = match.HeroPicture,
            //                },
            //                Children =
            //                {
            //                    new AdaptiveText()
            //                    {
            //                        Text =match.HeroName,
            //                        HintStyle=AdaptiveTextStyle.Subtitle
            //                    },
            //                    new AdaptiveText()
            //                    {
            //                        Text = match.IsWin,
            //                        HintStyle = AdaptiveTextStyle.CaptionSubtle
            //                    },
            //                    new AdaptiveText()
            //                    {
            //                        Text=match.KDA,
            //                        HintStyle=AdaptiveTextStyle.CaptionSubtle
            //                    }
            //                }
            //            }
            //        },

            //        TileLarge = new TileBinding()
            //        {
            //            Branding = TileBranding.Name,
            //            Content = new TileBindingContentAdaptive()
            //            {
            //                PeekImage = new TilePeekImage()
            //                {
            //                    Source = match.HeroPicture
            //                },
            //                Children =
            //                {
            //                    new AdaptiveText()
            //                    {
            //                        Text = match.HeroName,
            //                        HintStyle=AdaptiveTextStyle.Subtitle
            //                    },
            //                    new AdaptiveText()
            //                    {
            //                        Text = match.IsWin,
            //                        HintStyle = AdaptiveTextStyle.CaptionSubtle
            //                    },
            //                    new AdaptiveText()
            //                    {
            //                        Text=match.KDA,
            //                        HintStyle=AdaptiveTextStyle.CaptionSubtle
            //                    },
            //                    new AdaptiveText()
            //                    {
            //                        Text=match.GPM,
            //                        HintStyle=AdaptiveTextStyle.CaptionSubtle
            //                    },
            //                    new AdaptiveText()
            //                    {
            //                        Text=match.XPM,
            //                        HintStyle=AdaptiveTextStyle.CaptionSubtle
            //                    }
            //                }
            //            }
            //        }
            //    }
            //};
            //// Create a new tile notification.
            //updater.Update(new TileNotification(content.GetXml()));


            string playerRankTier = "分段: 待定";
            if (player.rank_tier != null)
            {
                switch (player.rank_tier[0])
                {
                    case '0':
                        break;
                    case '1':
                        playerRankTier = "先锋xxx" + player.rank_tier[1];
                        break;
                    case '2':
                        playerRankTier = "卫士xxxxxx" + player.rank_tier[1];
                        break;
                    case '3':
                        playerRankTier = "中军xxxxxx" + player.rank_tier[1];
                        break;
                    case '4':
                        playerRankTier = "统帅xxxxxxxx" + player.rank_tier[1];
                        break;
                    case '5':
                        playerRankTier = "传奇xxxxxxxxx" + player.rank_tier[1];
                        break;
                    case '6':
                        playerRankTier = "万古流芳xxxxxx" + player.rank_tier[1];
                        break;
                    case '7':
                        playerRankTier = "超凡入圣xxxxxxx" + player.rank_tier[1];
                        break;
                    case '8':
                        playerRankTier = "冠绝一世xxxxxx";
                        break;
                    default:
                        playerRankTier = "分段: 待定xxxxxxxxxx";
                        break;
                }
            }

            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            PeekImage = new TilePeekImage()
                            {
                                Source = player.profile.avatarfull
                            },
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text =player.profile.personaname,
                                    HintStyle=AdaptiveTextStyle.Body
                                },
                                new AdaptiveText()
                                {
                                    Text ="ID: "+player.profile.account_id,
                                    HintStyle=AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text=playerRankTier+"  "+player.profile.loccountrycode,
                                    HintStyle =AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            PeekImage = new TilePeekImage()
                            {
                                Source = player.profile.avatarfull
                            },
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text =player.profile.personaname,
                                    HintStyle=AdaptiveTextStyle.Subtitle
                                },
                                new AdaptiveText()
                                {
                                    Text ="ID: "+ player.profile.account_id,
                                    HintStyle = AdaptiveTextStyle.BodySubtle
                                },
                                new AdaptiveText()
                                {
                                    Text=playerRankTier+"  "+player.profile.loccountrycode,
                                    HintStyle=AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    },

                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            PeekImage = new TilePeekImage()
                            {
                                Source = player.profile.avatarfull
                            },
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text =player.profile.personaname,
                                    HintStyle=AdaptiveTextStyle.Subtitle,
                                    HintWrap=true
                                },
                                new AdaptiveText()
                                {
                                    Text = "ID: "+player.profile.account_id,
                                    HintStyle = AdaptiveTextStyle.Base
                                },
                                new AdaptiveText()
                                {
                                    Text=playerRankTier + "  " + player.profile.loccountrycode,
                                    HintStyle=AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text="MMR评估: "+player.mmr_estimate.estimate,
                                    HintStyle=AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    }
                }
            };
            var notification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

        }
    }

    public sealed class LastMatch
    {
        public string HeroPicture { get; set; }
        public string HeroName { get; set; }
        public string IsWin { get; set; }
        public string KDA { get; set; }
        public string GPM { get; set; }
        public string XPM { get; set; }
    }

    /// <summary>
    /// 用户的游戏信息比如分段排名等
    /// </summary>
    public sealed class PlayerProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public string tracked_until { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Profile profile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string competitive_rank { get; set; }
        /// <summary>
        /// 冠绝一世排名
        /// </summary>
        public string leaderboard_rank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string solo_competitive_rank { get; set; }
        /// <summary>
        /// 分段，十位数表示徽章，个位数表示星级
        /// </summary>
        public string rank_tier { get; set; }
        /// <summary>
        /// MMR（估计值，非天梯分数）
        /// </summary>
        public Mmr_estimate mmr_estimate { get; set; }

        public PlayerProfile()
        {
            this.tracked_until = "";
            this.competitive_rank = "";
            this.leaderboard_rank = "";
            this.solo_competitive_rank = "0";
            this.rank_tier = "10";
            this.mmr_estimate = new Mmr_estimate();
            this.profile = new Profile();
        }
    }

    // 用户个人信息,例如ID,名字等
    public sealed class Profile
    {
        /// <summary>
        /// 游戏内ID
        /// </summary>
        public int account_id { get; set; }
        /// <summary>
        /// Steam名字
        /// </summary>
        public string personaname { get; set; }

        public string name { get; set; }

        public int cheese { get; set; }
        /// <summary>
        /// SteamID
        /// </summary>
        public string steamid { get; set; }

        public string avatar { get; set; }

        public string avatarmedium { get; set; }
        /// <summary>
        /// 大图头像
        /// </summary>
        public string avatarfull { get; set; }

        public string profileurl { get; set; }

        public string last_login { get; set; }

        public string loccountrycode { get; set; }

        public Profile()
        {
            this.account_id = 0;
            this.avatar = "";
            this.avatarfull = "";
            this.avatarmedium = "";
            this.cheese = 0;
            this.last_login = "";
            this.loccountrycode = "";
            this.name = "";
            this.personaname = "";
            this.profileurl = "";
            this.steamid = "";
        }
    }
    //MMR评估(非天梯)
    public sealed class Mmr_estimate
    {
        /// <summary>
        /// MMR分数评估
        /// </summary>
        public int estimate { get; set; }

        public Mmr_estimate() { estimate = 0; }
    }
}
