﻿using System;
using System.Text.Json;
using System.Threading;
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
        public static async Task<DotaHeroDataModel?> GetHeroData(int heroId, string language, CancellationToken cancellationToken)
        {
            string url = string.Format("https://www.dota2.com/datafeed/herodata?language={0}&hero_id={1}", language, heroId);

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url)).AsTask(cancellationToken);
                var json = await response.Content.ReadAsStringAsync().AsTask(cancellationToken);
                var heroDataResponse = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DotaHeroDataResponse);
                if (heroDataResponse?.result?.status == 1 && heroDataResponse.result.data?.heroes?.Length > 0)
                {
                    return heroDataResponse.result.data.heroes[0];
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex) { LogCourier.Log($"GetHeroData({heroId}, {language}) error: {ex.Message}", LogCourier.LogType.Error); }

            return null;
        }

        /// <summary>
        /// 获取英雄的排行榜数据
        /// </summary>
        /// <param name="heroId"></param>
        /// <returns></returns>
        public static async Task<DotaHeroRankingModel[]> GetHeroRankings(int heroId, CancellationToken cancellationToken)
        {
            string url = string.Format("https://api.opendota.com/api/rankings?hero_id={0}", heroId);

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url)).AsTask(cancellationToken);
                var json = await response.Content.ReadAsStringAsync().AsTask(cancellationToken);
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
            catch (TaskCanceledException) { }
            catch (Exception ex) { LogCourier.Log($"GetHeroRankings({heroId}) error: {ex.Message}", LogCourier.LogType.Error); }

            return [];
        }

        /// <summary>
        /// 获取Steam游戏当前在线玩家数量
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        public static async Task<int> GetNumberOfCurrentPlayers(string appid = "570")
        {
            string url = $"http://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1?appid={appid}&format=json";

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url));
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.SteamNumberOfCurrentPlayersResponse);
                if (result?.response?.result == 1)
                {
                    return result.response.player_count;
                }
            }
            catch (Exception ex) { LogCourier.Log($"GetNumberOfCurrentPlayers error: {ex.Message}", LogCourier.LogType.Error); }

            return -1;
        }

        /// <summary>
        /// 获取玩家的个人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<DotaPlayerProfileModel?> GetPlayerProfile(string id, CancellationToken cancellationToken)
        {
            string url = $"https://api.opendota.com/api/players/{id}";

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url)).AsTask(cancellationToken);
                var json = await response.Content.ReadAsStringAsync().AsTask(cancellationToken);
                var result = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DotaPlayerProfileModel);
                if (result is not null)
                {
                    if (result.leaderboard_rank > 0 && result.rank_tier >= 80)
                    {
                        if (result.leaderboard_rank == 1)
                        {
                            result.rank_tier = 84;
                        }
                        else if (result.leaderboard_rank <= 10)
                        {
                            result.rank_tier = 83;
                        }
                        else if (result.leaderboard_rank <= 100)
                        {
                            result.rank_tier = 82;
                        }
                        else if (result.leaderboard_rank <= 1000)
                        {
                            result.rank_tier = 81;
                        }
                    }

                    return result;
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex) { LogCourier.Log($"GetPlayerProfile({id}) error: {ex.Message}", LogCourier.LogType.Error); }

            return null;
        }

        /// <summary>
        /// 获取玩家的胜负场数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<DotaPlayerWinLoseModel?> GetPlayerWinLose(string id, CancellationToken cancellationToken)
        {
            string url = $"https://api.opendota.com/api/players/{id}/wl";

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url)).AsTask(cancellationToken);
                var json = await response.Content.ReadAsStringAsync().AsTask(cancellationToken);
                var result = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DotaPlayerWinLoseModel);
                if (result is not null)
                {
                    return result;
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex) { LogCourier.Log($"GetPlayerWinLose({id}) error: {ex.Message}", LogCourier.LogType.Error); }

            return null;
        }

        /// <summary>
        /// 获取玩家的全期数据表现
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<DotaPlayerOverallPerformanceModel[]> GetPlayerOverallPerformances(string id, CancellationToken cancellationToken)
        {
            string url = $"https://api.opendota.com/api/players/{id}/totals";

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url)).AsTask(cancellationToken);
                var json = await response.Content.ReadAsStringAsync().AsTask(cancellationToken);
                var result = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DotaPlayerOverallPerformanceModelArray);
                if (result is not null)
                {
                    return result;
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex) { LogCourier.Log($"GetPlayerOverallPerformance({id}) error: {ex.Message}", LogCourier.LogType.Error); }

            return [];
        }

        /// <summary>
        /// 获取玩家的全期英雄胜率表现
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<DotaPlayerHeroPerformanceModel[]> GetPlayerHeroPerformances(string id, CancellationToken cancellationToken)
        {
            string url = $"https://api.opendota.com/api/players/{id}/heroes";

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url)).AsTask(cancellationToken);
                var json = await response.Content.ReadAsStringAsync().AsTask(cancellationToken);
                var result = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DotaPlayerHeroPerformanceModelArray);
                if (result is not null)
                {
                    return result;
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex) { LogCourier.Log($"GetPlayerHeroPerformance({id}) error: {ex.Message}", LogCourier.LogType.Error); }

            return [];
        }

        /// <summary>
        /// 获取玩家最近的比赛记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<DotaMatchModel[]> GetPlayerRecentMatches(string id, CancellationToken cancellationToken)
        {
            string url = $"https://api.opendota.com/api/players/{id}/recentMatches";

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url)).AsTask(cancellationToken);
                var json = await response.Content.ReadAsStringAsync().AsTask(cancellationToken);
                var result = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DotaMatchModelArray);
                if (result is not null)
                {
                    return result;
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex) { LogCourier.Log($"GetPlayerRecentMatches({id}) error: {ex.Message}", LogCourier.LogType.Error); }

            return [];
        }

        /// <summary>
        /// 获取玩家所有比赛记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<DotaMatchModel[]> GetPlayerAllMatches(string id, CancellationToken cancellationToken)
        {
            string url = $"https://api.opendota.com/api/players/{id}/matches";

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url)).AsTask(cancellationToken);
                var json = await response.Content.ReadAsStringAsync().AsTask(cancellationToken);
                var result = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DotaMatchModelArray);
                if (result is not null)
                {
                    return result;
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex) { LogCourier.Log($"GetPlayerAllMatches({id}) error: {ex.Message}", LogCourier.LogType.Error); }

            return [];
        }

        /// <summary>
        /// 获取特定比赛的详细数据
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<DotaMatchDataModel?> GetMatchData(string matchId, CancellationToken cancellationToken)
        {
            string url = $"https://api.opendota.com/api/matches/{matchId}";

            try
            {
                var response = await _apiHttpClient.GetAsync(new Uri(url)).AsTask(cancellationToken);
                var json = await response.Content.ReadAsStringAsync().AsTask(cancellationToken);
                var result = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DotaMatchDataModel);
                if (result is not null)
                {
                    return result;
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex) { LogCourier.Log($"GetMatchData({matchId}) error: {ex.Message}", LogCourier.LogType.Error); }

            return null;
        }
    }
}
