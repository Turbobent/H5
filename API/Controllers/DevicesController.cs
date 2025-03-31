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

        // PUT: api/Devices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(int id, Device device)
        {
            if (id != device.Id)
            {
                return BadRequest();
            }

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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

        [HttpPut("{deviceId}/password")]
        public async Task<IActionResult> UpdateDevicePassword(
        string deviceId,
        [FromBody] UpdatePassword request)  
        {
            // 1. Validate inputs
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return BadRequest("Device ID is required");
            }

            if (request == null || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("New password is required");
            }

            // 2. Find device
            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null)
            {
                return NotFound($"Device {deviceId} not found");
            }

            // 3. Validate password format
            if (!Regex.IsMatch(request.NewPassword, @"^\d{4,8}$"))  // 4-8 digits
            {
                return BadRequest("Password must be 4-8 digits");
            }

            // 4. Update and save
            await _context.SaveChangesAsync();

            return NoContent();  // 204 No Content
        }

        [HttpPost("Login")]
        public async Task<ActionResult<Device>> DeviceLogin(DeviceLogin deviceLogin)
        {
            Device device = new()
            {
                DeviceId = deviceLogin.DeviceId,
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = device.Id }, device);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Device>> PostDevice(PostDevice postDevice)
        {
            // 1. Validate input
            var errors = new Dictionary<string, string>();

            if (await _context.Devices.AnyAsync(d => d.DeviceId.ToLower() == postDevice.DeviceId.ToLower()))
            {
                return Conflict($"Device {postDevice.DeviceId} already exists");
            }

            if (!Regex.IsMatch(postDevice.Password, @"^\d{4,8}$"))
            {
                errors["Password"] = "Password must be 4-8 numeric digits";
            }

            if (string.IsNullOrWhiteSpace(postDevice.DeviceId))
            {
                errors["DeviceId"] = "Device ID is required";
            }

            if (errors.Any())
            {
                return BadRequest(new { Errors = errors });
            }

            // 2. Get authenticated user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out int parsedUserId))
            {
                return Unauthorized("Invalid user credentials");
            }

            // 3. Ensure ChangeTracker is clear before checking for duplicates
            _context.ChangeTracker.Clear();

            // 3. Check for existing device
            if (await _context.Devices.AnyAsync(d => d.DeviceId == postDevice.DeviceId))
            {
                return Conflict($"Device {postDevice.DeviceId} already exists");
            }

            // 4. Create shared password if it doesn't exist
            var sharedPassword = await _context.SharedPasswords
                .FirstOrDefaultAsync(sp => sp.HashedPassword == postDevice.Password);

            if (sharedPassword == null)
            {
                sharedPassword = new SharedPassword
                {
                    PasswordId = $"SP_{Guid.NewGuid().ToString("N")}",
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword(postDevice.Password),
                    CreatedAt = DateTime.UtcNow
                };
                _context.SharedPasswords.Add(sharedPassword);
            }

            // 5. Create device
            var device = new Device
            {
                DeviceId = postDevice.DeviceId,
                Name = postDevice.Name,
                Status = postDevice.Status,
                SharedPasswordId = sharedPassword.PasswordId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Devices.Add(device);

            // 6. Create user-device relationship
            _context.User_Devices.Add(new User_Device
            {
                DeviceId = postDevice.DeviceId,
                UserId = parsedUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            // 7. Return response (excluding sensitive data)
            return CreatedAtAction(nameof(GetDevice), new { id = device.DeviceId }, new
            {
                device.DeviceId,
                device.Name,
                device.Status,
                CreatedAt = device.CreatedAt
            });
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

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.Id == id);
        }
    }
}
