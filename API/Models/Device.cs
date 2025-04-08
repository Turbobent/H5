namespace API.Models
{
    public class Device : Common
    {
        [Key]
        public string DeviceId { get; set; }
        public bool Status { get; set; }
        public string Name { get; set; }
    }

    public class PostDevice
    {
        public string Name { get; set; }
        public string DeviceId { get; set; }
            
        public bool Status = false;
    }
    public class UpdateName
    {
        public string NewName { get; set; }
    }
    public class UpdateSta
    {
        public bool Status { get; set; }
    }
 

  
}
