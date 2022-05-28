namespace ProjectManagement.Database;

public interface ITrackable
{
    Instant CreatedTimestamp { get; set; }
    string CreatedBy { get; set; }
    Instant ModifiedTimestamp { get; set; }
    string ModifiedBy { get; set; }
}

public static class TrackableExtensions
{
    private const string SYSTEM = "System";

    public static T SetCreateBySystem<T>(this T trackable, Instant now)
        where T : class, ITrackable
        => trackable.SetCreateBy(SYSTEM, now).SetModifyBy(SYSTEM, now);

    public static T SetModifyBySystem<T>(this T trackable, Instant now)
        where T : class, ITrackable
        => trackable.SetModifyBy(SYSTEM, now);

    public static T SetCreateBy<T>(this T trackable, string username, Instant now)
        where T : class, ITrackable
    {
        trackable.CreatedTimestamp = now;
        trackable.CreatedBy = username;
        trackable.SetModifyBy(username, trackable.CreatedTimestamp);
        return trackable;
    }

    public static T SetModifyBy<T>(this T trackable, string username, Instant now)
    where T : class, ITrackable
    {
        trackable.ModifiedTimestamp = now;
        trackable.ModifiedBy = username;

        return trackable;
    }
}
