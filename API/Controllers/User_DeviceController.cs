﻿namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User_DeviceController : ControllerBase
    {
        private readonly AppDBContext _context;

        public User_DeviceController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/User_Device
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User_Device>>> GetUser_Devices()
        {
            return await _context.User_Devices.ToListAsync();
        }

        // GET: api/UserDevices/device-ids/5
        [HttpGet("device-ids/{userId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserDeviceIds(int userId)
        {
            var deviceIds = await _context.User_Devices
                .Where(ud => ud.UserId == userId)
                .Select(ud => ud.DeviceId)
                .ToListAsync();

            if (!deviceIds.Any())
            {
                return NotFound($"No devices found for user with ID: {userId}");
            }

            return Ok(deviceIds);
        }   
    }
}
