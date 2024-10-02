using System;
using azure_exam.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ChatCleanupFunc
{
    public class ChatCleanupFunc
    {
        private readonly CosmosDbService _cosmosDbService;

        public ChatCleanupFunc(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [Function("ChatCleanupFunction")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer) // every minute
        {
            await CleanupOldChats();
        }

        private async Task CleanupOldChats()
        {
            var chats = await _cosmosDbService.GetAllChatsAsync();
            foreach (var chat in chats)
            {
                if (chat.CreatedAt < DateTime.UtcNow.AddMinutes(-30))
                {
                    await _cosmosDbService.DeleteChatAsync(chat.chatId);
                }
            }
        }
    }
}
