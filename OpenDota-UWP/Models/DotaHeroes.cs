using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
    public class DotaHeroes
    {
        public string Name { get; set; }//中文名
        public string ID { get; set; }//英文名（控制台）
        public string PicID { get; set; }
        public string Pic { get; set; }//列表头像路径
        public string LargePic { get; set; }//资料头像大图路径
        public string IconPic { get; set; }//图标头像路径
        public string Dialogue { get; set; }//代表台词

        public DotaHeroes()
        {
            this.Name = "";
            this.ID = "";
            this.PicID = "";
            this.Pic = "ms-appx:///Assets/Pictures/null.png";
            this.LargePic = "ms-appx:///Assets/Pictures/null.png";
            this.IconPic = "ms-appx:///Assets/Pictures/null.png";
            this.Dialogue = "";
        }


        //下面的两个构造函数都用于添加英雄列表的时候，第二个是给图片名字和英文名字不一致的英雄使用的
        public DotaHeroes(string name, string id, string dialogue)
        {
            this.Name = name;
            this.ID = id;
            this.PicID = id;
            //https://api.opendota.com/apps/dota2/images/heroes/{0}_{1}
            //http://cdn.dota2.com/apps/dota2/images/heroes/drow_ranger_vert.jpg?v=5029491
            //this.Pic = String.Format("http://cdn.dota2.com/apps/dota2/images/heroes/{0}_full.png", id);
            this.Pic = String.Format("ms-appx:///Assets/HeroesPhoto/{0}_full.png", id);
            //http://www.dota2.com.cn/images/heroes/{0}_full.png
            //this.LargePic = String.Format("http://cdn.dota2.com/apps/dota2/images/heroes/{0}_vert.jpg", id);
            this.LargePic = String.Format("ms-appx:///Assets/HeroesPhotoVert/{0}_vert.jpg", id);
            //http://www.dota2.com.cn/images/heroes/{0}_vert.jpg
            //this.IconPic = String.Format("https://api.opendota.com/apps/dota2/images/heroes/{0}_icon.png", id);
            this.IconPic = String.Format("ms-appx:///Assets/HeroesPhotoIcon/Miniheroes_{0}.png", id);
            this.Dialogue = dialogue;
        }

        public DotaHeroes(string name, string id, string picId, string dialogue)
        {
            this.Name = name;
            this.ID = id;
            this.PicID = picId;
            //this.Pic = String.Format("http://www.dota2.com.cn/images/heroes/{0}_full.png", picId);
            this.Pic = String.Format("ms-appx:///Assets/HeroesPhoto/{0}_full.png", picId);
            //this.LargePic = String.Format("http://www.dota2.com.cn/images/heroes/{0}_vert.jpg", picId);
            this.LargePic = String.Format("ms-appx:///Assets/HeroesPhotoVert/{0}_vert.jpg", picId);
            //this.IconPic = String.Format("https://api.opendota.com/apps/dota2/images/heroes/{0}_icon.png", picId);
            this.IconPic = String.Format("ms-appx:///Assets/HeroesPhotoIcon/Miniheroes_{0}.png", picId);
            this.Dialogue = dialogue;
        }
    }
}
