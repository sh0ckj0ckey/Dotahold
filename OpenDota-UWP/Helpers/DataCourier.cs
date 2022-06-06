using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Helpers
{
    /// <summary>
    /// 这是一只信使，它运送一些通用的数据到各个模块
    /// </summary>
    public class DataCourier
    {
        private static readonly Lazy<DataCourier> lazy = new Lazy<DataCourier>(() => new DataCourier());
        internal static DataCourier Instance => lazy.Value;

        public DataCourier() { }
    }
}
