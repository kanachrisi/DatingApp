using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PaginatedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.MessageSent) // order messages by most recent first
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(m => m.Recipient.UserName == messageParams.Username && m.RecipientDeleted == false),
                "Outbox" => query.Where(m => m.Sender.UserName == messageParams.Username && m.SenderDeleted == false),
                _ => query.Where(m => m.Recipient.UserName ==
                messageParams.Username && m.RecipientDeleted == false && m.DateRead == null)
            };

            var messagesDto = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PaginatedList<MessageDto>.CreatePaginatedListAsync(messagesDto, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                .Include(msg => msg.Sender).ThenInclude(p => p.Photos)
                .Include(msg => msg.Recipient).ThenInclude(p => p.Photos)
                .Where(msg => msg.Recipient.UserName == currentUsername && msg.RecipientDeleted == false
                          && msg.Sender.UserName == recipientUsername
                          || msg.Recipient.UserName == recipientUsername
                          && msg.Sender.UserName == currentUsername && msg.SenderDeleted == false
                 ).OrderBy(msg => msg.MessageSent)
                .ToListAsync();

            var unreadMessages = messages.Where(msg => msg.DateRead == null
                  && msg.Recipient.UserName == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
