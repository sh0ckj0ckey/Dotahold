using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Helpers
{
    public class DotaItemHelper
    {

        /// <summary>
        /// 通过API获取物品信息
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetItemDataAsync()
        {
            string url = "http://www.dota2.com.cn/items/json";
            Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient();
            var response = await http.GetAsync(new Uri(url));
            var jsonMessage = await response.Content.ReadAsStringAsync();
            return jsonMessage;
        }

        public static async Task<string> GetItemDataENAsync()
        {
            string url = String.Format("http://www.dota2.com/jsfeed/itemdata");
            Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient();
            var response = await http.GetAsync(new Uri(url));
            var jsonMessage = await response.Content.ReadAsStringAsync();
            return jsonMessage;
        }
    }
}
