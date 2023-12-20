using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igs_backend.Data;
using igs_backend.Models;
using igs_backend.DTO;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Authorization;

namespace igs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenancesController : ControllerBase
    {
        private readonly MyDB _context;
        private readonly string _folder;
        private readonly string _location;

        public MaintenancesController(MyDB context, IHostingEnvironment env)
        {
            _context = context;
            _folder = $@"{env.WebRootPath}\Maintenances";
            _location =$@"/Maintenances/";
        }

        // GET: api/Maintenances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenanceDTO>>> GetMaintenance(string? language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                var maintenances = await _context.Maintenance.Where(x => x.Language == language).Select(x => new
                {
                    x.ID,
                    x.Content,
                    x.Language,
                    x.Sort,
                    x.StartDate,
                    x.EndDate,
                    x.IsPeriod,
                }).ToListAsync();

                return Ok(maintenances);
            }
            else
            {
                var maintenances = await _context.Maintenance.Select(x => new
                {
                    x.ID,
                    x.Content,
                    x.Language,
                    x.Sort,
                    x.StartDate,
                    x.EndDate,
                    x.IsPeriod,
                }).ToListAsync();

                return Ok(maintenances); 
            }
        }

        // GET: api/Maintenances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceDTO>> GetMaintenance(int id)
        {
            var maintenance = await _context.Maintenance.FindAsync(id);

            if (maintenance == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = maintenance.ID,
                Content = maintenance.Content,
                Language = maintenance.Language,
                Sort = maintenance.Sort,
                StartDate = maintenance.StartDate,
                EndDate = maintenance.EndDate,
                isPeriod = maintenance.IsPeriod,
            });
        }

        [HttpPatch("{id}"), Authorize]
        public async Task<IActionResult> PatchAsync(int id, MaintenanceDTO maintenanceDTO)
        {
            var maintenance = _context.Maintenance.FirstOrDefault(x => x.ID == id);
            if (maintenance == null)
            {
                return NotFound();
            }

            // Update other properties if needed

            if (!string.IsNullOrEmpty(maintenanceDTO.Content))
            {
                maintenance.Content = maintenanceDTO.Content;
            }
            else
            {
                maintenance.Content = string.Empty;
            }

            if (!string.IsNullOrEmpty(maintenanceDTO.Language))
            {
                maintenance.Language = maintenanceDTO.Language;
            }

            if (maintenanceDTO.StartDate != null)
            {
                maintenance.StartDate = maintenanceDTO.StartDate;
            }

            if(maintenanceDTO.EndDate != null)
            {
                maintenance.EndDate = maintenanceDTO.EndDate;
            }
            else
            {
                maintenance.EndDate = null;
            }

            switch (maintenanceDTO.IsPeriod)
            {
                case true:
                    maintenance.IsPeriod = true;
                    break;
                case false:
                    maintenance.IsPeriod = false;
                    maintenance.EndDate = null;
                    break;

                default:
                    break;
            }

            maintenance.UpdatedAt = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaintenanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(maintenance);


        }

        // PUT: api/Maintenances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutMaintenance(int id, Maintenance maintenance)
        {
            if (id != maintenance.ID)
            {
                return BadRequest();
            }

            maintenance.UpdatedAt = DateTime.Now;
            _context.Entry(maintenance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaintenanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new
            {
                id = maintenance.ID,
                Content = maintenance.Content,
                Language = maintenance.Language,
                Sort = maintenance.Sort,
                StartDate = maintenance.StartDate,
                EndDate = maintenance.EndDate,
                isPeriod = maintenance.IsPeriod,
            });
        }

        // POST: api/Maintenances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Maintenance>> PostMaintenance(Maintenance maintenance)
        {

            _context.Maintenance.Add(maintenance);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = maintenance.ID,
                Content = maintenance.Content,
                Language = maintenance.Language,
                Sort = maintenance.Sort,
                StartDate = maintenance.StartDate,
                EndDate = maintenance.EndDate,
                isPeriod = maintenance.IsPeriod,
            });
        }

        // DELETE: api/Maintenances/5
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteMaintenance(int id)
        {
            var maintenance = await _context.Maintenance.FindAsync(id);
            if (maintenance == null)
            {
                return NotFound();
            }

            _context.Maintenance.Remove(maintenance);
            await _context.SaveChangesAsync();

            return Ok(new{ });
        }

        private bool MaintenanceExists(int id)
        {
            return (_context.Maintenance?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        [HttpPost("upload")]
        [AllowAnonymous]
        public IActionResult UploadImage(IFormFile file)
        {
            // Check if a file was provided
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided.");
            }
            else
            {
                var fileName = $@"{Path.GetFileNameWithoutExtension(file.FileName)}-{DateTime.Now:yyyyMMddHHmm}{Path.GetExtension(file.FileName)}";
                string contentType = file.ContentType;
                if (contentType != "image/jpeg" && contentType != "image/png" && contentType != "image/jpg")
                {
                    return BadRequest("Only JPG, PNG, and JPEG image types are allowed.");
                }
                else
                {
                    var path = $@"{_folder}\{fileName}";
                    var domain = HttpContext.Request.Host;
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    // Return the URL of the uploaded image
                    var location = $@"{HttpContext.Request.Scheme}://" + domain +_location + fileName;

                    return Ok(new { location });
                }

            }
        }
    }
}
