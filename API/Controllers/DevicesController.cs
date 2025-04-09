
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

        [Authorize]
        [HttpGet("{deviceId}")]
        public async Task<ActionResult<Device>> GetDevice(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return BadRequest("Device ID is required");
            }

            // Get the authenticated user's ID from the JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User ID is missing or invalid in token.");
            }

            // Check if the user owns the device
            var userOwnsDevice = await _context.User_Devices
                .AnyAsync(ud => ud.UserId == userId && ud.DeviceId == deviceId);

            if (!userOwnsDevice)
            {
                return Forbid("You do not have permission to access this device.");
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
        [Authorize]
        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> DeleteDevice(string deviceId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User ID is missing or invalid.");
            }

            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null)
            {
                return NotFound("Device not found.");
            }

            var logs = await _context.Logs
                .Where(l => l.DeviceId == deviceId)
                .ToListAsync();

            if (!logs.Any())
            {
                return NotFound("Logs not found");
            }

            var userOwnsDevice = await _context.User_Devices
                .AnyAsync(ud => ud.UserId == userId && ud.DeviceId == deviceId);

            if (!userOwnsDevice)
            {
                return Forbid("You do not have permission to delete this device.");
            }

            var userDeviceLinks = await _context.User_Devices
                .Where(ud => ud.DeviceId == deviceId)
                .ToListAsync();

            _context.User_Devices.RemoveRange(userDeviceLinks);
            _context.Logs.RemoveRange(logs);
            _context.Devices.Remove(device);

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
