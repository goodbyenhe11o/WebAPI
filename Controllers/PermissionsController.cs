using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igs_backend.Data;
using igs_backend.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authorization;

namespace igs_backend.Controllers
{
    [Route("api/permissions")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly MyDB _context;
       

        public PermissionsController(MyDB context)
        {
            _context = context;
        }


        // GET: api/UserPermissions/5
        [HttpGet("{userID}"), Authorize]
        public async Task<ActionResult<IEnumerable<UserPermissions>>> GetUserPermissions(int userID)
        {
   
            var userPermissions = await _context.UserPermissions.Where(x=>x.UserID == userID).Include(x=>x.Permissions).ToListAsync();

            var permissionDictionary = new Dictionary<string, bool>();

            if (userPermissions == null)
            {
                return NotFound();
            }

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

            var result = new
            {
                Permission = permissionDictionary,
            };

            return Ok(permissionDictionary);
        }

        [HttpPut("{userID}"), Authorize]
  
        public async Task<IActionResult> UpdateUserPermissions(int userID, Dictionary<string, bool> _permissions)
        {
            var userPermissions = await _context.UserPermissions
                .Where(up => up.UserID == userID)
                .Include(up => up.Permissions)
                .ToListAsync();

            if (userPermissions.Count == 0)
            {
                return NotFound("User permissions not found.");
            }

            foreach (var kvp in _permissions)
            {
                var permissionName = kvp.Key;
                var isActive = kvp.Value;

                var userPermission = userPermissions.FirstOrDefault(up => up.Permissions.Name == permissionName);

                if (userPermission != null)
                {
                    userPermission.isActive = isActive;
                }
            }

            await _context.SaveChangesAsync();

            var newUserPermissions = await _context.UserPermissions.Where(x => x.UserID == userID).Include(x => x.Permissions).ToListAsync();

            var permissionDictionary = new Dictionary<string, bool>();

            var allPermissions = _context.Permissions.ToList();
            foreach (var permission in allPermissions)
            {
                permissionDictionary.Add(permission.Name, false);
            }

            foreach (var u in newUserPermissions)
            {
                if (permissionDictionary.ContainsKey(u.Permissions.Name))
                {
                    permissionDictionary[u.Permissions.Name] =u.isActive;
                }
            }



            return Ok(permissionDictionary);
        }


    
        // POST: api/UserPermissions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{userID}"), Authorize]
        public async Task<ActionResult<UserPermissions>> PostUserPermissions(int userID)
        {
            var a = "";
            var user = await _context.User.FindAsync(userID);

            if (user == null)
            {
                return NotFound();
            }
            var userPermissions = _context.UserPermissions.Where(x => x.UserID == userID);

            if(userPermissions.Count() > 1)
            {
                return BadRequest("user permissions have been granted");
            }

            var permissions = await _context.Permissions.ToListAsync();
            if (user.Role == "Admin")
            {

                foreach (var permission in permissions)
                {
                    var userPermission = new UserPermissions
                    {
                        UserID = user.ID,
                        PermissionsID = permission.ID,
                        isActive = true,
                    };

                    _context.UserPermissions.Add(userPermission);
                }

                await _context.SaveChangesAsync();

                return Ok("User permissions added.");
            }
            else
            {
                foreach (var permission in permissions)
                {
                    var userPermission = new UserPermissions
                    {
                        UserID = user.ID,
                        PermissionsID = permission.ID,
                        isActive = false,
                    };

                    _context.UserPermissions.Add(userPermission);
                }

                await _context.SaveChangesAsync();

                return Ok("User permissions added but not active.");
            }

        }

    }
}
