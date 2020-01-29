using Microsoft.EntityFrameworkCore;
using SignalRServer.DbSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer.Models
{
    public class MessengerDbContext : DbContext
    {
        public MessengerDbContext(DbContextOptions options)
            : base(options) 
        {
            try
            {
                Database.EnsureCreated();
            }
            catch(Exception ex)
            {

            } 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatParticipant> ChatParticipants { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageReadState> MessageReadStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(h => h.Property(j => j.Status).HasDefaultValue("user"));
        }

        public void SeedData()
        {
            var users = new[] 
            { 
                new User() { NickName = "NotZero_Bot", Password = "0000" }, 
                new User() { NickName = "ChinPin_Bot", Password = "0001" }, 
                new User() { NickName = "Viking_Bot", Password = "0002" }, 
                new User() { NickName = "Patriot_Bot", Password = "0003" }
            };
            var chats = new[]
            {
                new Chat()
            };
            var chatParticipants = new[]
            {
                new ChatParticipant() { Chatroom = chats[0], Participant = users[0]},
                new ChatParticipant() { Chatroom = chats[0], Participant = users[1]},
                new ChatParticipant() { Chatroom = chats[0], Participant = users[2]},
                new ChatParticipant() { Chatroom = chats[0], Participant = users[3]}
            };
            var messages = new[]
            {
                new Message(){ Chatroom = chats[0], Body = "1 message", SendDate = DateTime.Now.AddMinutes(-10), SendingUser = users[0]},
                new Message(){ Chatroom = chats[0], Body = "2 message", SendDate = DateTime.Now.AddMinutes(-9), SendingUser = users[1]},
                new Message(){ Chatroom = chats[0], Body = "3 message", SendDate = DateTime.Now.AddMinutes(-8), SendingUser = users[2]},
                new Message(){ Chatroom = chats[0], Body = "4 message", SendDate = DateTime.Now.AddMinutes(-7), SendingUser = users[2]},
                new Message(){ Chatroom = chats[0], Body = "5 message", SendDate = DateTime.Now.AddMinutes(-6), SendingUser = users[3]},
                new Message(){ Chatroom = chats[0], Body = "6 message", SendDate = DateTime.Now.AddMinutes(-5), SendingUser = users[1]},
                new Message(){ Chatroom = chats[0], Body = "7 message", SendDate = DateTime.Now.AddMinutes(-4), SendingUser = users[0]},
                new Message(){ Chatroom = chats[0], Body = "8 message", SendDate = DateTime.Now.AddMinutes(-3), SendingUser = users[0]},
                new Message(){ Chatroom = chats[0], Body = "9 message", SendDate = DateTime.Now.AddMinutes(-2), SendingUser = users[1]},
                new Message(){ Chatroom = chats[0], Body = "10 message", SendDate = DateTime.Now.AddMinutes(-1), SendingUser = users[2]},
                new Message(){ Chatroom = chats[0], Body = "11 message", SendDate = DateTime.Now.AddMinutes(-0), SendingUser = users[0]},
                new Message(){ Chatroom = chats[0], Body = "11 message", SendDate = DateTime.Now.AddMinutes(-0), SendingUser = users[3]},
            };
            Users.AddRange(users);
            Chats.AddRange(chats);
            ChatParticipants.AddRange(chatParticipants);
            foreach (var item in messages)
            {
                Messages.Add(item); 
                this.SaveChanges();
            }
        }
    }
}
