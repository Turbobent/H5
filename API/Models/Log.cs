namespace API.Models
{
    public class Log : Common
    {
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly ArmedTime { get; set; }
        public TimeOnly DisarmedTime { get; set; }
        public bool IsTriggered { get; set; }
        public TimeOnly? TriggeredTime { get; set; }
    }
}
