using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer.DbSchema
{
    public class MessageReadState
    {
        public int Id { get; set; }
        public Message WritenMessage { get; set; }
        public User Participant { get; set; }
        public DateTime ReadDate { get; set; }
    }
}
