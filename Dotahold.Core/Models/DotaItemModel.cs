using Dotahold.Core.DataShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Core.Models
{
    public class DotaItemModel : ViewModelBase
    {

        /// <summary>
        /// 使用的介绍，可能有多条，例如第一条是主动效果，第二条是被动
        /// </summary>
        public string[] hint { get; set; }

        /// <summary>
        /// 物品的 id
        /// int/bool
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// https://steamcdn-a.akamaihd.net/ + apps/dota2/images/items/{img}.png
        /// </summary>
        public string img { get; set; }

        /// <summary>
        /// 显示的名字
        /// </summary>
        public string dname { get; set; }

        /// <summary>
        /// 物品分类，“神秘”“圣物”等等
        /// </summary>
        public string qual { get; set; }

        /// <summary>
        /// 价格
        /// int/bool
        /// </summary>
        public string cost { get; set; }

        /// <summary>
        /// 备注，比如风杖的 notes 是你不能对友军使用
        /// </summary>
        public string notes { get; set; }

        /// <summary>
        /// 冷却时间
        /// int/bool
        /// </summary>
        public string cd { get; set; }

        /// <summary>
        /// 背景介绍，就是物品介绍界面最底部那几句介绍的话
        /// </summary>
        public string lore { get; set; }

        /// <summary>
        /// 合成配方
        /// </summary>
        public string[] components { get; set; }

        /// <summary>
        /// 似乎是表示物品是否需要合成
        /// </summary>
        public bool created { get; set; }

        /// <summary>
        /// 属性加成
        /// </summary>
        public Attrib[] attrib { get; set; }

        /// <summary>
        /// 似乎没什么用，只有二级散失有这个值，对应的是其他物品的 hint
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// 消耗品，例如芒果的 charges = 1，吃树的 charges = 3 
        /// int/bool
        /// </summary>
        public string charges { get; set; }

        /// <summary>
        /// 中立物品等级 
        /// int/bool
        /// </summary>
        public string tier { get; set; }

        /// <summary>
        /// mana cost 魔法消耗 
        /// int/bool
        /// </summary>
        public string mc { get; set; }

        // 装备图片
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ImageSource = ConstantsCourier.DefaultItemImageSource72;
        [Newtonsoft.Json.JsonIgnore]
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            private set { Set("ImageSource", ref _ImageSource, value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        private bool _loadedImage = false;
        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                if (_loadedImage || string.IsNullOrWhiteSpace(this.img)) return;
                var imageSource = await ImageCourier.GetImageAsync(this.img);
                if (imageSource != null)
                {
                    this.ImageSource = imageSource;
                    this.ImageSource.DecodePixelType = DecodePixelType.Logical;
                    this.ImageSource.DecodePixelWidth = decodeWidth;
                    _loadedImage = true;
                }
            }
            catch { }
        }
    }

    public class Attrib
    {
        public string key { get; set; }
        public string header { get; set; }
        public string footer { get; set; }
        public bool generated { get; set; }

        /// <summary>
        /// 例如大根，根据不同等级增加不同的属性，这个时候 value 为数组 string[]，否则为 string
        /// </summary>
        public object value { get; set; }
    }
}
