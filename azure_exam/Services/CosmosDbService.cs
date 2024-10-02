using azure_exam.Models;
using Microsoft.Azure.Cosmos;

namespace azure_exam.Services
{
    public class CosmosDbService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public CosmosDbService(CosmosClient cosmosClient, IConfiguration configuration)
        {
            _cosmosClient = cosmosClient;
            string databaseName = configuration["CosmosDb:DatabaseName"];
            string containerName = configuration["CosmosDb:ContainerName"];
            _container = _cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task<Chat> CreateChatAsync(string chatId)
        {
            var existingChat = await GetChatAsync(chatId);
            if (existingChat != null)
            {
                throw new InvalidOperationException("Chat with the same Id already exists.");
            }

            var chat = new Chat
            {
                id = chatId,
                chatId = chatId, 
                CreatedAt = DateTime.UtcNow
            };
            await _container.CreateItemAsync(chat, new PartitionKey(chatId));
            return chat;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns>null if there is no such chat</returns>
        public async Task<Chat?> GetChatAsync(string chatId)
        {
            try
            {
                return await _container.ReadItemAsync<Chat>(chatId, new PartitionKey(chatId));
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddMessageToChatAsync(string chatId, Message message)
        {
            var chat = await GetChatAsync(chatId);
            if (chat != null)
            {
                chat.Messages.Add(message);
                await _container.ReplaceItemAsync(chat, chat.chatId, new PartitionKey(chatId));
            }
        }


        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            var query = _container.GetItemQueryIterator<Chat>("SELECT * FROM c");
            List<Chat> results = new List<Chat>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task DeleteChatAsync(string chatId)
        {
            try
            {
                await _container.DeleteItemAsync<Chat>(chatId, new PartitionKey(chatId));
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Chat with id {chatId} not found.");
            }
        }

    }
}
