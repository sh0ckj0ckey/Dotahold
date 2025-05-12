using System.Collections.Generic;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;

namespace Dotahold.Models
{
    public class HeroDataModel
    {
        public DotaHeroDataModel DotaHeroData { get; private set; }

        /// <summary>
        /// 命石列表
        /// </summary>
        public List<HeroFacetModel> Facets { get; private set; } = new List<HeroFacetModel>();

        public HeroDataModel(DotaHeroDataModel heroData)
        {
            this.DotaHeroData = heroData;

            if (this.DotaHeroData.facets is not null)
            {
                foreach (var facet in this.DotaHeroData.facets)
                {
                    this.Facets.Add(new HeroFacetModel(facet));
                }
            }
        }
    }

    public class HeroFacetModel
    {
        public AsyncImage IconImage { get; private set; }

        public HeroFacetModel(FacetData facetData)
        {
            this.IconImage = new AsyncImage($"{ConstantsCourier.ImageSourceDomain}/apps/dota2/images/dota_react/icons/facets/{facetData.icon}.png", 0, 72);
        }
    }
}
