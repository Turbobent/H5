namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AppDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, Edit user)
        {
            var userEdit = await _context.Users.FindAsync(id);

            if (userEdit == null)
            {
                return NotFound();
            }

            // Regex patterns
            Regex validateUsername = new(@"^[a-zA-Z0-9]{5,15}$");  // Only letters and numbers (5-15 chars)
            Regex validateEmail = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");  // Standard email format
            Regex validatePassword = new(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"); // Strong password

            var errors = new Dictionary<string, string>();

            // Validate username format only if changed
            if (!string.IsNullOrWhiteSpace(user.Username) && user.Username != userEdit.Username)
            {
                if (!validateUsername.IsMatch(user.Username))
                {
                    errors["Username"] = "Username must be 5-15 characters long and contain only letters and numbers.";
                }
                else if (_context.Users.Any(x => x.Username == user.Username && x.Id != id))
                {
                    errors["Username"] = "Username is already taken.";
                }
            }

            // Validate email format
            if (!string.IsNullOrWhiteSpace(user.Email) && user.Email != userEdit.Email)
            {
                if (!validateEmail.IsMatch(user.Email))
                {
                    errors["Email"] = "Invalid email format.";
                }
                else if (_context.Users.Any(x => x.Email == user.Email && x.Id != id))
                {
                    errors["Email"] = "Email is already registered.";
                }
            }

            // Validate password strength only if provided
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                if (!validatePassword.IsMatch(user.Password))
                {
                    errors["Password"] = "Password must be at least 8 characters long, contain at least one letter, one number, and one special character.";
                }
                else
                {
                    userEdit.HashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    userEdit.Salt = userEdit.HashedPassword.Substring(0, 29);
                }
            }

            if (errors.Count > 0)
            {
                return BadRequest(new { Errors = errors });
            }

            // Update fields only if they are provided
            if (!string.IsNullOrWhiteSpace(user.Username)) userEdit.Username = user.Username;
            if (!string.IsNullOrWhiteSpace(user.Email)) userEdit.Email = user.Email;

            userEdit.UpdatedAt = DateTime.UtcNow;

            _context.Entry(userEdit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == id))
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

        [HttpPost("signUp")]
        public async Task<ActionResult<User>> Signup(Signup userSignUp)
        {
            // Regex patterns
            Regex validateUsername = new(@"^[a-zA-Z0-9]{5,15}$");  // Only letters and numbers (5-15 chars)
            Regex validateEmail = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");  // Standard email format
            Regex validatePassword = new(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"); // Strong password

            // Dictionary to store validation errors
            var errors = new Dictionary<string, string>();

            // Validate username format
            if (!validateUsername.IsMatch(userSignUp.Username))
            {
                errors["Username"] = "Username must be 5-15 characters long and contain only letters and numbers.";
            }

            // Check if username already exists
            if (_context.Users.Any(x => x.Username == userSignUp.Username))
            {
                errors["Username"] = "Username is already taken.";
            }

            // Validate email format
            if (!validateEmail.IsMatch(userSignUp.Email))
            {
                errors["Email"] = "Invalid email format.";
            }

            // Check if email already exists
            if (_context.Users.Any(x => x.Email == userSignUp.Email))
            {
                errors["Email"] = "Email is already registered.";
            }

            // Validate password strength
            if (!validatePassword.IsMatch(userSignUp.Password))
            {
                errors["Password"] = "Password must be at least 8 characters long, contain at least one letter, one number, and one special character.";
            }

            // If there are validation errors, return BadRequest (400) with error details
            if (errors.Count > 0)
            {
                return BadRequest(new { Errors = errors });
            }

            // Hash the password
            var HashedPassword = BCrypt.Net.BCrypt.HashPassword(userSignUp.Password);

            // Create new user object
            User user = new()
            {
                Email = userSignUp.Email,
                Username = userSignUp.Username,
                HashedPassword = HashedPassword,
                Salt = HashedPassword.Substring(0, 29),
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            // Save to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login userLogin)
        {
            var findUser = await _context.Users.SingleOrDefaultAsync(x => x.Username == userLogin.Username || x.Email == userLogin.Username);

            if (findUser == null || !BCrypt.Net.BCrypt.Verify(userLogin.Password, findUser.HashedPassword))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = GenerateJwtToken(findUser);

            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("email", user.Email),
                new Claim("name", user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"] ?? Environment.GetEnvironmentVariable("Key")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(

            _configuration["JwtSettings:Issuer"] ?? Environment.GetEnvironmentVariable("Issuer"),
            _configuration["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("Audience"),

            claims,

            expires: DateTime.Now.AddDays(30),

            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully" });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
