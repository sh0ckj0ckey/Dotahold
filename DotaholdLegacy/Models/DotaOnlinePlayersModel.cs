namespace Dotahold.Models
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
