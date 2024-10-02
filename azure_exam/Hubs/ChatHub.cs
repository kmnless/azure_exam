using azure_exam.Models;
using Microsoft.AspNetCore.SignalR;

namespace azure_exam.Hubs
{
    //public class ChatHub : Hub
    //{
    //    private static readonly Dictionary<string, Chat> Chats = new();

    //    public async Task CreateChat(string chatId)
    //    {
    //        if (!Chats.ContainsKey(chatId))
    //        {
    //            Chats[chatId] = new Chat { Id = chatId, Users = new List<string>(), Messages = new List<Message>() };
    //            await Clients.All.SendAsync("ChatCreated", chatId); // Уведомить всех пользователей о создании чата
    //        }
    //    }

    //    public async Task JoinChat(string chatId, string username)
    //    {
    //        if (Chats.ContainsKey(chatId))
    //        {
    //            Chats[chatId].Users.Add(username);
    //            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    //            await Clients.Group(chatId).SendAsync("UserJoined", username);
    //        }
    //    }

    //    public async Task SendMessage(string chatId, string username, string message)
    //    {
    //        var msg = new Message
    //        {
    //            Sender = username,
    //            Content = message,
    //            Timestamp = DateTime.Now
    //        };

    //        if (Chats.ContainsKey(chatId))
    //        {
    //            Chats[chatId].Messages.Add(msg);
    //            await Clients.Group(chatId).SendAsync("ReceiveMessage", msg);
    //        }
    //    }
    //}

}
