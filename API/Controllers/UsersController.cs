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

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, Edit user)
        {
            var userEdit = await _context.Users.FindAsync(id);

            if (userEdit == null)
            {
                return NotFound();
            }

            // Hash the password
            var HashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Regex patterns
            Regex validateUsername = new(@"^[a-zA-Z0-9]{5,15}$");  // Only letters and numbers (5-15 chars)
            Regex validateEmail = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");  // Standard email format
            Regex validatePassword = new(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"); // Strong password

            var errors = new Dictionary<string, string>();

            // Validate username format only if changed
            if (user.Username != userEdit.Username)
            {
                if (!validateUsername.IsMatch(user.Username))
                {
                    errors["Username"] = "Username must be 5-15 characters long and contain only letters and numbers.";
                }
                else if (_context.Users.Any(x => x.Username == user.Username))
                {
                    errors["Username"] = "Username is already taken.";
                }
            }

            // Validate email format
            if (!validateEmail.IsMatch(user.Email))
            {
                errors["Email"] = "Invalid email format.";
            }
            else if (user.Email != userEdit.Email && _context.Users.Any(x => x.Email == user.Email))
            {
                errors["Email"] = "Email is already registered.";
            }

            // Validate password strength
            if (!validatePassword.IsMatch(user.Password))
            {
                errors["Password"] = "Password must be at least 8 characters long, contain at least one letter, one number, and one special character.";
            }

            if (errors.Count > 0)
            {
                return BadRequest(new { Errors = errors });
            }

            // Update fields
            userEdit.Username = user.Username;
            userEdit.Email = user.Email;
            userEdit.HashedPassword = HashedPassword;
            userEdit.Salt = HashedPassword.Substring(0, 29);
            userEdit.UpdatedAt = DateTime.UtcNow;

            _context.Entry(userEdit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
            var findUser = await _context.Users.SingleOrDefaultAsync(x => x.Username == userLogin.UserName || x.Email == userLogin.UserName);

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
