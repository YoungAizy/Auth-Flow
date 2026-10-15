using UserProfiles.Api.Domain;

namespace UserProfiles.Api.Application
{
    public static class ReminderSequence
    {
        public static IEnumerable<ReminderDays> GetStages()
        {
            foreach (ReminderDays stage in Enum.GetValues(typeof(ReminderDays)))
            {
                yield return stage;
            }
        }

        // Finds the next stage after the current one
        public static ReminderDays? GetNext(ReminderDays current)
        {
            return GetStages().Cast<ReminderDays?>()
                .SkipWhile(s => s != current)
                .Skip(1)
                .FirstOrDefault();
        }
    }
}
