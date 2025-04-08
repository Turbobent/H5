public class Log : Common
{
 public int Id { get; set; }
    public string DeviceId { get; set; }
    [ForeignKey("DeviceId")]

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
    public string DeviceId { get; set; }

    public DatePart Date { get; set; }
    public DatePart EndDate { get; set; }

    public TimePart ArmedTime { get; set; }
    public TimePart DisarmedTime { get; set; }

    public bool IsTriggered { get; set; }
    public TimePart? TriggeredTime { get; set; }
}

public class DatePart
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
}

public class TimePart
{
    public int Hour { get; set; }
    public int Minute { get; set; }
}

