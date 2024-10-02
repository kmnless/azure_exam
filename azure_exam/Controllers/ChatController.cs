using azure_exam.Models;
using azure_exam.Services;
using Microsoft.AspNetCore.Mvc;

namespace azure_exam.Controllers
{
    public class ChatController : Controller
    {
        private readonly CosmosDbService _cosmosDbService;

        public ChatController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var chats = await _cosmosDbService.GetAllChatsAsync();
            return View(chats);
        }

        [HttpGet]
        public IActionResult CreateChat()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat(string chatId)
        {
            if (string.IsNullOrEmpty(chatId))
            {
                ViewBag.Message = "Chat ID is empty!";
                return View();
            }

            var existingChat = await _cosmosDbService.GetChatAsync(chatId);
            if (existingChat != null)
            {
                ViewBag.Message = "Chat with current id already exists!";
                return View();
            }

            await _cosmosDbService.CreateChatAsync(chatId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Chat(string chatId)
        {
            var chat = await _cosmosDbService.GetChatAsync(chatId);
            if (chat == null)
            {
                return NotFound();
            }

            string? sender = HttpContext.Session.GetString("Sender");
            if (sender != null)
            {
                ViewBag.Sender = sender;
            }

            return View(chat);
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage(string chatId, string messageContent, string sender)
        {
            if (string.IsNullOrEmpty(messageContent))
            {
                return BadRequest("Message is empty");
            }
            HttpContext.Session.SetString("Sender", sender);
            HttpContext.Session.SetString("SenderTimestamp", DateTime.UtcNow.ToString());
            var message = new Message
            {
                Sender = sender,
                Text = messageContent,
                Timestamp = DateTime.UtcNow
            };

            await _cosmosDbService.AddMessageToChatAsync(chatId, message);
            return RedirectToAction("Chat", new { chatId });
        }
    }

}
