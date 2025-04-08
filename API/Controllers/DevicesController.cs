using API.Models;
using System.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public DevicesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            return await _context.Devices.ToListAsync();
        }

        // GET: api/Devices/ABC123
        [HttpGet("{deviceId}")]
        public async Task<ActionResult<Device>> GetDevice(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return BadRequest("Device ID is required");
            }

            var device = await _context.Devices.FindAsync(deviceId);

            if (device == null)
            {
                return NotFound($"Device with ID {deviceId} not found");
            }

            return Ok(device);
        }

        [HttpPut("UpdateStatus/{deviceId}")]
        public async Task<IActionResult> PutDeviceName(string deviceId, UpdateSta device)
        {
            // Find the device in the database
            var deviceEntity = await _context.Devices.FindAsync(deviceId);
            if (deviceEntity == null)
            {
                return NotFound(new { message = "Device not found" });
            }

            deviceEntity.Status = device.Status;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Status updated successfully" });
        }

        [HttpPut("UpdateName/{deviceId}")]
        public async Task<IActionResult> PutDeviceName(string deviceId, UpdateName device)
        {
            // Find the device in the database
            var deviceEntity = await _context.Devices.FindAsync(deviceId);
            if (deviceEntity == null)
            {
                return NotFound(new { message = "Device not found" });
            }

            // Update the name
            deviceEntity.Name = device.NewName;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Name updated successfully" });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Device>> PostDevice(PostDevice postDevice)
        {
            //  Validate input
            var errors = new Dictionary<string, string>();

        

            if (string.IsNullOrWhiteSpace(postDevice.DeviceId))
            {
                errors["DeviceId"] = "Device ID is required";
            }

            if (errors.Any())
            {
                return BadRequest(new { Errors = errors });
            }

            //  Get authenticated user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out int parsedUserId))
            {
                return Unauthorized("Invalid user credentials");
            }

            //  Create device
            var device = new Device
            {
                DeviceId = postDevice.DeviceId,
                Name = postDevice.Name,
                Status = postDevice.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Devices.Add(device);

            //  Create user-device relationship
            _context.User_Devices.Add(new User_Device
            {
                DeviceId = postDevice.DeviceId,
                UserId = parsedUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            // 7. Return response 
            return Ok(device);
        }

        // DELETE: api/Devices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

      
    }
}
