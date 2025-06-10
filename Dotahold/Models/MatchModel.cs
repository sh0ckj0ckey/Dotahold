using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotahold.Data.Models;

namespace Dotahold.Models
{
    public class MatchModel
    {
        public DotaMatchModel DotaMatch { get; private set; }

        public HeroModel Hero { get; private set; }

        public AbilitiesFacetModel? AbilitiesFacet { get; private set; }

        public MatchModel(DotaMatchModel dotaMatch, HeroModel hero, AbilitiesFacetModel? abilitiesFacet)
        {
            this.DotaMatch = dotaMatch;
            this.Hero = hero;
            this.AbilitiesFacet = abilitiesFacet;
        }
    }
}
