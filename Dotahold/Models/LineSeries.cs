using Dotahold.Data.Models;
using Windows.UI;

namespace Dotahold.Models
{
    public class LineSeries
    {
        /// <summary>
        /// The icon shown in the series tooltip
        /// </summary>
        public AsyncImage? Icon { get; set; }

        /// <summary>
        /// The title shown in the series tooltip
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The icon shown in the series tooltip when the value is negative
        /// </summary>
        public AsyncImage? NegativeIcon { get; set; }

        /// <summary>
        /// The title shown in the series tooltip when the value is negative
        /// </summary>
        public string NegativeTitle { get; set; } = string.Empty;

        /// <summary>
        /// The color of the line in the series
        /// </summary>
        public Color LineColor { get; set; }

        /// <summary>
        /// The data points in the series
        /// </summary>
        public int[] Data { get; set; } = [];
    }
}
