using Dotahold.Models;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dotahold.Controls
{
    public sealed partial class HeroFacetCard : UserControl
    {
        private readonly HeroFacetModel _heroFacetModel;

        public HeroFacetCard(HeroFacetModel heroFacetModel)
        {
            _heroFacetModel = heroFacetModel;

            this.InitializeComponent();
        }
    }
}
