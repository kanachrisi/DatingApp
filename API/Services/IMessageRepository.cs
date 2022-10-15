using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Services
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);

        void DeleteMessage(Message message);

        Task<Message> GetMessage(int id);

        Task<PaginatedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);

        /// <summary>
        /// Get Conversation between two users
        /// </summary>
        /// <param name="currentUsername"></param>
        /// <param name="recipientUsername"></param>
        /// <returns></returns>
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);

        Task<bool> SaveAllAsync();
    }
}
