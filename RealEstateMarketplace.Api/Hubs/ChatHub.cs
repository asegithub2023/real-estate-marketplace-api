using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IConversationRepository _conversationRepository;

    public ChatHub(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

   
    public static string UserGroup(int userId) => $"user-{userId}";


    public static string ConversationGroup(int conversationId) => $"conversation-{conversationId}";

    private bool TryGetCurrentUserId(out int userId)
    {
        var value = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out userId);
    }

    public override async Task OnConnectedAsync()
    {
        if (TryGetCurrentUserId(out var userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
        }

        await base.OnConnectedAsync();
    }

 
    public async Task JoinConversation(int conversationId)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return;
        }

        var conversation = await _conversationRepository.GetByIdAsync(conversationId);

        if (conversation is null || (conversation.BuyerId != userId && conversation.OwnerId != userId))
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
    }

   
    public Task LeaveConversation(int conversationId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
    }
}