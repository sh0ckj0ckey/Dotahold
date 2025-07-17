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
