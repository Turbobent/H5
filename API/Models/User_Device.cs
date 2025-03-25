namespace API.Models
{
    public class User_Device : Common
    {
        public string DeviceId { get; set; }
        [ForeignKey("DeviceId")]
        public Device Device { get; set; }

        [ForeignKey("UserId")]
        public User user { get; set; }
        public int UserId { get; set; }
    }

    public class PostUser_Device
    {
        public string DeviceId { get; set; }
        public int UserId { get; set; }
    }
}
