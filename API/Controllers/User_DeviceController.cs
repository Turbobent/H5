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

        // GET: api/User_Device/user-with-devices/5
        [HttpGet("user-with-devices/{userId}")]
        public async Task<ActionResult<IEnumerable<User_Device>>> GetUserWithDevices(int userId)
        {
            var userDevices = await _context.User_Devices
                .Where(ud => ud.UserId == userId)
                .Include(ud => ud.Device)
                .Include(ud => ud.UserId)
                .ToListAsync();

            if (userDevices == null || !userDevices.Any())
            {
                return NotFound();
            }

            return Ok(userDevices);
        }

        // PUT: api/User_Device/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser_Device(int id, User_Device user_Device)
        {
            if (id != user_Device.Id)
            {
                return BadRequest();
            }

            _context.Entry(user_Device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!User_DeviceExists(id))
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

        // POST: api/User_Device
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User_Device>> PostUser_Device(User_Device user_Device)
        {
            _context.User_Devices.Add(user_Device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser_Device", new { id = user_Device.Id }, user_Device);
        }

        // DELETE: api/User_Device/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser_Device(int id)
        {
            var user_Device = await _context.User_Devices.FindAsync(id);
            if (user_Device == null)
            {
                return NotFound();
            }

            _context.User_Devices.Remove(user_Device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool User_DeviceExists(int id)
        {
            return _context.User_Devices.Any(e => e.Id == id);
        }
    }
}
