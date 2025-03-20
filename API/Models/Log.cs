namespace API.Models
{
    public class Log : Common
    {
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public DateTime Date { get; set; } 
        public TimeSpan ArmedTime { get; set; } 
        public TimeSpan DisarmedTime { get; set; } 
        public bool IsTriggered { get; set; }
        public TimeSpan? TriggeredTime { get; set; } 
    }

    public class PostLog
    {
        public int DeviceId { get; set; }
        public DateTime Date { get; set; } 
        public TimeSpan ArmedTime { get; set; } 
        public TimeSpan DisarmedTime { get; set; } 
        public bool IsTriggered { get; set; }
        public TimeSpan? TriggeredTime { get; set; } 
    }

}
