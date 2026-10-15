using UserProfiles.Api.Domain;

namespace UserProfiles.Api.Application.Interfaces
{
    public interface IEmailReminderScheduler
    {
        Task StartRemindersChain(string userSub);

        Task ScheduleJob(string userSub, ReminderDays currentStage, int daysToWait);

        bool DeleteJob(string jobId);

        void CancelReminder(string sub);
    }
}
