using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Chat;
using ProjectManagementSystem.Application.Abstractions.Chat.Dto;
using ProjectManagementSystem.Application.Abstractions.Repositories.Chat;

namespace ProjectManagementSystem.Application.Chat
{
    public class ChatService : IChatService
    {
        private readonly IChatReadRepository _chatReadRepository;
        private readonly IChatWriteRepository _chatWriteRepository;
        private readonly IMapper _mapper;

        public ChatService(IChatReadRepository chatReadRepository, IChatWriteRepository chatWriteRepository, IMapper mapper)
        {
            _chatReadRepository = chatReadRepository;
            _chatWriteRepository = chatWriteRepository;
            _mapper = mapper;
        }

        public async Task<List<ChatMessageDto>> GetChatHistoryAsync(Guid user1, Guid user2)
        {
            try
            {
                var messages = await _chatReadRepository.GetQueryable().Where(x => 
                                                                        (x.SenderId == user1 && x.ReceiverId == user2) ||
                                                                        (x.SenderId == user2 && x.ReceiverId == user1))
                                                                       .OrderBy(x => x.CreatedDatee)
                                                                       .Select(m => new ChatMessageDto
                                                                       {
                                                                           Message = m.Message,
                                                                           CreatedDate = m.CreatedDatee,
                                                                           SenderId = m.SenderId,
                                                                           ReceiverId = m.ReceiverId,
                                                                           Id = m.Id
                                                                       })
                                                                       .ToListAsync();

                if (messages is null)
                    return [];

                return messages;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<bool> SaveMessageAsync(ChatMessageDto chatMessage)
        {
            if (chatMessage is null)
                return false;

            try
            {
                var mappedResult = _mapper.Map<ChatMessageDto, Domain.Entities.ChatMessage>(chatMessage);

                var response = await _chatWriteRepository.AddAsync(mappedResult);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
