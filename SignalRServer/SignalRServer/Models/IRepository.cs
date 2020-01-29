using SignalRServer.DbSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer.Models
{
    public interface IRepository
    {
        Task<User> SignUp(string nickname, string password);
        User VerifyDataForLogingIn(string nickname, string password);
        Task<int> NewChat(int firstParticipant, int secondParticipant);
        List<User> FindUser(string nickname);
        User FindUser(int id);
        User FindUserByConnectionId(string connectionId);
        Task AddParticipantsToChat(int ChatId, List<int> ParticipantsId);
        Task AddMessageToChat(int chatId, int sendedUserId, string message);
        Task UpdateDataOfUser(User user);
        List<User> GetParticipantsOfChat(int chatId);
        List<Message> GetMessages(int chatId, int page, int count);
        List<Chat> GetChats(int userId);
    }
}
