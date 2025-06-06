using System;
using Dotahold.Data.Models;

namespace Dotahold.Models
{
    public class PlayerOverallPerformanceModel
    {
        public string FieldName { get; private set; }

        public string FieldIcon { get; private set; }

        public double Average { get; private set; }

        public PlayerOverallPerformanceModel(DotaPlayerOverallPerformanceModel overall)
        {
            this.FieldName = overall.field;
            this.FieldIcon = overall.field switch
            {
                "kills" => "",
                "deaths" => "",
                "assists" => "",
                _ => "",
            };

            this.Average = overall.n > 0 ? Math.Floor((overall.sum / overall.n) * 10) / 10 : Math.Floor(overall.sum * 10) / 10;
        }
    }
}
