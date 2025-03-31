namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public LogsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Logs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Log>>> GetLogs()
        {
            return await _context.Logs.ToListAsync();
        }

        // GET: api/Logs/device/5
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<LogDto>>> GetLogsByDeviceId(string deviceId)
        {
            var logs = await _context.Logs
                .Where(l => l.DeviceId == deviceId)
                .OrderByDescending(l => l.Date)
                .Select(l => new LogDto
                {
                    DeviceId = l.DeviceId,
                    Date = l.Date,
                    EndDate = l.EndDate,
                    ArmedTime = l.ArmedTime,
                    DisarmedTime = l.DisarmedTime,
                    IsTriggered = l.IsTriggered,
                    TriggeredTime = l.TriggeredTime
                })
                .ToListAsync();

            if (!logs.Any())
            {
                return NotFound($"No logs found for device with ID: {deviceId}");
            }

            return Ok(logs);
        }

        // GET: api/Logs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> GetLog(int id)
        {
            var log = await _context.Logs.FindAsync(id);

            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        // PUT: api/Logs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLog(int id, Log log)
        {
            if (id != log.Id)
            {
                return BadRequest();
            }

            _context.Entry(log).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Log>> PostLog([FromBody] PostLog postLog) 
        {
            if (postLog == null)
            {
                return BadRequest("Request body is empty");
            }

            try
            {
                var log = new Log
                {
                    DeviceId = postLog.DeviceId,
                    Date = new DateOnly(postLog.Date.Year, postLog.Date.Month, postLog.Date.Day),
                    EndDate = new DateOnly(postLog.EndDate.Year, postLog.EndDate.Month, postLog.EndDate.Day),
                    ArmedTime = new TimeOnly(postLog.ArmedTime.Hour, postLog.ArmedTime.Minute),
                    DisarmedTime = new TimeOnly(postLog.DisarmedTime.Hour, postLog.DisarmedTime.Minute),
                    IsTriggered = postLog.IsTriggered,
                    TriggeredTime = postLog.TriggeredTime != null
                        ? new TimeOnly(postLog.TriggeredTime.Hour, postLog.TriggeredTime.Minute)
                        : null
                };

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetLog), new { id = log.Id }, log);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // DELETE: api/Logs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            _context.Logs.Remove(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LogExists(int id)
        {
            return _context.Logs.Any(e => e.Id == id);
        }
    }
}
