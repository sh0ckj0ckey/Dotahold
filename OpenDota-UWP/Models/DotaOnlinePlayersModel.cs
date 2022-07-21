using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
    public class DotaOnlinePlayersModel
    {
        public Response response { get; set; }
    }

    public class Response
    {
        public int player_count { get; set; }
        public int result { get; set; }
    }
}
