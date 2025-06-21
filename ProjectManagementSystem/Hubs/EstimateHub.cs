using Microsoft.AspNetCore.SignalR;

namespace ProjectManagementSystem.Hubs
{
    public class EstimateHub : Hub
    {
        public async Task JoinEstimateGroup(Guid estimateId)
            => await Groups.AddToGroupAsync(Context.ConnectionId, $"estimate-{estimateId}");

        public async Task LeaveEstimateGroup(Guid estimateId)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"estimate-{estimateId}");

        public async Task NotifyTaskUpdated(string estimateId, Guid taskId)
        {
            await Clients.Group($"estimate-{estimateId}").SendAsync("ReceiveTaskUpdate", taskId);
        }

        public async Task SendVote(Guid estimateId, Guid taskId, string user, string value)
        {
            // Not: Bu örnekte her task ayrı ayrı oy seti tutuyor olabilir
            await Clients.Group($"estimate-{estimateId}")
                         .SendAsync("ReceiveVote", new
                         {
                             TaskId = taskId,
                             User = user,
                             Value = value
                         });
        }

        public async Task RevealVotes(Guid estimateId, Guid taskId)
        {
            // Not: Sunucu tarafı memory store'dan oyları getirebilir
            // Burada doğrudan client'ten alınan votes array varsayalım
            await Clients.Group($"estimate-{estimateId}")
                         .SendAsync("VotesRevealed", new
                         {
                             TaskId = taskId
                         });
        }

        public async Task NotifySuccessEstimate(Guid estimateId)
        {
            await Clients.Group($"estimate-{estimateId}")
                         .SendAsync("NotifySuccess");
        }

        public async Task NotifyFailEstimate(Guid estimateId)
        {
            await Clients.Group($"estimate-{estimateId}")
                         .SendAsync("NotifyError");
        }
    }
}
