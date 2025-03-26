public class Log : Common
{
    public int DeviceId { get; set; }
    public Device Device { get; set; }
    public DateOnly Date { get; set; }
    public DateOnly EndDate { get; set; }
    public TimeOnly ArmedTime { get; set; }
    public TimeOnly DisarmedTime { get; set; }
    public bool IsTriggered { get; set; }
    public TimeOnly? TriggeredTime { get; set; }
}

public class PostLog
{
    public int DeviceId { get; set; }

    public DateObject Date { get; set; }
    public DateObject EndDate { get; set; }

    public TimeObject ArmedTime { get; set; }
    public TimeObject DisarmedTime { get; set; }

    public bool IsTriggered { get; set; }
    public TimeObject? TriggeredTime { get; set; }
}

public class DateObject
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
}

public class TimeObject
{
    public int Hour { get; set; }
    public int Minute { get; set; }
}