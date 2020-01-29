using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WpfMessenger.Models
{
    class User
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
    }
}
