using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igs_backend.Data;
using igs_backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace igs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDB _context;
        private readonly IConfiguration _configuration;

        public UsersController(MyDB context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Users
        [HttpGet, Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
          if (_context.User == null)
          {
              return NotFound();
          }
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
          if (_context.User == null)
          {
              return NotFound();
          }
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPatch("{id}"), Authorize]
        public async Task<ActionResult<User>> PatchUser(int id, DTO.RegisterUserDTO registerUserDTO)
        {
            var user = _context.User.FirstOrDefault(x => x.ID == id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!string.IsNullOrEmpty(registerUserDTO.Password))
            {
                byte[] salt = Code.Utility.GenerateSalt();
                string passwordHash =Code.Utility.ComputePasswordHash(registerUserDTO.Password, salt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = Convert.ToBase64String(salt);
            }

            if (!string.IsNullOrEmpty(registerUserDTO.Phone))
            {
                user.Phone = registerUserDTO.Phone;
            }

            if (!string.IsNullOrEmpty(registerUserDTO.Remark))
            {
                user.Remark = registerUserDTO.Remark;
            }

            user.UpdatedAt = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(user);
        }


        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { });
        }

        private bool UserExists(int id)
        {
            return (_context.User?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
