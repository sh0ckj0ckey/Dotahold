using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenDota_UWP.Helpers
{
    public class HeroBio
    {
        public string Atk { get; set; }
        public string Role { get; set; }
        public string Bio { get; set; }
    }

    public class HeroTalent
    {
        public string Talent_10_left { get; set; }
        public string Talent_10_right { get; set; }
        public string Talent_15_left { get; set; }
        public string Talent_15_right { get; set; }
        public string Talent_20_left { get; set; }
        public string Talent_20_right { get; set; }
        public string Talent_25_left { get; set; }
        public string Talent_25_right { get; set; }
    }

    public class HeroAbility
    {
        public string PictureUrl { get; set; }
        public string Name { get; set; }
        public string Intro { get; set; }
        public string Tips { get; set; }
        public string Mana { get; set; }
        public string CoolDown { get; set; }
        public string Info { get; set; }
        public string Background { get; set; }
    }

    public static class CrawlerHelper
    {
        /// <summary>
        /// 获取指定英雄的页面的HTML
        /// </summary>
        /// <param name="id"></param>
        public async static Task<string> InitializeHtml(string id)
        {
            string html = await GetHtml("http://www.dota2.com.cn/hero/" + id + "/");
            if (html == null)
            {
                return "gg";
            }
            return html;
        }

        /// <summary>
        /// 获取指定网址的HTML
        /// </summary>
        /// <param name="webUrl"></param>
        /// <returns></returns>
        public async static Task<string> GetHtml(string webUrl)
        {
            //string htmlStr = "";
            //if (!String.IsNullOrEmpty(webUrl))
            //{
            //    ServicePointManager.DefaultConnectionLimit = 100;
            //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(webUrl);         //实例化WebRequest对象  
            //    request.Method = "GET";
            //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();           //创建WebResponse对象  
            //    Stream datastream = response.GetResponseStream();       //创建流对象  
            //    StreamReader reader = new StreamReader(datastream, Encoding.Default);
            //    htmlStr = reader.ReadToEnd();                           //读取数据  
            //    reader.Close();
            //    datastream.Close();
            //    response.Close();
            //}
            string jsonMessage;
            HttpClient http = new HttpClient();
            try
            {
                var response = await http.GetAsync(new Uri(webUrl));
                jsonMessage = await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
            return jsonMessage;
        }

        /// <summary>
        /// 获得英雄的定位和背景
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<HeroBio> GetHeroBio(string id, string html)
        {
            if (html == null)
            {
                await InitializeHtml(id);
            }
            HeroBio heroBio = new HeroBio();
            try
            {
                MatchCollection matchCollection = Regex.Matches(html, "<span\\sclass=\\\"_btn_type\\\"\\sstar=\\\"\\d\\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t \\stype=\\\"(.*?)\\\">");
                Match matchAtk = Regex.Match(html, "攻击类型：</span>\\n\\n\\t\\t\\t\\t\\t\\t\\t\\t<p\\sclass=\\\"info_p\\\"><span>(.*?)</span>");
                Match matchBio = Regex.Match(html, "</a>\\n\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t\\t([\\s\\S]*?)\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t<div class=\\\"silide_r\\\">");
                heroBio.Atk = matchAtk.Groups[1].Value;
                heroBio.Bio = matchBio.Groups[1].Value.Replace("<br>", "");
                string role = "";
                foreach (Match item in matchCollection)
                {
                    role += "-";
                    role += item.Groups[1].Value;
                }
                heroBio.Role = role;
            }
            catch
            {
                return null;
            }
            return heroBio;
        }

        /// <summary>
        /// 备用获取英雄背景故事，只有背景故事
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<string> GetHeroBioBackup(string id, string html)
        {
            string jsonMessage;
            HttpClient http = new HttpClient();
            // http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Accept-Language: zh-CN,zh;q=0.9,en;q=0.8"));
            try
            {
                var response = await http.GetAsync(new Uri("https://www.dota2.com/hero/" + id + "/"));
                jsonMessage = await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return "英雄背景故事获取失败，可能是网络有问题，请稍后重试。";
            }
            try
            {
                Match matchBio = Regex.Match(html, "<div id=\\\"bioInner\\\">\\r\\n\\t\\t\\t\\t\\t\\\t\\t\\\t\\t([\\s\\S]*?)\\t\\t");
                return matchBio.Groups[1].Value.Trim().Replace(" ", "") == "" ? "英雄背景故事获取失败，可能是网络有问题，请稍后重试。" : matchBio.Groups[1].Value;
            }
            catch
            {
                return "英雄背景故事获取失败，可能是网络有问题，请稍后重试。";
            }
        }


        /// <summary>
        /// 获得英雄的天赋树
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<HeroTalent> GetHeroTalent(string id, string html)
        {
            if (html == null)
            {
                await InitializeHtml(id);
            }
            HeroTalent heroTalent = new HeroTalent();
            try
            {
                MatchCollection matchCollection = Regex.Matches(html, "<div class=\\\"talent-explain\\\">(.*?)</div>");
                heroTalent.Talent_25_left = matchCollection[0].Groups[1].Value.Replace("\\n", "\n");
                heroTalent.Talent_25_right = matchCollection[1].Groups[1].Value.Replace("\\n", "\n");
                heroTalent.Talent_20_left = matchCollection[2].Groups[1].Value.Replace("\\n", "\n");
                heroTalent.Talent_20_right = matchCollection[3].Groups[1].Value.Replace("\\n", "\n");
                heroTalent.Talent_15_left = matchCollection[4].Groups[1].Value.Replace("\\n", "\n");
                heroTalent.Talent_15_right = matchCollection[5].Groups[1].Value.Replace("\\n", "\n");
                heroTalent.Talent_10_left = matchCollection[6].Groups[1].Value.Replace("\\n", "\n");
                heroTalent.Talent_10_right = matchCollection[7].Groups[1].Value.Replace("\\n", "\n");
            }
            catch
            {
                return null;
            }
            return heroTalent;
        }

        /// <summary>
        /// 获得英雄的技能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<List<HeroAbility>> GetHeroAbility(string id, string html)
        {
            if (html == null)
            {
                await InitializeHtml(id);
            }
            List<HeroAbility> list = new List<HeroAbility>();
            try
            {
                //MatchCollection matchCollection = Regex.Matches(Html, "<dd jnxs[\\S\\s]*?src=\\\"(.*?)\\\"/>[\\s\\S]*?<div class=\\\"skill_info\\\">[\\s\\S]*?<p class=\\\"skill_intro\\\">[\\s\\S]*?<span>(.*?)</span><br/>\\n\\t*?(.*?)\\t*?</p>\\n\\t*?<p class=\\\"color_green\\\">\\n\\t*?(.*?)\\t*?</p>[\\S\\s]*?魔法消耗：(.*?)</div>[\\s\\S]*?冷却时间：(.*?)</div>[\\S\\s]*?(技能[\\s\\S]*?)</ul>[\\S\\s]*?<div class=\\\"skill_bot\\\">(.*?)</div>[\\n]+[\\t]+</dd>");
                MatchCollection src = Regex.Matches(html, "src=\\\"//(.*?)\\\"/>\\n\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t<div class=\\\"skill_info\\\">");
                MatchCollection basis = Regex.Matches(html, "<div class=\\\"skill_info\\\">[\\s\\S]*?<p class=\\\"skill_intro\\\">([\\s\\S]*?)<div class=\\\"xiaohao_wrap\\\">");
                MatchCollection mana = Regex.Matches(html, "魔法消耗：(.*?)</div>");
                MatchCollection coolDown = Regex.Matches(html, "冷却时间：(.*?)</div>");
                MatchCollection info = Regex.Matches(html, "class=\\\"color_white\\\">(技能:[\\s\\S]*?)</ul>");
                MatchCollection background = Regex.Matches(html, "<div class=\\\"skill_bot\\\">([\\s\\S]*?)</div>");
                for (int i = 0; i < basis.Count; i++)
                {
                    HeroAbility heroAbility = new HeroAbility();
                    heroAbility.PictureUrl = "https://" + src[i].Groups[1].Value;
                    string basisStr = basis[i].Groups[1].Value.Replace("\t", "").Replace("\n", "").Replace("</br>", "\r\n").Replace("<span>", "~").Replace("</span><br/>", "~").Replace("<p class=\"color_green\">", "~").Replace("</p>", "~").Replace("<br>", " ");
                    string[] basisSplit = basisStr.Split('~');
                    heroAbility.Name = basisSplit[1];
                    heroAbility.Intro = basisSplit[2];
                    heroAbility.Tips = basisSplit.Length <= 4 ? "" : basisSplit[4];
                    heroAbility.Mana = mana[i].Groups[1].Value;
                    heroAbility.CoolDown = coolDown[i].Groups[1].Value;
                    heroAbility.Info = info[i].Groups[1].Value.Replace("span", "").Replace("<li>", "").Replace("\t", "").Replace("\n", "").Replace("</li>", "\r\n").Replace("class=\"color_white\"", "").Replace("<", "").Replace(">", "").Replace("\\", "").Replace("：/", ": ").Replace(":/", ": ");
                    heroAbility.Background = background[i].Groups[1].Value;
                    list.Add(heroAbility);
                }
            }
            catch
            {
                return null;
            }
            return list;
        }

    }
}
