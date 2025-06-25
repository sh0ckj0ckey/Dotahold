using Dotahold.Data.Models;
using Windows.UI;

namespace Dotahold.Models
{
    public class LineSeries
    {
        public AsyncImage? Icon { get; set; }

        public string Title { get; set; } = string.Empty;

        public Color LineColor { get; set; }

        public int[] Data { get; set; } = [];
    }
}
