using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfMessenger.Models;
using System.Text.Json;
using System.Threading;

namespace WpfMessenger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Message> messages { get; set; } = new List<Message>();
        int curChat;
        int page = 1;
        bool findMode;
        bool findUserForNewChat = true;
        List<int> selectedIdUsersForAddToChat;
        object[] bufferCollection { get; set; }
        Client client = new Client();
        public MainWindow()
        {
            InitializeComponent();
            client.Connect();
            InitializeConnectionListeners();
            gContentOfChats.Visibility = Visibility.Hidden;
            gChats.Visibility = Visibility.Hidden;
            gEnter.Visibility = Visibility.Visible;
            tbLoginNickName.Text = "Enter nickname";

            tbLoginNickName.GotFocus += RemoveText;
            tbLoginNickName.LostFocus += AddText;

            tbLoginPassword.Text = "Enter password";

            tbLoginPassword.GotFocus += RemoveText;
            tbLoginPassword.LostFocus += AddText;


            MainGrid.SizeChanged += (sender, e) =>
            {
                MainGrid.Width = 804;
                MainGrid.Height = 594;
            };
        }

        private void RemoveText(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "Enter password" || ((TextBox)sender).Text == "Enter nickname")
                ((TextBox)sender).Text = "";
            ((TextBox)sender).Background = Brushes.White;
        }

        private void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbLoginPassword.Text))
                tbLoginPassword.Text = "Enter password";
            if (string.IsNullOrWhiteSpace(tbLoginNickName.Text))
                tbLoginNickName.Text = "Enter nickname";
        }

        private ListBoxItem CreateItemOfListChats(string nameOfChat, string countUnreadMessage, int tag)
        {
            var dock = new DockPanel() { LastChildFill = true, Width = 190, Height = 40 };
            dock.Children.Add(new TextBlock() { Text = nameOfChat});
            dock.Children.Add(new TextBlock() { Text = countUnreadMessage.Equals("0") ? null : countUnreadMessage });
            dock.Children.Add(new TextBlock());
            DockPanel.SetDock(dock.Children[0], Dock.Left);
            DockPanel.SetDock(dock.Children[1], Dock.Right);
            var button = new Button() { Content = dock, Margin = new Thickness(0)};
            button.Click += SelectChat_Click;
            button.Tag = tag;
            return new ListBoxItem() { Content = button, Padding = new Thickness(0) };   
        }

        private ListBoxItem CreateItemOfListUsers(string nickname, int tag)
        {
            var dock = new DockPanel() { LastChildFill = true, Width = 190, Height = 40 };
            dock.Children.Add(new TextBlock() { Text = nickname });
            dock.Children.Add(new TextBlock());
            DockPanel.SetDock(dock.Children[0], Dock.Left);
            var button = new Button() { Content = dock, Margin = new Thickness(0) };
            button.Click += SelectUser_Click;
            button.Tag = tag;
            return new ListBoxItem() { Content = button, Padding = new Thickness(0) };
        }

        private void InitializeConnectionListeners()
        {
            client.Connection.On<int>("ReceiveResultOfSignUp", (id) =>
            {
                if (id == 0)
                {
                    tbLoginPassword.Text = "Enter password";
                    tbLoginNickName.Text = "Enter nickname";
                }
                else
                {
                    gContentOfChats.Visibility = Visibility.Visible;
                    gChats.Visibility = Visibility.Visible;
                    gEnter.Visibility = Visibility.Hidden;
                    client.User.Id = id;
                    client.GetChats();
                }
            });

            client.Connection.On<int>("ReceiveResultOfLogIn", (id) =>
            {
                if (id == 0)
                {
                    tbLoginPassword.Text = "Enter password";
                    tbLoginNickName.Text = "Enter nickname";
                }
                else
                {
                    gContentOfChats.Visibility = Visibility.Visible;
                    gChats.Visibility = Visibility.Visible;
                    gEnter.Visibility = Visibility.Hidden;
                    client.User.Id = id;
                    client.GetChats();
                }
            });

            client.Connection.On<Message>("ReceiveMessage", (message) =>
            {
                if (message.Chatroom.Id == curChat)
                {
                    if (message.SendingUser.NickName == client.User.NickName)
                    {
                        message.TextAlignment = TextAlignment.Right;
                        message.Column = 2;
                        message.ShortInfo = message.Body + "\n" + message.SendDate.ToString();
                    }
                    else
                        message.ShortInfo = message.ToString();
                    messages.Add(message);
                    lbChatHistory.ItemsSource = null;
                    lbChatHistory.ItemsSource = messages;
                    lbChatHistory.UpdateLayout();
                }
            });

            client.Connection.On<List<Message>>("ReceiveMessagesFromChat", (messagesFromChat) =>
            {
                foreach (var item in messagesFromChat)
                    if (item.SendingUser.NickName == client.User.NickName)
                    {
                        item.TextAlignment = TextAlignment.Right;
                        item.Column = 2;
                        item.ShortInfo = item.Body + "\n" + item.SendDate.ToString();
                    }
                    else
                        item.ShortInfo = item.ToString();
                messages.InsertRange(0, messagesFromChat);
                lbChatHistory.ItemsSource = null;
                lbChatHistory.ItemsSource = messages;
                lbChatHistory.UpdateLayout();
            });

            client.Connection.On<List<Chat>>("ReceiveChats", (chats) =>
            {
                foreach (var chat in chats)
                    lbListOfChats.Items.Add(CreateItemOfListChats(chat.Id + "", "0", chat.Id));
            });

            client.Connection.On<List<User>>("ReceiveSearchedUsers", (users) =>
            {
                lbListOfChats.Items.Clear();
                foreach (var user in users)
                    lbListOfChats.Items.Add(CreateItemOfListUsers(user.NickName, user.Id));
            });

            client.Connection.On<int>("ReceiveUpdateInListOfChats", (chatId) =>
            {
                var item = CreateItemOfListChats(chatId + "", "0", chatId);
                var button = (Button)item.Content;
                if (findMode)
                {
                    var v = bufferCollection?.ToList();
                    v?.Add(item);
                    bufferCollection = v?.ToArray();
                }
                else
                {
                    lbListOfChats.Items.Add(item);
                }
                SelectChat_Click(button, null);

            });
        }



        private void SelectUser_Click(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            if (findUserForNewChat)
            {
                client.CreateChat(client.User.Id, (int)b.Tag);
                BackToListOfChats_Click(null,null);
            }
            else
            {
                b.Background = Brushes.Aqua;
                selectedIdUsersForAddToChat.Add((int)b.Tag);
            }
        }

        private void SelectChat_Click(object sender, RoutedEventArgs e)
        {
            var l = (Button)sender;
            page = 1;
            messages = new List<Message>();
            curChat = (int)l.Tag;
            client.GetMessages(curChat,page);
            //var s = (TextBlock)((DockPanel)l.Content).Children[0];
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            bool b = false;
            if (tbLoginNickName.Text.Equals("Enter nickname"))
            {
                tbLoginNickName.Background = Brushes.Red;
                b = true;
            }
            if (tbLoginPassword.Text.Equals("Enter password"))
            {
                tbLoginPassword.Background = Brushes.Red;
                b = true;
            }
            if (b) return;
            client.User.NickName = tbLoginNickName.Text;
            client.User.Password = tbLoginPassword.Text;
            client.LogIn();
        }

        private void Signup_Click(object sender, RoutedEventArgs e)
        {
            bool b = false;
            if (tbLoginNickName.Text.Equals("Enter nickname"))
            {
                tbLoginNickName.Background = Brushes.Red;
                b = true;
            }
            if (tbLoginPassword.Text.Equals("Enter password"))
            {
                tbLoginPassword.Background = Brushes.Red;
                b = true;
            }
            if (b) return;
            client.User.NickName = tbLoginNickName.Text;
            client.User.Password = tbLoginPassword.Text;
            client.SignUp();
        }

        private void MoreMessage_Click(object sender, RoutedEventArgs e)
        {
            page++;
            client.GetMessages(curChat, page);
        }

        private void FindUser_Click(object sender, RoutedEventArgs e)
        {
            if (findUserForNewChat)
                bBack.Visibility = Visibility.Visible;
            if (!findMode)
            {
                bufferCollection = new object[lbListOfChats.Items.Count];
                lbListOfChats.Items.CopyTo(bufferCollection, 0);
            }
            findMode = true;
            lbListOfChats.Items.Clear();
            client.FindUsers(tbFieldForFindUser.Text);
        }

        private void BackToListOfChats_Click(object sender, RoutedEventArgs e)
        {
            if (findUserForNewChat)
                bBack.Visibility = Visibility.Hidden;
            lbListOfChats.Items.Clear();
            foreach (var item in bufferCollection)
                lbListOfChats.Items.Add(item);
            bufferCollection = null;
            findMode = false;
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            client.Send(new Message() { Body = tbFieldForMessage.Text, Chatroom = new Chat() { Id = curChat }, SendDate = DateTime.Now, SendingUser = client.User });
            tbFieldForMessage.Text = "";
        }

        private void Signout_Click(object sender, RoutedEventArgs e)
        {
            client.Disconnect();
            client.Connect();
            InitializeConnectionListeners();
            client.ClearInfo();
            messages = new List<Message>();
            curChat = 0;
            page = 1;
            findMode = false;
            findUserForNewChat = true;
            selectedIdUsersForAddToChat = null;
            bufferCollection = null;
            lbListOfChats.Items.Clear();
            lbChatHistory.ItemsSource = null;
            lbChatHistory.UpdateLayout();
            gContentOfChats.Visibility = Visibility.Hidden;
            gChats.Visibility = Visibility.Hidden;
            gEnter.Visibility = Visibility.Visible;
        }
        
        private async void AddParticipantsToChat_Click(object sender, RoutedEventArgs e)
        {
            if (curChat == 0) return;
            if(bAddParticipantsToChat.Content.Equals("Пригласить участника в чат"))
            {
                findUserForNewChat = false;
                bAddParticipantsToChat.Content = "Пригласить";
                bAddParticipantsToChat.Background = Brushes.AntiqueWhite;
                FindUser_Click(null, null);
                selectedIdUsersForAddToChat = new List<int>();
            }
            else
            {
                findUserForNewChat = true;
                bAddParticipantsToChat.Content = "Пригласить участника в чат";
                bAddParticipantsToChat.Background = Brushes.White;
                await client.AddUsersToChat(curChat, selectedIdUsersForAddToChat);
                findMode = false;
                lbListOfChats.Items.Clear();
                foreach (var item in bufferCollection)
                    lbListOfChats.Items.Add(item);
            }
        }
    }
}
