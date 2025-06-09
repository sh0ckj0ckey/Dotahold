using System;
using System.Linq;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;
using Dotahold.Helpers;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class AbilitiesModel
    {
        public DotaAibilitiesModel DotaAbilities { get; private set; }

        public AbilitiesFacetModel[] AbilitiesFacets { get; private set; }

        public AbilitiesModel(DotaAibilitiesModel abilities)
        {
            this.DotaAbilities = abilities;
            if (abilities.facets is not null && abilities.facets.Length > 0)
            {
                this.AbilitiesFacets = [.. abilities.facets.Select(facet => new AbilitiesFacetModel(facet))];
            }
            else
            {
                this.AbilitiesFacets = [];
            }
        }
    }

    public class AbilitiesFacetModel
    {
        /// <summary>
        /// 默认命石图标
        /// </summary>
        private static BitmapImage? _defaultFacetImageSource36 = null;

        /// <summary>
        /// 命石图标
        /// </summary>
        public AsyncImage IconImage { get; private set; }

        /// <summary>
        /// 命石名称代码
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// 命石名称
        /// </summary>
        public string Title { get; private set; } = string.Empty;

        /// <summary>
        /// 命石描述
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// 命石背景色
        /// </summary>
        public LinearGradientBrush BackgroundBrush { get; private set; }

        /// <summary>
        /// 命石索引
        /// </summary>
        public int Index { get; private set; } = -1;

        public AbilitiesFacetModel(DotaAibilitiesFacet facetData)
        {
            _defaultFacetImageSource36 ??= new BitmapImage(new Uri("ms-appx:///Assets/icon_dota2.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 36,
            };

            this.IconImage = new AsyncImage($"{ConstantsCourier.ImageSourceDomain}/apps/dota2/images/dota_react/icons/facets/{facetData.icon}.png", 0, 36, _defaultFacetImageSource36);
            this.Name = facetData.name;
            this.Title = facetData.title;
            this.Description = facetData.description;
            this.BackgroundBrush = ColorHelper.GetFacetGradientBrush($"FacetColor{ColorHelper.GetFacetColorName(int.TryParse(facetData.color, out int color) ? color : -1)}{facetData.gradient_id}");
            this.Index = facetData.id;
        }

    }
}
