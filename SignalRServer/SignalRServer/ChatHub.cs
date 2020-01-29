
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;
using SignalRServer.DbSchema;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SignalRServer
{
    public class ChatHub : Hub
    {
        private IRepository repo { get; set; }
        public ChatHub(IRepository mes)
            => repo = mes;

        public async Task Send(Message message)
        {
            await repo.AddMessageToChat(message.Chatroom.Id,message.SendingUser.Id, message.Body);
            var temp = repo.GetParticipantsOfChat(message.Chatroom.Id).Select(h => h.ConnectionId).ToList();
            await this.Clients.Clients(temp).SendAsync("ReceiveMessage",  message);
        }

        public async Task SignUp(string nickname, string password)
        {
            var user = await repo.SignUp(nickname, password);
            if (user.Id != 0)
            {
                user.ConnectionId = this.Context.ConnectionId;
                await repo.UpdateDataOfUser(user);
            }
            await AddUsersToChat(1, new List<int>() { user.Id }); //Add user to global chat
            await this.Clients.Caller.SendAsync("ReceiveResultOfSignUp", user.Id);
        }

        public async Task LogIn(string nickname, string password)
        {
            var user = repo.VerifyDataForLogingIn(nickname, password);
            if(user.Id != 0)
            {
                user.ConnectionId = this.Context.ConnectionId;
                await repo.UpdateDataOfUser(user);
            }
            await this.Clients.Caller.SendAsync("ReceiveResultOfLogIn", user.Id);
        }

        public async Task GetMessageFromChat(string fromUserId, string chatId, int page)
            => await this.Clients.Caller.SendAsync("ReceiveMessagesFromChat", repo.GetMessages(int.Parse(chatId), page, 10));

        public async Task GetChats(string fromUserId)
            => await this.Clients.Caller.SendAsync("ReceiveChats", repo.GetChats(int.Parse(fromUserId)));

        public async Task FindUserByNickName(string nickName)
            => await this.Clients.Caller.SendAsync("ReceiveSearchedUsers", repo.FindUser(nickName));

        public async Task AddUsersToChat(int chatId, List<int> userIds)
            => await repo.AddParticipantsToChat(chatId, userIds);

        public async Task CreateNewChat(int creatorId, int secondId)
        {
            var chatId = await repo.NewChat(creatorId, secondId);
            var userConnection = repo.FindUser(secondId).ConnectionId;
            await this.Clients.Caller.SendAsync("ReceiveUpdateInListOfChats", chatId);
            if(!string.IsNullOrEmpty(userConnection))
                await this.Clients.Client(userConnection).SendAsync("ReceiveUpdateInListOfChats", chatId);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var user = repo.FindUserByConnectionId(this.Context.ConnectionId);
            if (user == null)
                return base.OnDisconnectedAsync(exception);
            user.ConnectionId = null;
            repo.UpdateDataOfUser(user);
            return base.OnDisconnectedAsync(exception);
        }
    }
}