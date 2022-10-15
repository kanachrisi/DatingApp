using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IMemberRepository memberRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _memberRepository = memberRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> PostMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();
            if(username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send messages to yourself");
            }
            var sender = await _memberRepository.GetMemberByUsernameAsync(username);

            var recipient = await _memberRepository.GetMemberByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllAsync())
            {
                return Ok(_mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to post the message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messagesDto = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messagesDto.CurrentPage, messagesDto.PageSize, messagesDto.TotalCount, messagesDto.TotalPages);

            return messagesDto;

        }

        /// <summary>
        /// Get the conversation bwtn
        /// current user and other user
        /// </summary>
        /// <param name="username">other username</param>
        /// <returns></returns>
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var currentUsername = User.GetUsername();

            var message = await _messageRepository.GetMessage(id);

            if(message.Sender.UserName != currentUsername && message.Recipient.UserName != currentUsername)
            {
                return Unauthorized();
            }

            if(message.Sender.UserName == currentUsername)
            {
                message.SenderDeleted = true;
            }

            if(message.Recipient.UserName == currentUsername)
            {
                message.RecipientDeleted = true;
            }

            if(message.SenderDeleted && message.RecipientDeleted)
            {
                _messageRepository.DeleteMessage(message);
            }

            if(await _messageRepository.SaveAllAsync())
            {
                return Ok();
            }
            return BadRequest("Problem deleting the message");
        }
    }
}
