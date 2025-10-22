namespace Dr_Poro.Services;

public interface IAvailabilityWorkflow
{
    public void InitializeSession(ulong userId);
    public void SetDayAvailability(ulong userId, string day, string timeRange);
    public Dictionary<string, string> GetSessionData(ulong userId);
    public bool HasSessionData(ulong userId);
    public string FormatCurrentAvailability(ulong userId);
    public void ClearSession(ulong userId);
}

public class AvailabilityWorkflow: IAvailabilityWorkflow
{
    private readonly Dictionary<ulong, Dictionary<string, string>> _sessions = new();

    public void InitializeSession(ulong userId)
    {
        if (_sessions.ContainsKey(userId))
            _sessions[userId].Clear();
        else
            _sessions[userId] = new Dictionary<string, string>();
    }

    public void SetDayAvailability(ulong userId, string day, string timeRange)
    {
        if (!_sessions.ContainsKey(userId))
            _sessions[userId] = new Dictionary<string, string>();

        _sessions[userId][day] = timeRange;
    }

    public Dictionary<string, string> GetSessionData(ulong userId)
    {
        return _sessions.TryGetValue(userId, out Dictionary<string, string>? session) ? session : new();
    }

    public bool HasSessionData(ulong userId)
    {
        return _sessions.TryGetValue(userId, out Dictionary<string, string>? session) && session.Count > 0;
    }

    public string FormatCurrentAvailability(ulong userId)
    {
        if (!_sessions.TryGetValue(userId, out Dictionary<string, string>? session))
            return "";

        return string.Join("\n",
            session
                .OrderBy(kvp => GetDayOrder(kvp.Key))
                .Select(kvp => $"**{kvp.Key}:** {kvp.Value}"));
    }

    public void ClearSession(ulong userId)
    {
        _sessions.Remove(userId);
    }

    private int GetDayOrder(string day)
    {
        return day switch
        {
            "Monday" => 1,
            "Tuesday" => 2,
            "Wednesday" => 3,
            "Thursday" => 4,
            "Friday" => 5,
            "Saturday" => 6,
            "Sunday" => 7,
            _ => 8
        };
    }
}