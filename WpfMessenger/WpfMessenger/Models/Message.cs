using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace WpfMessenger.Models
{
    class Message
    {
        public int Id { get; set; }
        public Chat Chatroom { get; set; }
        public DateTime SendDate { get; set; }
        public string Body { get; set; }
        public User SendingUser { get; set; }

        [JsonIgnore]
        public string ShortInfo { get; set; }
        [JsonIgnore]
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;
        [JsonIgnore]
        public int Column { get; set; } = 0;

        public override string ToString()
            => string.Format($"{SendingUser.NickName}: {Body}\n{SendDate.ToString()}");
        
    }
}
