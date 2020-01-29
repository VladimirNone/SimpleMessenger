using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer.DbSchema
{
    public class Message
    {
        public int Id { get; set; }
        public Chat Chatroom { get; set; }
        public DateTime SendDate { get; set; }
        public string Body { get; set; }
        public User SendingUser { get; set; }
    }
}
