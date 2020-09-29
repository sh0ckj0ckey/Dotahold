using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenDota_UWP.Helpers
{
    public class HeroAttr
    {
        public string Base_health { get; set; }
        public string Base_health_regen { get; set; }
        public string Base_mana { get; set; }
        public string Base_mana_regen { get; set; }
        public string Base_armor { get; set; }
        public string Base_attack_min { get; set; }
        public string Base_attack_max { get; set; }
        public string Base_str { get; set; }
        public string Base_agi { get; set; }
        public string Base_int { get; set; }
        public string Str_gain { get; set; }
        public string Agi_gain { get; set; }
        public string Int_gain { get; set; }
        public string Attack_range { get; set; }
        public string Projectile_speed { get; set; }
        public string Attack_rate { get; set; }
        public string Move_speed { get; set; }
        public string Turn_rate { get; set; }
        public string Cm_enabled { get; set; }

        public string _1_pick { get; set; }
        public string _2_pick { get; set; }
        public string _3_pick { get; set; }
        public string _4_pick { get; set; }
        public string _5_pick { get; set; }
        public string _6_pick { get; set; }
        public string _7_pick { get; set; }
        public string _8_pick { get; set; }
        public string _1_win { get; set; }
        public string _2_win { get; set; }
        public string _3_win { get; set; }
        public string _4_win { get; set; }
        public string _5_win { get; set; }
        public string _6_win { get; set; }
        public string _7_win { get; set; }
        public string _8_win { get; set; }
    }
    public class RankPlayer
    {
        public string account_id { get; set; }
        public string score { get; set; }
        public string personaname { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string last_login { get; set; }
        public string rank_tier { get; set; }

        public int Rank { get; set; }
    }
    public class RankPlayers
    {
        public string hero_id { get; set; }
        public List<RankPlayer> rankings { get; set; }
    }

    public class DotaHeroHelper
    {
        public static string _data = "";

        //保存英雄属性数据到本地
        public static async void SaveToFile()
        {
            await StorageFileHelper.WriteAsync(_data, "HeroesAttrCache.dota");
        }

        //从本地文件读取数据
        public static async Task<bool> LoadFromFile()
        {
            _data = await StorageFileHelper.ReadAsync<string>("HeroesAttrCache.dota");
            return (_data != null);
        }
        //获取数据
        public static async Task<string> Getdata()
        {
            if (_data == null)
            {
                bool isExist = await LoadFromFile();
                if (!isExist)
                {
                    _data = "";
                }
            }
            return _data;
        }

        /// <summary>
        /// 通过API获取英雄的三围成长信息并存到本地
        /// (更新数据的话直接调用这个方法就可以了)
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> DownloadHeroAttributesDataAsync()
        {
            //string url = String.Format("https://www.dota2.com/jsfeed/heropediadata?feeds=herodata");
            string url = "https://api.opendota.com/api/heroStats";
            Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient();
            string jsonMessage;
            try
            {
                var response = await http.GetAsync(new Uri(url));
                jsonMessage = await response.Content.ReadAsStringAsync();
                if (jsonMessage.Length < 256)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            //然后把这一长串字符串存到本地
            try
            {
                if (jsonMessage.Length < 256)
                {
                    return false;
                }
                DotaHeroHelper._data = jsonMessage;
                DotaHeroHelper.SaveToFile();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取本地存储的三围成长信息
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetHeroAttributesDataAsync()
        {
            return await DotaHeroHelper.Getdata();
        }

        /// <summary>
        /// 使用正则表达式获取指定英雄的属性
        /// </summary>
        /// <returns></returns>
        public static async Task<HeroAttr> GetHeroAttr(string id)
        {
            if (_data.Length < 256)
            {
                await DotaHeroHelper.GetHeroAttributesDataAsync();
            }
            HeroAttr result = null;
            try
            {
                Match match = Regex.Match(_data, "{\\\"id\\\":" + id + ",[\\d\\D]*?}");
                string jsonPart = match.Groups[0].Value
                    .Replace("1_pick", "_1_pick")
                    .Replace("2_pick", "_2_pick")
                    .Replace("3_pick", "_3_pick")
                    .Replace("4_pick", "_4_pick")
                    .Replace("5_pick", "_5_pick")
                    .Replace("6_pick", "_6_pick")
                    .Replace("7_pick", "_7_pick")
                    .Replace("8_pick", "_8_pick")
                    .Replace("1_win", "_1_win")
                    .Replace("2_win", "_2_win")
                    .Replace("3_win", "_3_win")
                    .Replace("4_win", "_4_win")
                    .Replace("5_win", "_5_win")
                    .Replace("6_win", "_6_win")
                    .Replace("7_win", "_7_win")
                    .Replace("8_win", "_8_win");
                result = JsonConvert.DeserializeObject<HeroAttr>(jsonPart);
            }
            catch
            {
                return null;
            }
            return result;
            //Match matchAttr = Regex.Match(_data, "\\\"id\\\":" + id + ",[\\d\\D]*?\\\"base_health\\\":([\\d\\D]*?)," +
            //    "\\\"base_health_regen\\\":([\\d\\D]*?),\\\"base_mana\\\":([\\d\\D]*?),\\\"base_mana_regen\\\":" +
            //    "([\\d\\D]*?),\\\"base_armor\\\":([\\d\\D]*?),\\\"[\\d\\D]*?,\\\"base_attack_min\\\":([\\d\\D]*?)," +
            //    "\\\"base_attack_max\\\":([\\d\\D]*?),\\\"base_str\\\":([\\d\\D]*?),\\\"base_agi\\\":([\\d\\D]*?)," +
            //    "\\\"base_int\\\":([\\d\\D]*?),\\\"str_gain\\\":([\\d\\D]*?),\\\"agi_gain\\\":([\\d\\D]*?)," +
            //    "\\\"int_gain\\\":([\\d\\D]*?),\\\"attack_range\\\":([\\d\\D]*?)," +
            //    "\\\"projectile_speed\\\":([\\d\\D]*?),\\\"attack_rate\\\":([\\d\\D]*?)," +
            //    "\\\"move_speed\\\":([\\d\\D]*?),\\\"turn_rate\\\":([\\d\\D]*?)," +
            //    "\\\"cm_enabled\\\":([\\d\\D]*?),\\\"[\\d\\D]*?\\\"pro_ban\\\":[\\d\\D]*?,([\\d\\D]*?)}");
            //HeroAttr hero = new HeroAttr();
            //hero.Base_health = matchAttr.Groups[1].Value;
            //hero.Base_health_regen = matchAttr.Groups[2].Value;
            //hero.Base_mana = matchAttr.Groups[3].Value;
            //hero.Base_mana_regen = matchAttr.Groups[4].Value;
            //hero.Base_armor = matchAttr.Groups[5].Value;
            //hero.Base_attack_min = matchAttr.Groups[6].Value;
            //hero.Base_attack_max = matchAttr.Groups[7].Value;
            //hero.Base_str = matchAttr.Groups[8].Value;
            //hero.Base_agi = matchAttr.Groups[9].Value;
            //hero.Base_int = matchAttr.Groups[10].Value;
            //hero.Str_gain = matchAttr.Groups[11].Value;
            //hero.Agi_gain = matchAttr.Groups[12].Value;
            //hero.Int_gain = matchAttr.Groups[13].Value;
            //hero.Attack_range = matchAttr.Groups[14].Value;
            //hero.Projectile_speed = matchAttr.Groups[15].Value;
            //hero.Attack_rate = matchAttr.Groups[16].Value;
            //hero.Move_speed = matchAttr.Groups[17].Value;
            //hero.Turn_rate = matchAttr.Groups[18].Value;
            //hero.Cm_enabled = matchAttr.Groups[19].Value;

            //string pick_win = matchAttr.Groups[20].Value;
            //Match pick1Match = Regex.Match(pick_win, "\\\"1_pick\\\":([\\d\\D]*?)");
            //Match pick2Match = Regex.Match(pick_win, "\\\"2_pick\\\":([\\d\\D]*?)");
            //Match pick3Match = Regex.Match(pick_win, "\\\"3_pick\\\":([\\d\\D]*?)");
            //Match pick4Match = Regex.Match(pick_win, "\\\"4_pick\\\":([\\d\\D]*?)");
            //Match pick5Match = Regex.Match(pick_win, "\\\"5_pick\\\":([\\d\\D]*?)");
            //Match pick6Match = Regex.Match(pick_win, "\\\"6_pick\\\":([\\d\\D]*?)");
            //Match pick7Match = Regex.Match(pick_win, "\\\"7_pick\\\":([\\d\\D]*?)");
            //Match win1Match = Regex.Match(pick_win, "\\\"1_win\\\":([\\d\\D]*?)");
            //Match win2Match = Regex.Match(pick_win, "\\\"2_win\\\":([\\d\\D]*?)");
            //Match win3Match = Regex.Match(pick_win, "\\\"3_win\\\":([\\d\\D]*?)");
            //Match win4Match = Regex.Match(pick_win, "\\\"4_win\\\":([\\d\\D]*?)");
            //Match win5Match = Regex.Match(pick_win, "\\\"5_win\\\":([\\d\\D]*?)");
            //Match win6Match = Regex.Match(pick_win, "\\\"6_win\\\":([\\d\\D]*?)");
            //Match win7Match = Regex.Match(pick_win, "\\\"7_win\\\":([\\d\\D]*?)");

            //hero._1_pick = pick1Match.Groups[1].Value;
            //hero._2_pick = pick2Match.Groups[1].Value;
            //hero._3_pick = pick3Match.Groups[1].Value;
            //hero._4_pick = pick4Match.Groups[1].Value;
            //hero._5_pick = pick5Match.Groups[1].Value;
            //hero._6_pick = pick6Match.Groups[1].Value;
            //hero._7_pick = pick7Match.Groups[1].Value;
            //hero._1_win = win1Match.Groups[1].Value;
            //hero._2_win = win2Match.Groups[1].Value;
            //hero._3_win = win3Match.Groups[1].Value;
            //hero._4_win = win4Match.Groups[1].Value;
            //hero._5_win = win5Match.Groups[1].Value;
            //hero._6_win = win6Match.Groups[1].Value;
            //hero._7_win = win7Match.Groups[1].Value;
        }

        /// <summary>
        /// 获取这个英雄的前100名玩家
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<List<RankPlayer>> GetHeroPlayersAsync(string id)
        {
            string jsonMessage;
            try
            {
                string url = String.Format("https://api.opendota.com/api/rankings?hero_id={0}", id);
                Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient();
                var response = await http.GetAsync(new Uri(url));
                jsonMessage = await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
            if (jsonMessage.Length < 256)
            {
                return null;
            }
            RankPlayers result = null;
            try
            {
                result = JsonConvert.DeserializeObject<RankPlayers>(jsonMessage);
            }
            catch
            {
                return null;
            }
            return result.rankings;
        }

    }
}
