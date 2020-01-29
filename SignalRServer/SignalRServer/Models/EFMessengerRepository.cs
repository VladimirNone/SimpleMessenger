using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SignalRServer.DbSchema;

namespace SignalRServer.Models
{
    public class EFMessengerRepository : IRepository
    {
        private MessengerDbContext db { get; set; }
        public EFMessengerRepository(MessengerDbContext database)
            => db = database;

        public async Task<User> SignUp(string nickname, string password)
        {
            if (db.Users.Any(h => h.NickName.Equals(nickname) || h.Password.Equals(password)))
                return new User();
            var user = new User() { NickName = nickname, Password = password };
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return user;
        }

        public User VerifyDataForLogingIn(string nickname, string password)
            => db.Users.Where(h => h.NickName.Equals(nickname) && h.Password.Equals(password)).FirstOrDefault() ?? new User();

        public async Task<int> NewChat(int firstParticipantId, int secondParicipantId)
        {
            var chat = new Chat();
            db.Chats.Add(chat);
            db.ChatParticipants.Add(new ChatParticipant() { Chatroom = chat, Participant = db.Users.Single(u => u.Id == firstParticipantId) });
            if(firstParticipantId != secondParicipantId)
                db.ChatParticipants.Add(new ChatParticipant() { Chatroom = chat, Participant = db.Users.Single(u => u.Id == secondParicipantId) });
            await db.SaveChangesAsync();
            return chat.Id;
        }

        public async Task AddParticipantsToChat(int chatId, List<int> participantsId)
        {
            if (participantsId.Count == 0) 
                return;
            var chat = db.Chats.Single(h => h.Id == chatId);
            var oldParticipantsId = db.ChatParticipants.Where(h => h.Chatroom.Id == chatId).Select(h => h.Participant.Id);
            var newParticipantsId = participantsId.Except(oldParticipantsId).ToArray();
            var participantsToAdd = new ChatParticipant[newParticipantsId.Length];
            for (int i = 0; i < newParticipantsId.Length; i++)
                participantsToAdd[i] = new ChatParticipant() { Chatroom = chat, Participant = db.Users.Single(u => u.Id == newParticipantsId[i]) };
            
            await db.ChatParticipants.AddRangeAsync(participantsToAdd);
            await db.SaveChangesAsync();
        }

        public List<User> FindUser(string nickname)
        {
            if (string.IsNullOrWhiteSpace(nickname))
                return db.Users.ToList();
            return db.Users.Where(h => h.NickName.Contains(nickname)).ToList();
        }
        public User FindUser(int id)
            => db.Users.Find(id);
        public User FindUserByConnectionId(string connectionId)
            => db.Users.SingleOrDefault(h => h.ConnectionId.Equals(connectionId));

        public async Task AddMessageToChat(int chatId, int sendedUserId, string message)
        {
            await db.Messages.AddAsync(new Message() { Chatroom = db.Chats.Single(h => h.Id == chatId), Body = message, SendingUser = db.Users.Single(u => u.Id == sendedUserId), SendDate = DateTime.Now });
            await db.SaveChangesAsync();
        }

        public List<User> GetParticipantsOfChat(int chatId)
            => db.ChatParticipants.Where(h => h.Chatroom.Id == chatId && h.Participant.ConnectionId!=null).Select(h => h.Participant).ToList();

        public async Task UpdateDataOfUser(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }

        public List<Message> GetMessages(int chatId, int page, int count)
            => db.Messages.Where(h => h.Chatroom.Id == chatId).Include(h => h.Chatroom).Include(h => h.SendingUser).AsEnumerable().OrderBy(h => h.SendDate, new DateTimeComparer()).SkipLast(count * (page - 1)).TakeLast(count).ToList();
        
        public List<Chat> GetChats(int userId)
            => db.ChatParticipants.Include(h => h.Participant).Where(h => h.Participant.Id == userId).Include(h => h.Chatroom).Select(h => h.Chatroom).ToList();
    }


    class DateTimeComparer : IComparer<DateTime>
    {
        public int Compare(DateTime x, DateTime y)
            => DateTime.Compare(x, y);
    }
}
