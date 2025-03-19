namespace API.Models
{
    public class User_Device : Common
    {
        public Device Device { get; set; }
        public int DeviceId { get; set; }
        public User user { get; set; }
        public int UserId { get; set; }
    }

    public class PostUser_Device
    {
        public int DeviceId { get; set; }
        public int UserId { get; set; }
    }
}
