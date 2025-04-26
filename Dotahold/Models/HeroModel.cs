using Dotahold.Data.DataShop;
using Dotahold.Data.Models;

namespace Dotahold.Models
{
    internal class HeroModel
    {
        public DotaHeroModel DotaHeroAttributes { get; private set; }

        public AsyncImage HeroImage { get; private set; }

        public AsyncImage HeroIcon { get; private set; }

        public HeroModel(DotaHeroModel hero)
        {
            this.DotaHeroAttributes = hero;
            this.HeroImage = new AsyncImage($"https://cdn.cloudflare.steamstatic.com{this.DotaHeroAttributes.img}", 0, 144, ConstantsCourier.DefaultHeroImageSource72);
            this.HeroIcon = new AsyncImage($"https://cdn.cloudflare.steamstatic.com{this.DotaHeroAttributes.icon}", 0, 36, ConstantsCourier.DefaultHeroImageSource72);
        }
    }
}
