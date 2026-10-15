using UserProfiles.Api.Domain;

namespace UserProfiles.Api.Application.Interfaces
{
    public interface IEmailReminderJob
    {
        Task ProcessJob(string userSub, ReminderDays stage);
        Task SendReminderAsync(string userSub, ReminderDays currentStage);
        Task DeleteUnverifiedUser(string userSub);
    }
}
