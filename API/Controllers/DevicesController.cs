using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

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

        // GET: api/Devices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDevice(int id)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return device;
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

        [HttpPut("UpdateName/{id}")]
        public async Task<IActionResult> PutDeviceName(int id, UpdateName device)
        {
            // Find the device in the database
            var deviceEntity = await _context.Devices.FindAsync(id);
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


        [HttpPost("Login")]
        public async Task<ActionResult<Device>> DeviceLogin(DeviceLogin deviceLogin)
        {
            Device device = new()
            {
                DeviceId = deviceLogin.DeviceId,
                Password = deviceLogin.Password,
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = device.Id }, device);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Device>> PostDevice(PostDevice postdevice)
        {
            Regex validateDevicePassword = new(@"^[0-9]{1,}$");
            var errors = new Dictionary<string, string>();

            if (!validateDevicePassword.IsMatch(postdevice.Password))
            {
                errors["Username"] = "Username must be 5-15 characters long and contain only letters and numbers.";
            }

            if (errors.Count > 0)
            {
                return BadRequest(new { Errors = errors });
            }

            // Get user ID from token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest(new { Message = "Invalid user ID format." });
            }

            // Check if the DeviceId already exists in the Devices table
            var deviceExists = await _context.Devices.AnyAsync(d => d.DeviceId == postdevice.DeviceId);
            if (deviceExists)
            {
                return BadRequest(new { Message = $"Device with ID {postdevice.DeviceId} already exists." });
            }

            // Create and save the new Device
            Device device = new()
            {
                DeviceId = postdevice.DeviceId,
                Status = postdevice.Status,
                Password = postdevice.Password,
                Name = postdevice.Name,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            // Create and save the User_Device relationship
            User_Device joinDeviceAndUser = new()
            {
                DeviceId = postdevice.DeviceId,
                UserId = userId,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };
            _context.User_Devices.Add(joinDeviceAndUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = device.Id }, device);
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
