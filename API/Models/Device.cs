namespace API.Models
{
    public class Device : Common
    {
        public bool Status { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }
    }
    public class PostDevice
    {
        public string Name { get; set; }
        public int DeviceId { get; set; }

        public bool Status = false;
    }
    class UpdateName
    {
        public string Name { get; set; }
    }
    class UpadteStatus
    {
        public int DeviceId { get; set; }
        public bool Status { get; set; }
    }
}
