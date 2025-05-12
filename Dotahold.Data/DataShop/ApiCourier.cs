using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dotahold.Data.Models;

namespace Dotahold.Data.DataShop
{
    /// <summary>
    /// 这是一只信使，帮你运送各种接口返回的数据
    /// </summary>
    public static class ApiCourier
    {
        private static readonly Windows.Web.Http.HttpClient _apiHttpClient = new();

        /// <summary>
        /// 获取英雄详情信息
        /// </summary>
        /// <param name="heroId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static async Task<DotaHeroDataModel?> GetHeroData(int heroId, string language = "english")
        {
            string url = string.Format("https://www.dota2.com/datafeed/herodata?language={0}&hero_id={1}", language, heroId);

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url));
                var json = await response.Content.ReadAsStringAsync();
                var heroDataResponse = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DotaHeroDataResponse);
                if (heroDataResponse?.result?.status == 1 && heroDataResponse.result.data?.heroes?.Length > 0)
                {
                    return heroDataResponse.result.data.heroes[0];
                }
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }

            return null;
        }

        /// <summary>
        /// 获取英雄的排行榜数据
        /// </summary>
        /// <param name="heroId"></param>
        /// <returns></returns>
        public static async Task<DotaHeroRankingModel[]?> GetHeroRankings(int heroId)
        {
            string url = string.Format("https://api.opendota.com/api/rankings?hero_id={0}", heroId);

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url));
                var json = await response.Content.ReadAsStringAsync();
                var heroDataResponse = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DotaHeroRankingResponse);
                if (heroDataResponse?.hero_id == heroId && heroDataResponse.rankings is not null)
                {
                    return heroDataResponse.rankings;
                }
                else
                {
                    throw new Exception($"Hero ID mismatch, {heroId} != {heroDataResponse?.hero_id}");
                }
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }

            return [];
        }
    }
}
