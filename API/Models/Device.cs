namespace API.Models
{
    public class Device : Common
    {
        public bool Status { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public TimeOnly ArmedTime { get; set; }
        public TimeOnly DisarmedTime { get; set; }
    }
}
