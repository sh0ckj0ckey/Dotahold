using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
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


    public class DotaItemModel
    {
        /// <summary>
        /// 使用的介绍，可能有多条，例如第一条是主动效果，第二条是被动
        /// </summary>
        public string[] hint { get; set; }

        /// <summary>
        /// 物品的 id
        /// int/bool
        /// </summary>
        public object id { get; set; }

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
        public object cost { get; set; }

        /// <summary>
        /// 备注，比如风杖的 notes 是你不能对友军使用
        /// </summary>
        public string notes { get; set; }

        /// <summary>
        /// 冷却时间
        /// int/bool
        /// </summary>
        public object cd { get; set; }

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
        public object charges { get; set; }

        /// <summary>
        /// 中立物品等级 
        /// int/bool
        /// </summary>
        public object tier { get; set; }

        /// <summary>
        /// mana cost 魔法消耗 
        /// int/bool
        /// </summary>
        public object mc { get; set; }

    }

    public class DotaItems
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string Price { get; set; }
        public string Info { get; set; }
        public string Tips { get; set; }
        public string Attributes { get; set; }
        public string Background { get; set; }

        public string Mana { get; set; }
        public string CoolDown { get; set; }

        public string[] Components { get; set; }

        public string Pic { get; set; }

        public DotaItems() { }

        public DotaItems(string name, string id)
        {
            this.Name = name;
            this.ID = id;
            this.Pic = String.Format("http://www.dota2.com.cn/items/images/{0}_lg.png", id);
            //http://cdn.dota2.com/apps/dota2/images/items/{0}_lg.png
        }
    }
}
