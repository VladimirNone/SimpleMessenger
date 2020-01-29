using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using WpfMessenger.Models;

namespace WpfMessenger
{
    class Client
    {
        public HubConnection Connection { get; set; }
        public static string UrlOfServer { get; set; } = @"https://localhost:00000/chat";
        public User User { get; set; } = new User();

        public Client(string nickname, string password, HubConnection hubConnection)
        {
            User.NickName = nickname;
            User.Password = password;
            Connection = hubConnection;
        }
        public Client()
        {
        }
        public async void Connect()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(UrlOfServer)
                .Build();
            await Connection.StartAsync(); 
        }

        public async void Disconnect()
            => await Connection.StopAsync();
        
        public async void GetMessages(int chatId, int page)
            => await Connection.InvokeAsync("GetMessageFromChat", User.Id+"", chatId+"", page);
        
        public async void LogIn()
            => await Connection.InvokeAsync("LogIn", User.NickName, User.Password);

        public async void SignUp()
            => await Connection.InvokeAsync("SignUp", User.NickName, User.Password);

        public async void GetChats()
            => await Connection.InvokeAsync("GetChats", User.Id+"");
        
        public async void Send(Message message)
            => await Connection.InvokeAsync("Send",message);

        public async void FindUsers(string nickName)
            => await Connection.InvokeAsync("FindUserByNickName", nickName);

        public async Task AddUsersToChat(int chatId, List<int> userIds)
            => await Connection.InvokeAsync("AddUsersToChat", chatId, userIds);

        public async void CreateChat(int id, int tag)
            => await Connection.InvokeAsync("CreateNewChat", id, tag);

        public void ClearInfo()
            => User = new User();
        
    }
}
