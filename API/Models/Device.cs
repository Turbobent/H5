namespace API.Models
{
    public class Device : Common
    {
        [Key]
        public string DeviceId { get; set; }
        public bool Status { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }

    public class DeviceMakePassword
    {
        public string DeviceId { get; set; }
        public string Password { get; set; }
    }
    public class DeviceLogin
    {
        public string DeviceId { get; set; }
        public string Password { get; set; }
    }
    public class PostDevice
    {
        public string Name { get; set; }
        public string DeviceId { get; set; }

        public bool Status = false;
        public string Password { get; set; }

    }
    public class UpdateName
    {
        public string NewName { get; set; }
    }
    public class UpadteStatus
    {
        public string DeviceId { get; set; }
        public bool Status { get; set; }
    }
}
