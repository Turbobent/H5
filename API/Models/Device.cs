namespace API.Models
{
    public class Device : Common
    {
        public string DeviceId { get; set; }
        public bool Status { get; set; }
        public string Name { get; set; }

        public string? SharedPasswordId { get; set; }  

        // Navigation property
        [ForeignKey("SharedPasswordId")]  // Attribute goes here
        public SharedPassword? SharedPassword { get; set; }
    }

    public class SharedPassword
    {
        [Key]
        public string PasswordId { get; set; }
        public string HashedPassword { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Device> Devices { get; set; }
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
    public class UpdateSta
    {
        public bool Status { get; set; }
    }
    public class UpdatePassword
    {
        public string NewPassword { get; set; }

    }

  
}
