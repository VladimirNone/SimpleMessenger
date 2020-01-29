using System.Text.Json.Serialization;

namespace SignalRServer.DbSchema
{
    
    public class User
    {

        public int Id { get; set; }

        public string NickName { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public string Status { get; set; }
        [JsonIgnore]
        public string ConnectionId { get; set; }
    }
}
