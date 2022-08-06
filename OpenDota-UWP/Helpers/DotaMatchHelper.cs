using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Windows.Data.Json;
using Windows.UI.Xaml.Controls;
using System.Net;
using Windows.Web.Http;
using OpenDota_UWP.Models;

namespace OpenDota_UWP.Helpers
{
    public class DotaMatchHelper
    {



        /// <summary>
        /// 获取指定编号的比赛的详细信息（没解析玩家）
        /// </summary>
        /// 技能图片：https://www.dota2.com.cn/images/heroes/abilities/{英雄名加技能名，如chaos_knight_chaos_bolt}_hp1.png
        /// <param name="matchid"></param>
        public static async Task<List<string>> GetMatchInfoAsync(string matchid)
        {
            //示例比赛编号3792271763
            string url = String.Format("https://api.opendota.com/api/matches/{0}", matchid);
            //HttpClientHandler handler = new HttpClientHandler();
            //handler.UseProxy = false;   //不加这个会非常慢
            HttpClient http = new HttpClient();
            List<string> matchInfoList = new List<string>();
            string jsonMessage;
            try
            {
                if (HeroPlayerInfo.CurrentMatchID != matchid || HeroPlayerInfo.buffer.StartsWith("{\"error\""))
                {
                    var response = await http.GetAsync(new Uri(url));
                    jsonMessage = await response.Content.ReadAsStringAsync();

                    HeroPlayerInfo.buffer = jsonMessage;
                    HeroPlayerInfo.CurrentMatchID = matchid;
                    if (jsonMessage == "{\"error\":\"rate limit exceeded\"}")
                    {
                        matchInfoList.Clear();
                        matchInfoList.Add("time_limit");
                        return matchInfoList;
                    }
                    else if (jsonMessage == "{\"error\":\"Internal Server Error\"}")
                    {
                        matchInfoList.Clear();
                        matchInfoList.Add("server_error");
                        return matchInfoList;
                    }
                }
                else
                {
                    jsonMessage = HeroPlayerInfo.buffer;
                }
                Match first_blood_timeMatch = Regex.Match(jsonMessage, "\\\"first_blood_time\\\":([\\d\\D]*?),");
                //Match start_timeMatch = Regex.Match(jsonMessage, "\\\"start_time\\\":([\\d\\D]*?),");
                Match durationMatch = Regex.Match(jsonMessage, "\\\"duration\\\":([\\d\\D]*?),");
                //Match levelMatch = Regex.Match(jsonMessage, "\\\"skill\\\":([\\d\\D]*?),");
                Match game_modeMatch = Regex.Match(jsonMessage, "\\\"game_mode\\\":([\\d\\D]*?),");
                Match replay_urlMatch = Regex.Match(jsonMessage, "\\\"replay_url\\\":\\\"([\\d\\D]*?)\\\"}");
                Match radiant_scoreMatch = Regex.Match(jsonMessage, "\\\"radiant_score\\\":([\\d\\D]*?),");
                Match dire_scoreMatch = Regex.Match(jsonMessage, "\\\"dire_score\\\":([\\d\\D]*?),");
                Match lobby_typeMatch = Regex.Match(jsonMessage, "\\\"lobby_type\\\":([\\d\\D]*?),");
                Match radiant_winMatch = Regex.Match(jsonMessage, "\\\"radiant_win\\\":([\\d\\D]*?),");
                Match radiant_gold_advMatch = Regex.Match(jsonMessage, "\\\"radiant_gold_adv\\\":\\[([\\d\\D]*?)\\],");
                Match radiant_xp_advMatch = Regex.Match(jsonMessage, "\\\"radiant_xp_adv\\\":\\[([\\d\\D]*?)\\],");


                matchInfoList.AddRange(new List<string> {
                    first_blood_timeMatch.Groups[1].Value,
                    durationMatch.Groups[1].Value,
                    game_modeMatch.Groups[1].Value,
                    replay_urlMatch.Groups[1].Value,
                    radiant_scoreMatch.Groups[1].Value,
                    dire_scoreMatch.Groups[1].Value,
                    lobby_typeMatch.Groups[1].Value,
                    radiant_winMatch.Groups[1].Value,
                    radiant_gold_advMatch.Groups[1].Value,
                    radiant_xp_advMatch.Groups[1].Value
                });
            }
            catch
            {
                matchInfoList.Clear();
                matchInfoList.Add("data_error");
                return matchInfoList;
            }
            return matchInfoList;
        }

        /// <summary>
        /// 获取本场比赛指定玩家的具体数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static HeroPlayerInfoViewModel GetHeroPlayerInfo(int index)
        {
            //#region 匹配
            //MatchCollection ability_upgrades_arrMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"ability_upgrades_arr\\\":\\[([\\s\\S]*?)\\],");
            //MatchCollection backpackItemsMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"assists\\\":([\\d]*?),\\\"backpack_0\\\":([\\s\\S]*?),\\\"backpack_1\\\":([\\s\\S]*?),\\\"backpack_2\\\":([\\s\\S]*?),");
            //MatchCollection hero_idMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"hero_id\\\":([\\s\\S]*?),");
            //MatchCollection hero_damageMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"hero_damage\\\":([\\s\\S]*?),");
            //MatchCollection hero_healingMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"hero_healing\\\":([\\s\\S]*?),");
            //MatchCollection itemsMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"hero_id\\\":[\\d]*?,\\\"item_0\\\":([\\s\\S]*?),\\\"item_1\\\":([\\s\\S]*?),\\\"item_2\\\":([\\s\\S]*?),\\\"item_3\\\":([\\s\\S]*?),\\\"item_4\\\":([\\s\\S]*?),\\\"item_5\\\":([\\s\\S]*?),");
            //MatchCollection permanent_buffsMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"permanent_buffs\\\":([\\s\\S]*?),\\\"pings");
            //MatchCollection total_goldMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"total_gold\\\":([\\s\\S]*?),");
            //MatchCollection total_xpMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"total_xp\\\":([\\s\\S]*?),");
            //MatchCollection party_sizeMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"party_size\\\":([\\s\\S]*?),");
            //MatchCollection rank_tierMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"rank_tier\\\":([\\s\\S]*?),");
            //MatchCollection benchmarksMatch = Regex.Matches(HeroPlayerInfo.buffer, "\\\"benchmarks\\\":{([\\s\\S]*?)}}");
            //#endregion

            //#region 赋值
            //HeroPlayerInfo heroPlayerInfo = new HeroPlayerInfo();
            //try
            //{
            //    heroPlayerInfo.Ability_upgrades_arr = ability_upgrades_arrMatch[index].Groups[1].Value;
            //}
            //catch { heroPlayerInfo.Ability_upgrades_arr = ""; }
            //try
            //{
            //    heroPlayerInfo.Benchmarks = benchmarksMatch[index].Groups[1].Value;
            //}
            //catch { heroPlayerInfo.Benchmarks = ""; }
            //try
            //{
            //    heroPlayerInfo.Hero_id = hero_idMatch[index + hero_idMatch.Count - 10].Groups[1].Value;
            //}
            //catch { heroPlayerInfo.Hero_id = ""; }
            //try
            //{
            //    heroPlayerInfo.Hero_damage = hero_damageMatch[index].Groups[1].Value;
            //}
            //catch { heroPlayerInfo.Hero_damage = ""; }
            //try
            //{
            //    heroPlayerInfo.Hero_healing = hero_healingMatch[index].Groups[1].Value;
            //}
            //catch { heroPlayerInfo.Hero_healing = ""; }
            //try
            //{
            //    heroPlayerInfo.Rank_tier = rank_tierMatch[index].Groups[1].Value;
            //}
            //catch { heroPlayerInfo.Rank_tier = ""; }
            //try
            //{
            //    heroPlayerInfo.Total_gold = total_goldMatch[index].Groups[1].Value;
            //}
            //catch { heroPlayerInfo.Total_gold = ""; }
            //try
            //{
            //    heroPlayerInfo.Total_xp = total_xpMatch[index].Groups[1].Value;
            //}
            //catch { heroPlayerInfo.Total_xp = ""; }
            //try
            //{
            //    heroPlayerInfo.Party_size = party_sizeMatch[index].Groups[1].Value;
            //    switch (heroPlayerInfo.Party_size)
            //    {
            //        case "1":
            //            heroPlayerInfo.Party_size = "单排";
            //            break;
            //        case "2":
            //            heroPlayerInfo.Party_size = "二黑";
            //            break;
            //        case "3":
            //            heroPlayerInfo.Party_size = "三黑";
            //            break;
            //        case "4":
            //            heroPlayerInfo.Party_size = "四黑";
            //            break;
            //        case "5":
            //            heroPlayerInfo.Party_size = "五黑";
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //catch { heroPlayerInfo.Party_size = ""; }
            //try
            //{
            //    heroPlayerInfo.Permanent_buffs = permanent_buffsMatch[index].Groups[1].Value;
            //}
            //catch { heroPlayerInfo.Permanent_buffs = ""; }
            //heroPlayerInfo.Item_0 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[itemsMatch[index].Groups[1].Value]);
            //heroPlayerInfo.Item_1 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[itemsMatch[index].Groups[2].Value]);
            //heroPlayerInfo.Item_2 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[itemsMatch[index].Groups[3].Value]);
            //heroPlayerInfo.Item_3 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[itemsMatch[index].Groups[4].Value]);
            //heroPlayerInfo.Item_4 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[itemsMatch[index].Groups[5].Value]);
            //heroPlayerInfo.Item_5 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[itemsMatch[index].Groups[6].Value]);
            //heroPlayerInfo.Backpack_0 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[backpackItemsMatch[index].Groups[1].Value]);
            //heroPlayerInfo.Backpack_1 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[backpackItemsMatch[index].Groups[2].Value]);
            //heroPlayerInfo.Backpack_2 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[backpackItemsMatch[index].Groups[3].Value]);
            //#endregion
            //return new HeroPlayerInfoViewModel(heroPlayerInfo);
            return null;
        }

        /// <summary>
        /// 其实和上一个方法基本一样，只不过单独返回团战的json，而且要格式化一下，所以可能很慢，单独做一个方法
        /// </summary>
        /// <param name="matchid"></param>
        /// <returns></returns>
        public static async Task<string> GetTeamfightInfoAsync(string matchid)
        {
            //示例比赛编号3792271763
            string url = String.Format("https://api.opendota.com/api/matches/{0}", matchid);
            HttpClient http = new HttpClient();
            List<string> matchInfoList = new List<string>();
            try
            {
                var response = await http.GetAsync(new Uri(url));
                var jsonMessage = await response.Content.ReadAsStringAsync();

                if (jsonMessage == "{\"error\":\"rate limit exceeded\"}")
                {
                    return "";
                }

                //Match team_fightMatch = Regex.Match(jsonMessage, "(\\\"teamfights\\\":[\\s\\S]*?),\\\"tower_status_dire\\\"");
                //return FormatJson(new StringBuilder(team_fightMatch.Groups[1].Value.Replace("\\", "")));
                return string.Empty;
            }
            catch { return "数据读取错误！请联系开发者yaoyiming123@live.com，我们将尽快解决这个问题，感激不尽！"; }
        }


        /// <summary>
        /// 获取一场比赛的各名玩家的信息
        /// </summary>
        /// <param name="matchid"></param>
        /// <returns></returns>
        public static async Task<List<PlayersInfo>> GetPlayersInfoAsync(string matchid)
        {
            string url = String.Format("https://api.opendota.com/api/matches/{0}", matchid);
            //HttpClientHandler handler = new HttpClientHandler();
            //handler.UseProxy = false;   //不加这个会非常慢
            HttpClient http = new HttpClient();
            List<PlayersInfo> playersInfoList = new List<PlayersInfo>();
            string jsonMessage;
            //try
            //{
            //    if (HeroPlayerInfo.CurrentMatchID != matchid || HeroPlayerInfo.buffer.StartsWith("{\"error\""))
            //    {
            //        var response = await http.GetAsync(new Uri(url));
            //        jsonMessage = await response.Content.ReadAsStringAsync();
            //        HeroPlayerInfo.CurrentMatchID = matchid;
            //        if (jsonMessage == "{\"error\":\"rate limit exceeded\"}")
            //        {
            //            return null;
            //        }
            //    }
            //    else
            //    {
            //        jsonMessage = HeroPlayerInfo.buffer;
            //    }

            //    MatchCollection account_idMatch = Regex.Matches(jsonMessage, "\\\"account_id\\\":([\\s\\S]*?),");
            //    MatchCollection assistsMatch = Regex.Matches(jsonMessage, "\\\"assists\\\":([\\s\\S]*?),");
            //    MatchCollection deathsMatch = Regex.Matches(jsonMessage, "\\\"damage_targets\\\"[\\s\\S]*?,\\\"deaths\\\":([\\s\\S]*?),\\\"denies");
            //    MatchCollection deniesMatch = Regex.Matches(jsonMessage, "\\\"denies\\\":([\\s\\S]*?),");
            //    MatchCollection killsMatch = Regex.Matches(jsonMessage, "\\\"kills\\\":([\\s\\S]*?),\\\"kills_log");
            //    MatchCollection last_hitsMatch = Regex.Matches(jsonMessage, "\\\"last_hits\\\":([\\s\\S]*?),");
            //    MatchCollection levelMatch = Regex.Matches(jsonMessage, "\\\"level\\\":([\\s\\S]*?),");
            //    MatchCollection hero_damageMatch = Regex.Matches(jsonMessage, "\\\"hero_damage\\\":([\\s\\S]*?),");
            //    MatchCollection hero_idItemsMatch = Regex.Matches(jsonMessage, "\\\"hero_id\\\":([\\d]*?),\\\"item_0\\\":([\\s\\S]*?),\\\"item_1\\\":([\\s\\S]*?),\\\"item_2\\\":([\\s\\S]*?),\\\"item_3\\\":([\\s\\S]*?),\\\"item_4\\\":([\\s\\S]*?),\\\"item_5\\\":([\\s\\S]*?),");

            //    //String.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", id);
            //    for (int i = 0; i < 10; i++)
            //    {
            //        string color = "#FFFFFF";
            //        switch (i)
            //        {
            //            case 0:
            //                color = "#3375FF";
            //                break;
            //            case 1:
            //                color = "#66FFBF";
            //                break;
            //            case 2:
            //                color = "#BF00BF";
            //                break;
            //            case 3:
            //                color = "#F3F00B";
            //                break;
            //            case 4:
            //                color = "#FF6B00";
            //                break;
            //            case 5:
            //                color = "#FE86C2";
            //                break;
            //            case 6:
            //                color = "#A1B447";
            //                break;
            //            case 7:
            //                color = "#65D9F7";
            //                break;
            //            case 8:
            //                color = "#008321";
            //                break;
            //            case 9:
            //                color = "#A46900";
            //                break;
            //            default:
            //                color = "#FFFFFF";
            //                break;
            //        }
            //        try
            //        {
            //            playersInfoList.Add(new PlayersInfo
            //            {
            //                Account_id = account_idMatch[i].Groups[1].Value,
            //                Assists = assistsMatch[i].Groups[1].Value,
            //                Deaths = deathsMatch[i].Groups[1].Value,
            //                Denies = deniesMatch[i].Groups[1].Value,
            //                Kills = killsMatch[i].Groups[1].Value,
            //                Last_hits = last_hitsMatch[i].Groups[1].Value,
            //                Level = levelMatch[i].Groups[1].Value,
            //                Hero_damage = hero_damageMatch[i].Groups[1].Value,
            //                Hero_id = hero_idItemsMatch[i].Groups[1].Value,
            //                Item_0 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[hero_idItemsMatch[i].Groups[2].Value]),
            //                Item_1 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[hero_idItemsMatch[i].Groups[3].Value]),
            //                Item_2 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[hero_idItemsMatch[i].Groups[4].Value]),
            //                Item_3 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[hero_idItemsMatch[i].Groups[5].Value]),
            //                Item_4 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[hero_idItemsMatch[i].Groups[6].Value]),
            //                Item_5 = string.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", ConstantsHelper.ItemsDictionary[hero_idItemsMatch[i].Groups[7].Value]),
            //                Color = color
            //            });
            //        }
            //        catch
            //        {
            //            playersInfoList.Add(new PlayersInfo
            //            {
            //                Account_id = "Unknown",
            //                Assists = "0",
            //                Deaths = "0",
            //                Denies = "0",
            //                Kills = "0",
            //                Last_hits = "0",
            //                Level = "0",
            //                Hero_damage = "0",
            //                Hero_id = "1",
            //                Item_0 = string.Format("http://www.dota2.com.cn/items/images/null_lg.png"),
            //                Item_1 = string.Format("http://www.dota2.com.cn/items/images/null_lg.png"),
            //                Item_2 = string.Format("http://www.dota2.com.cn/items/images/null_lg.png"),
            //                Item_3 = string.Format("http://www.dota2.com.cn/items/images/null_lg.png"),
            //                Item_4 = string.Format("http://www.dota2.com.cn/items/images/null_lg.png"),
            //                Item_5 = string.Format("http://www.dota2.com.cn/items/images/null_lg.png"),
            //                Color = color
            //            });
            //        }
            //    }
            //}
            //catch
            //{
            //    return null;
            //}
            return playersInfoList;
        }

    }





    public class PlayersInfo
    {
        public string Account_id { get; set; }
        public string Assists { get; set; }
        public string Deaths { get; set; }
        public string Denies { get; set; }
        public string Kills { get; set; }
        public string Last_hits { get; set; }
        public string Level { get; set; }
        public string Item_0 { get; set; }
        public string Item_1 { get; set; }
        public string Item_2 { get; set; }
        public string Item_3 { get; set; }
        public string Item_4 { get; set; }
        public string Item_5 { get; set; }
        public string Hero_damage { get; set; }
        public string Hero_id { get; set; }
        public string Color { get; set; }
    }

    public class PlayersInfoViewModel
    {
        public string HeroPhoto { get; set; }
        public string PlayerName { get; set; }
        public string Fight_rate { get; set; }  //在外面计算
        public string Damage_rate { get; set; } //在外面计算
        public string LD { get; set; }
        public double Kills { get; set; }
        public double Deaths { get; set; }
        public double Assists { get; set; }
        public string K_D_A { get; set; }
        public string KDA { get; set; }
        public string Item_0 { get; set; }
        public string Item_1 { get; set; }
        public string Item_2 { get; set; }
        public string Item_3 { get; set; }
        public string Item_4 { get; set; }
        public string Item_5 { get; set; }
        public string PlayerPhoto { get; set; }
        public string Level { get; set; }
        public string Hero_damage { get; set; }
        public string Account_id { get; set; }
        public string Color { get; set; }

        public PlayersInfoViewModel(PlayersInfo playersInfo)
        {
            //this.Item_0 = playersInfo.Item_0 == "http://www.dota2.com.cn/items/images/_lg.png" ? "ms-appx:///Assets/Pictures/null.png" : playersInfo.Item_0;
            //this.Item_1 = playersInfo.Item_1 == "http://www.dota2.com.cn/items/images/_lg.png" ? "ms-appx:///Assets/Pictures/null.png" : playersInfo.Item_1;
            //this.Item_2 = playersInfo.Item_2 == "http://www.dota2.com.cn/items/images/_lg.png" ? "ms-appx:///Assets/Pictures/null.png" : playersInfo.Item_2;
            //this.Item_3 = playersInfo.Item_3 == "http://www.dota2.com.cn/items/images/_lg.png" ? "ms-appx:///Assets/Pictures/null.png" : playersInfo.Item_3;
            //this.Item_4 = playersInfo.Item_4 == "http://www.dota2.com.cn/items/images/_lg.png" ? "ms-appx:///Assets/Pictures/null.png" : playersInfo.Item_4;
            //this.Item_5 = playersInfo.Item_5 == "http://www.dota2.com.cn/items/images/_lg.png" ? "ms-appx:///Assets/Pictures/null.png" : playersInfo.Item_5;
            //this.Level = playersInfo.Level;
            //this.Hero_damage = playersInfo.Hero_damage;
            //this.HeroPhoto = ConstantsHelper.dotaHerosDictionary[ConstantsHelper.HeroID[Convert.ToInt32(playersInfo.Hero_id)]].Pic;
            //this.K_D_A = playersInfo.Kills + "/" + playersInfo.Deaths + "/" + playersInfo.Assists;
            //double deaths = playersInfo.Deaths == "0" ? 1 : Convert.ToDouble(playersInfo.Deaths);
            //this.KDA = "KDA：" + ((Convert.ToDouble(playersInfo.Kills) + Convert.ToDouble(playersInfo.Assists)) / deaths).ToString("0.00");
            //this.LD = "正/反：" + playersInfo.Last_hits + "/" + playersInfo.Denies;
            //this.Account_id = playersInfo.Account_id;
            //this.PlayerName = "匿名玩家";
            //this.PlayerPhoto = "ms-appx:///Assets/Pictures/null.png";
            //this.Kills = Convert.ToDouble(playersInfo.Kills);
            //this.Deaths = Convert.ToDouble(playersInfo.Deaths);
            //this.Assists = Convert.ToDouble(playersInfo.Assists);
            //this.Color = playersInfo.Color;
        }
    }

    public class HeroUsingInfo
    {
        public string hero_id { get; set; }
        public string last_played { get; set; }
        public double games { get; set; }
        public double win { get; set; }
        public double with_games { get; set; }
        public double with_win { get; set; }
        public double against_games { get; set; }
        public double against_win { get; set; }
    }

    public class HeroUsingInfoViewModel
    {
        public string Photo { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public double Games { get; set; }
        public double Win { get; set; }
        public double Win_rate { get; set; }
        public double With { get; set; }
        public double With_win { get; set; }
        public double Against { get; set; }
        public double Against_win { get; set; }

        public HeroUsingInfoViewModel() { }
        public HeroUsingInfoViewModel(HeroUsingInfo heroUsingInfo)
        {
            //this.Photo = ConstantsHelper.dotaHerosDictionary[ConstantsHelper.HeroID[Convert.ToInt32(heroUsingInfo.hero_id)]].IconPic;
            //this.Name = ConstantsHelper.dotaHerosDictionary[ConstantsHelper.HeroID[Convert.ToInt32(heroUsingInfo.hero_id)]].Name;
            //this.Games = heroUsingInfo.games;
            //this.Win = heroUsingInfo.win;
            //this.Win_rate = Math.Round(100 * heroUsingInfo.win / heroUsingInfo.games, 1);
            //this.With = heroUsingInfo.with_games;
            //this.With_win = heroUsingInfo.with_win;
            //this.Against = heroUsingInfo.against_games;
            //this.Against_win = heroUsingInfo.against_win;
        }
    }

    public class HeroPlayerInfo
    {
        /// <summary>
        /// 缓存字符串，当查看比赛数据时将获取到的比赛json缓存，避免点击玩家查看其更详细数据时又要用API获取一次
        /// </summary>
        public static string buffer = "";

        /// <summary>
        /// 当前缓存的字符串对应的比赛编号
        /// </summary>
        public static string CurrentMatchID { get; set; }

        public string Item_0 { get; set; }
        public string Item_1 { get; set; }
        public string Item_2 { get; set; }
        public string Item_3 { get; set; }
        public string Item_4 { get; set; }
        public string Item_5 { get; set; }
        public string Backpack_0 { get; set; }
        public string Backpack_1 { get; set; }
        public string Backpack_2 { get; set; }
        public string Hero_damage { get; set; }
        public string Hero_healing { get; set; }
        public string Hero_id { get; set; }
        public string Party_size { get; set; }
        public string Total_gold { get; set; }
        public string Total_xp { get; set; }
        public string Rank_tier { get; set; }
        public string Ability_upgrades_arr { get; set; }
        public string Permanent_buffs { get; set; }
        public string Benchmarks { get; set; }
    }

    public class HeroPlayerInfoViewModel
    {
        public class Buff
        {
            public string Permanent_buff { get; set; }

            private string _stack_count;
            public string Stack_count
            {
                get { return _stack_count; }
                set
                {
                    if (value == "0")
                    {
                        _stack_count = "";
                    }
                    else
                    {
                        _stack_count = value;
                    }
                }
            }
        }

        public class Benchmark
        {
            private string _raw;
            public string Raw
            {
                get { return _raw; }
                set
                {
                    if (value == null || value == "null")
                    {
                        _raw = "Null";
                        return;
                    }
                    if (value.Length > 6)
                    {
                        _raw = value.Substring(0, 6);
                    }
                    else
                    {
                        _raw = value;
                    }
                }
            }

            private string _pct;
            public string Pct
            {
                get { return _pct; }
                set
                {
                    if (value == null || value == "null")
                    {
                        _pct = "Null";
                        return;
                    }
                    if (value.Length > 6)
                    {
                        _pct = value.Substring(2, 2) + "." + value.Substring(4, 2) + "%";
                    }
                    else
                    {
                        _pct = double.Parse(value) * 100 + "%";
                    }
                }
            }
        }


        // 这些取自十个人的总体比赛数据页面
        public string Account_id { get; set; }
        public string Personaname { get; set; }
        public string KDAString { get; set; }
        public string Last_hits { get; set; }
        public string Denies { get; set; }
        public string KDA { get; set; }
        public string Level { get; set; }

        // 这些取自新解析的数据
        public string Hero_id { get; set; }
        public string Hero_name { get; set; }
        public string Hero_photo { get; set; }
        public string Item_0 { get; set; }
        public string Item_1 { get; set; }
        public string Item_2 { get; set; }
        public string Item_3 { get; set; }
        public string Item_4 { get; set; }
        public string Item_5 { get; set; }
        public string Backpack_0 { get; set; }
        public string Backpack_1 { get; set; }
        public string Backpack_2 { get; set; }
        public string Hero_damage { get; set; }
        public string Hero_healing { get; set; }
        public string Party_size { get; set; }
        public string Total_gold { get; set; }
        public string Total_xp { get; set; }
        public List<string> Ability_upgrades_arr { get; set; }
        public List<Buff> Permanent_buffs { get; set; }
        public string Rank_tier { get; set; }

        //这三个数据直接从benchmark取
        public string GPM { get; set; }
        public string XPM { get; set; }
        public string Tower_damage { get; set; }

        /// <summary>
        /// GPM排名
        /// </summary>
        public Benchmark Benchmarks_gpm { get; set; }

        /// <summary>
        /// XPM排名
        /// </summary>
        public Benchmark Benchmarks_xpm { get; set; }

        /// <summary>
        /// Kill per minute排名
        /// </summary>
        public Benchmark Benchmarks_kill_per_minute { get; set; }

        /// <summary>
        /// last hit per minute排名
        /// </summary>
        public Benchmark Benchmarks_last_hit_per_minute { get; set; }

        /// <summary>
        /// hero damage per minute排名
        /// </summary>
        public Benchmark Benchmarks_hero_damage_per_minute { get; set; }

        /// <summary>
        /// hero healing per minute排名
        /// </summary>
        public Benchmark Benchmarks_hero_healing_per_minute { get; set; }

        /// <summary>
        /// tower damage排名
        /// </summary>
        public Benchmark Benchmarks_tower_damage { get; set; }

        public HeroPlayerInfoViewModel(HeroPlayerInfo heroPlayerInfo)
        {
            //List<Benchmark> got = GetBenchmarks(heroPlayerInfo.Benchmarks);
            //this.Benchmarks_gpm = got[0];
            //this.Benchmarks_hero_damage_per_minute = got[4];
            //this.Benchmarks_hero_healing_per_minute = got[5];
            //this.Benchmarks_kill_per_minute = got[2];
            //this.Benchmarks_last_hit_per_minute = got[3];
            //this.Benchmarks_tower_damage = got[6];
            //this.Benchmarks_xpm = got[1];

            //this.Ability_upgrades_arr = GetAbility_upgrades_arr(heroPlayerInfo.Ability_upgrades_arr);
            //this.Permanent_buffs = GetBuffs(heroPlayerInfo.Permanent_buffs);
            //this.Backpack_0 = heroPlayerInfo.Backpack_0;
            //this.Backpack_1 = heroPlayerInfo.Backpack_1;
            //this.Backpack_2 = heroPlayerInfo.Backpack_2;
            //this.Item_0 = heroPlayerInfo.Item_0;
            //this.Item_1 = heroPlayerInfo.Item_1;
            //this.Item_2 = heroPlayerInfo.Item_2;
            //this.Item_3 = heroPlayerInfo.Item_3;
            //this.Item_4 = heroPlayerInfo.Item_4;
            //this.Item_5 = heroPlayerInfo.Item_5;
            //this.GPM = this.Benchmarks_gpm.Raw;
            //this.Hero_damage = heroPlayerInfo.Hero_damage;
            //this.Hero_healing = heroPlayerInfo.Hero_healing;
            //this.Party_size = heroPlayerInfo.Party_size;
            //this.Total_gold = heroPlayerInfo.Total_gold;
            //this.Total_xp = heroPlayerInfo.Total_xp;
            //this.Tower_damage = this.Benchmarks_tower_damage.Raw;
            //this.XPM = this.Benchmarks_xpm.Raw;
            //this.Rank_tier = heroPlayerInfo.Rank_tier;

            //this.Hero_id = heroPlayerInfo.Hero_id;
            //this.Hero_name = ConstantsHelper.dotaHerosDictionary[ConstantsHelper.HeroID[Convert.ToInt32(heroPlayerInfo.Hero_id)]].Name;
            //this.Hero_photo = ConstantsHelper.dotaHerosDictionary[ConstantsHelper.HeroID[Convert.ToInt32(heroPlayerInfo.Hero_id)]].Pic;
        }

        private List<Benchmark> GetBenchmarks(string regular)
        {
            Match gold_per_minMatch = Regex.Match(regular, "\\\"gold_per_min\\\":{\\\"raw\\\":([\\d\\D]*?),\\\"pct\\\":([\\d\\D]*?)}");
            Match xp_per_minMatch = Regex.Match(regular, "\\\"xp_per_min\\\":{\\\"raw\\\":([\\d\\D]*?),\\\"pct\\\":([\\d\\D]*?)}");
            Match kill_per_minMatch = Regex.Match(regular, "\\\"kills_per_min\\\":{\\\"raw\\\":([\\d\\D]*?),\\\"pct\\\":([\\d\\D]*?)}");
            Match lh_per_minMatch = Regex.Match(regular, "\\\"last_hits_per_min\\\":{\\\"raw\\\":([\\d\\D]*?),\\\"pct\\\":([\\d\\D]*?)}");
            Match hero_damage_per_minMatch = Regex.Match(regular, "\\\"hero_damage_per_min\\\":{\\\"raw\\\":([\\d\\D]*?),\\\"pct\\\":([\\d\\D]*?)}");
            Match hero_healing_per_minMatch = Regex.Match(regular, "\\\"hero_healing_per_min\\\":{\\\"raw\\\":([\\d\\D]*?),\\\"pct\\\":([\\d\\D]*?)}");
            Match tower_damage_per_minMatch = Regex.Match(regular, "\\\"tower_damage\\\":{\\\"raw\\\":([\\d\\D]*?),\\\"pct\\\":([\\d\\D]*?)}");
            List<Benchmark> benchmarks = new List<Benchmark>();
            benchmarks.Add(new Benchmark { Raw = gold_per_minMatch.Groups[1].Value, Pct = gold_per_minMatch.Groups[2].Value });
            benchmarks.Add(new Benchmark { Raw = xp_per_minMatch.Groups[1].Value, Pct = xp_per_minMatch.Groups[2].Value });
            benchmarks.Add(new Benchmark { Raw = kill_per_minMatch.Groups[1].Value, Pct = kill_per_minMatch.Groups[2].Value });
            benchmarks.Add(new Benchmark { Raw = lh_per_minMatch.Groups[1].Value, Pct = lh_per_minMatch.Groups[2].Value });
            benchmarks.Add(new Benchmark { Raw = hero_damage_per_minMatch.Groups[1].Value, Pct = hero_damage_per_minMatch.Groups[2].Value });
            benchmarks.Add(new Benchmark { Raw = hero_healing_per_minMatch.Groups[1].Value, Pct = hero_healing_per_minMatch.Groups[2].Value });
            benchmarks.Add(new Benchmark { Raw = tower_damage_per_minMatch.Groups[1].Value, Pct = tower_damage_per_minMatch.Groups[2].Value });
            return benchmarks;
        }

        private List<Buff> GetBuffs(string regular)
        {
            MatchCollection buffsCollection = Regex.Matches(regular, "{\\\"permanent_buff\\\":([\\d\\D]*?),\\\"stack_count\\\":([\\d\\D]*?)}");
            List<Buff> buffsList = new List<Buff>();
            for (int i = 0; i < buffsCollection.Count; i++)
            {
                buffsList.Add(new Buff { Permanent_buff = buffsCollection[i].Groups[1].Value, Stack_count = buffsCollection[i].Groups[2].Value });
            }
            return buffsList;
        }

        private List<string> GetAbility_upgrades_arr(string regular)
        {
            string[] aua = regular.Replace("\"", "").Split(",");
            return aua.ToList();
        }

    }

    public class AbilityViewModel
    {
        public string ID { get; set; }
        public string Ability { get; set; }
    }

}
