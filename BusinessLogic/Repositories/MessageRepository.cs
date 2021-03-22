using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogic.DTOs;
using BusinessLogic.Helpers;
using BusinessLogic.Interfaces;
using DAL;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups.Include(x => x.Connections).Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                                        .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                                .Include(x => x.Sender)
                                .Include(x => x.Recipient)
                                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(o => o.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(w => w.Recipient.UserName == messageParams.Username && w.RecipientDeleted == false),
                "Outbox" => query.Where(w => w.Sender.UserName == messageParams.Username && w.SenderDeleted == false),
                _ => query.Where(w => w.Recipient.UserName == messageParams.Username && w.DateRead == null && w.RecipientDeleted == false)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string RecipientUsername)
        {
            var messages = await _context.Messages.Include(x => x.Sender).ThenInclude(x => x.Photos)
                                                    .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                                                    .Where(m => m.Recipient.UserName == currentUsername
                                                                && m.RecipientDeleted == false
                                                                && m.Sender.UserName == RecipientUsername
                                                                || m.Recipient.UserName == RecipientUsername
                                                                && m.SenderDeleted == false
                                                                && m.Sender.UserName == currentUsername)
                                                    .OrderBy(m => m.MessageSent)
                                                    .ToListAsync();
            var unreadMessages = messages.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                // await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}