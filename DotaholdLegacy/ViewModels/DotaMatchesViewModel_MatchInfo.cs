namespace Dotahold.ViewModels
{
    public partial class DotaMatchesViewModel : ViewModelBase
    {
        /// <summary>
        /// 分析当前选择玩家的数据
        /// </summary>
        /// <returns></returns>
        public async void AnalyzePlayerInfo(Models.Player player)
        {
            try
            {
                if (player != null)
                {
                    // benchmarks
                    try
                    {
                        if (player.vBenchmarks == null) player.vBenchmarks = new ObservableCollection<Benchmark>();

                        var benchmarks = player.benchmarks;
                        if (benchmarks != null && benchmarks.Count > 0 && player.vBenchmarks.Count <= 0)
                        {
                            player.vBenchmarks.Clear();
                            foreach (var benchmark in benchmarks)
                            {
                                if (benchmark.Value == null)
                                    continue;

                                string name = benchmark.Key;
                                StringBuilder nameSb = new StringBuilder();
                                name = name.Replace('_', ' ');
                                name = name.ToUpper();
                                benchmark.Value.Name = name;

                                benchmark.Value.raw = Math.Floor(benchmark.Value.raw * 100) / 100;
                                benchmark.Value.pct = Math.Floor(benchmark.Value.pct * 100);

                                benchmark.Value.BarWidth = 1.2/*进度条长度120*/* benchmark.Value.pct;
                                if (benchmark.Value.BarWidth < 0) benchmark.Value.BarWidth = 0;
                                if (benchmark.Value.BarWidth > 120) benchmark.Value.BarWidth = 120;

                                player.vBenchmarks.Add(benchmark.Value);
                            }
                        }
                    }
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

                    // runes
                    try
                    {
                        if (player.runes_log == null) player.runes_log = new List<Runes_Log>();
                    }
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

                    // purchase log
                    try
                    {
                        if (player.vPurchaseLog == null) player.vPurchaseLog = new ObservableCollection<Purchase_Log>();

                        var purchaseLog = player.purchase_log;
                        if (purchaseLog != null && purchaseLog.Count > 0 && player.vPurchaseLog.Count <= 0)
                        {
                            player.vPurchaseLog.Clear();
                            foreach (var purchase in purchaseLog)
                            {
                                if (purchase == null)
                                    continue;

                                int time = 0;
                                if (purchase.time != null && purchase.time > 0) time = purchase.time ?? 0;
                                purchase.PurchaseTime = (time / 60) + ":" + (time % 60).ToString("00");

                                purchase.ItemCharges = purchase.charges == null ? string.Empty : purchase.charges?.ToString();

                                string itemUrl = GetItemImgByName(purchase.key);
                                purchase.key = itemUrl;

                                player.vPurchaseLog.Add(purchase);
                            }
                            foreach (var purchase in player.vPurchaseLog)
                            {
                                try
                                {
                                    purchase?.LoadImageAsync(44);
                                }
                                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                            }
                        }
                    }
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                }
                this.CurrentMatchPlayer = player;
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }
}
