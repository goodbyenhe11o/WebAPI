using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igs_backend.Data;
using igs_backend.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Authorization;

namespace igs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositPolicyController : ControllerBase
    {
        private readonly MyDB _context;
        private readonly string _folder;
        private readonly string _location;

        public DepositPolicyController(MyDB context, IHostingEnvironment env)
        {
            _context = context;
            _folder = $@"{env.WebRootPath}\DepositPolicy";
            _location = $@"/DepositPolicy/";
        }

        // GET: api/DepositPolicys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepositPolicy>>> GetDeposit(string? language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                var deposit = await _context.DepositPolicy.Where(x => x.Language == language).Select(x => new
                {
                    x.ID,
                    x.Content,
                    x.Language,
                }).ToListAsync();

                return Ok(deposit);
            }
            else
            {
                var deposit = await _context.DepositPolicy.Select(x => new
                {
                    x.ID,
                    x.Content,
                    x.Language,
                }).ToListAsync();
                return Ok(deposit);
            }
        }

        // GET: api/DepositPolicys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DepositPolicy>> GetDeposit(int id)
        {
            var deposit = await _context.DepositPolicy.FindAsync(id);

            if (deposit == null)
            {
                return NotFound();
            }

            return Ok(new
            {

                id = deposit.ID,
                Content = deposit.Content,
                Language =deposit.Language,

            });
        }

        [HttpGet]
        [Route("copy")]
        public async Task<IActionResult> GetAsyncLanguage(int id, string language)
        {
            DepositPolicy depositPolicy = new DepositPolicy();
            var deposit = _context.DepositPolicy.FirstOrDefault(x => x.ID == id);
            var toUpdate = _context.DepositPolicy.FirstOrDefault(x => x.Language == language);

            if ( deposit == null)
            {
                return NotFound();
            }

            if (toUpdate != null)
            {
                _context.DepositPolicy.Remove(toUpdate);
                await _context.SaveChangesAsync();

                
                depositPolicy.Content = deposit.Content;
                depositPolicy.Language = language;

            }
            else
            {

                depositPolicy.Content = deposit.Content;
                depositPolicy.Language = language;
 
            }

            depositPolicy.UpdatedAt = DateTime.Now;

            try
            {
                _context.DepositPolicy.Add(depositPolicy);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepositExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var added = _context.DepositPolicy.Where(x => x.Language == language).FirstOrDefault();

            return Ok(added);

        }

        [HttpPatch("{id}"), Authorize]
        public async Task<IActionResult> PatchAsync(int id, DTO.DepositPolicyDTO depositPolicy)
        {
            var deposit = _context.DepositPolicy.FirstOrDefault(x => x.ID == id);
            if (deposit == null)
            {
                return NotFound();
            }

            // Update other properties if needed

            if (!string.IsNullOrEmpty(depositPolicy.Content))
            {
                deposit.Content = depositPolicy.Content;
            }
            else
            {
                deposit.Content = null;
            }


            if (!string.IsNullOrEmpty(depositPolicy.Language))
            {
                deposit.Language = depositPolicy.Language;
            }

            deposit.UpdatedAt = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepositExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(deposit);


        }

        // POST: api/DepositPolicys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<DepositPolicy>> PostDeposit( DepositPolicy deposit)
        {

            _context.DepositPolicy.Add(deposit);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = deposit.ID,
                Content = deposit.Content,
                Language = deposit.Language,
            });
        }

        // DELETE: api/DepositPolicys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeposit(int id)
        {
            var deposit = await _context.DepositPolicy.FindAsync(id);
            if (deposit == null)
            {
                return NotFound();
            }

            _context.DepositPolicy.Remove(deposit);
            await _context.SaveChangesAsync();

            return Ok(new { });
        }

        private bool DepositExists(int id)
        {
            return (_context.DepositPolicy?.Any(e => e.ID == id)).GetValueOrDefault();
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
                    var location = $@"{HttpContext.Request.Scheme}://" + domain + _location + fileName;

                    return Ok(new { location });
                }

            }
        }
    }
}
