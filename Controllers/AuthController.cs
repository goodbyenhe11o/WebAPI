
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igs_backend.Data;
using igs_backend.Models;
using System.Security.Cryptography;
using igs_backend.Code;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace igs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyDB _context;
        private readonly IConfiguration _configuration;
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, DateTime> _userActivity =
        new System.Collections.Concurrent.ConcurrentDictionary<string, DateTime>();

        public AuthController(MyDB context,
         IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Users

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register(DTO.RegisterUserDTO registerUserDTO)
        {

            var isCorrect = Code.Utility.CheckEmailFormat(registerUserDTO.Email);
            if (!isCorrect)
            {
                var msg = "Please check your email format if it's correct";
                return BadRequest(msg);
            }

            if (await _context.User.AnyAsync(x => x.Email == registerUserDTO.Email))
            {
                return Conflict("Username is already taken.");
            }

            byte[] salt = GenerateSalt();
            string passwordHash = ComputePasswordHash(registerUserDTO.Password, salt);


            var user = new User
            {
                Email = registerUserDTO.Email,
                PasswordHash = passwordHash,
                PasswordSalt = Convert.ToBase64String(salt),
                Phone = registerUserDTO.Phone,
                Remark = registerUserDTO.Remark,
                Role = registerUserDTO.Role,
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            var newUserId = user.ID;

            var permissions = await _context.Permissions.ToListAsync();

            foreach (var permission in permissions)
            {
                var newUserPermission = new UserPermissions
                {
                    UserID = newUserId,
                    PermissionsID = permission.ID,
                    isActive = false // Set IsActive to true or false as needed
                };

                _context.UserPermissions.Add(newUserPermission);
            }

            await _context.SaveChangesAsync();


            var userPermissions = await _context.UserPermissions
         .Where(up => up.UserID == newUserId)
         .ToListAsync();

            var permissionDictionary = new Dictionary<string, bool>();

            var allPermissions = await _context.Permissions.ToListAsync();
            foreach (var permission in allPermissions)
            {
                permissionDictionary.Add(permission.Name, false);
            }

            foreach (var userPermission in userPermissions)
            {
                if (permissionDictionary.ContainsKey(userPermission.Permissions.Name))
                {
                    permissionDictionary[userPermission.Permissions.Name] = userPermission.isActive;
                }
            }

            var result = new
            {
                UserId = newUserId,
                Permissions = permissionDictionary
            };

            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login(DTO.LoginUserDTO loginUser)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == loginUser.Email);

            if (user == null)
            {
                return NotFound("Invalid username or password.");
            }

            byte[] salt = Convert.FromBase64String(user.PasswordSalt);
            string passwordHash = ComputePasswordHash(loginUser.Password, salt);

            if (user.PasswordHash != passwordHash)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Authentication successful, generate JWT token
            string token = CreateToken(user);



            return Ok(new
            {
                token = token,
                loginStatus = "success",
            });

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return Ok(new
            {
                message = "log out successfully"
            });
        }


        [Authorize]
        [HttpPost("updateActivity")]
        public IActionResult UpdateActivity()
        {
            //update the user's last activity timestamp in session
            HttpContext.Session.SetString("LastActivity", DateTime.UtcNow.ToString());
            return Ok();
        }

        [Authorize]
        [HttpGet("checkIdleTime")]
        public IActionResult CheckIdleTime()
        {
            var lastActivityString = HttpContext.Session.GetString("LastActivity");
            if (string.IsNullOrEmpty(lastActivityString))
            {
                // User has no recorded activity yet, consider it idle
                return Ok(true);
            }

            var lastActivity = DateTime.Parse(lastActivityString);
            var idleTime = DateTime.UtcNow - lastActivity;

            if (idleTime.TotalMinutes >= 30)
            {
                return Ok(true);
            }

            return Ok(false);
        }


        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private string ComputePasswordHash(string password, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hash);
            }
        }

        private string CreateToken(User user)
        {
            var userPermissions = _context.UserPermissions
       .Where(up => up.UserID == user.ID)
       .ToList();

            var permissionDictionary = new Dictionary<string, bool>();

            var allPermissions = _context.Permissions.ToList();
            foreach (var permission in allPermissions)
            {
                permissionDictionary.Add(permission.Name, false);
            }

            foreach (var userPermission in userPermissions)
            {
                if (permissionDictionary.ContainsKey(userPermission.Permissions.Name))
                {
                    permissionDictionary[userPermission.Permissions.Name] = userPermission.isActive;
                }
            }
            var permissionJson = JsonConvert.SerializeObject(permissionDictionary);

            //var permissionString = string.Join(',', permissionDictionary.Select(kv => $"{kv.Key}:{kv.Value}"));
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                //new Claim("Permissions", permissionString)
                new Claim("Permissions", permissionJson)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                //expires: DateTime.Now.AddDays(1),
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
