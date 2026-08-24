using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Api.Hubs;

/// Realtime channel for the conversation/message feature. All persistence still goes
/// through the REST endpoints (ConversationsController/MessageController) - this hub
/// only broadcasts what already happened and manages which connections receive which
/// broadcasts. It never creates or modifies data itself.
[Authorize]
public class ChatHub : Hub
{
    private readonly IConversationRepository _conversationRepository;

    public ChatHub(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    // Every authenticated user's connection(s) auto-join this group so the API can
    // push "ConversationUpdated" events (new last message, unread count) to them
    // regardless of which conversation - if any - they currently have open.
    public static string UserGroup(int userId) => $"user-{userId}";

    // One group per conversation. Only participants are added to it (see
    // JoinConversation below), so a broadcast to this group can never reach someone
    // who isn't the buyer or owner on that conversation.
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

    // Called by the client right after it opens a conversation. Re-checks
    // participation against the DB (same rule as ConversationsController/
    // MessageController) so a connection can only ever be added to the realtime
    // group for a conversation it's actually allowed to read.
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

    // Called by the client when it navigates away from a conversation (or picks a
    // different one), so this connection stops receiving that thread's broadcasts.
    public Task LeaveConversation(int conversationId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
    }
}