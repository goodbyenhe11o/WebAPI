using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igs_backend.Data;
using igs_backend.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using igs_backend.DTO;
using Microsoft.AspNetCore.Authorization;

namespace igs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawPolicyController : ControllerBase
    {
        private readonly MyDB _context;
        private readonly string _folder;
        private readonly string _location;

        public WithdrawPolicyController(MyDB context, IHostingEnvironment env)
        {
            _context = context;
            _folder = $@"{env.WebRootPath}\WithdrawPolicy";
            _location = $@"/WithdrawPolicy/";
        }

        // GET: api/marquees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WithdrawPolicy>>> GetWithdraw(string? language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                var withdraw = await _context.WithdrawPolicy.Where(x => x.Language == language).Select(x => new
                {
                    x.ID,
                    x.Content,
                    x.Language,
                }).ToListAsync();

                return Ok(withdraw);
            }
            else
            {
                var withdraw = await _context.WithdrawPolicy.Select(x => new
                {
                    x.ID,
                    x.Content,
                    x.Language,
                }).ToListAsync();
                return Ok(withdraw);
            }
        }

        // GET: api/marquees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WithdrawPolicy>> GetWithdraw(int id)
        {
            var withdraw = await _context.WithdrawPolicy.FindAsync(id);

            if (withdraw == null)
            {
                return NotFound();
            }

            return Ok(new
            {

                id = withdraw.ID,
                Content = withdraw.Content,
                Language =withdraw.Language,
            });
        }

        [HttpGet]
        [Route("copy")]
        public async Task<IActionResult> GetAsyncLanguage(int id, string language)
        {
            WithdrawPolicy WithdrawPolicy = new WithdrawPolicy();
            var withdraw = _context.WithdrawPolicy.FirstOrDefault(x => x.ID == id);
            var toUpdate = _context.WithdrawPolicy.FirstOrDefault(x => x.Language == language);

            if ( withdraw == null)
            {
                return NotFound();
            }

            if (toUpdate != null)
            {
                _context.WithdrawPolicy.Remove(toUpdate);
                await _context.SaveChangesAsync();

                
                WithdrawPolicy.Content = withdraw.Content;
                WithdrawPolicy.Language = language;

            }
            else
            {

                WithdrawPolicy.Content = withdraw.Content;
                WithdrawPolicy.Language = language;
 
            }

            WithdrawPolicy.UpdatedAt = DateTime.Now;

            try
            {
                _context.WithdrawPolicy.Add(WithdrawPolicy);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!withdrawExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var added = _context.WithdrawPolicy.Where(x => x.Language == language).FirstOrDefault();

            return Ok(added);

        }

        [HttpPatch("{id}"), Authorize]
        public async Task<IActionResult> PatchAsync(int id, DTO.WithdrawPolicyDTO WithdrawPolicy)
        {
            var withdraw = _context.WithdrawPolicy.FirstOrDefault(x => x.ID == id);
            if (withdraw == null)
            {
                return NotFound();
            }

            // Update other properties if needed

            if (!string.IsNullOrEmpty(WithdrawPolicy.Content))
            {
                withdraw.Content = WithdrawPolicy.Content;
            }
            else
            {
                withdraw.Content = string.Empty;
            }


            if (!string.IsNullOrEmpty(WithdrawPolicy.Language))
            {
                withdraw.Language = WithdrawPolicy.Language;
            }

            withdraw.UpdatedAt = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!withdrawExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(withdraw);


        }

      
        // POST: api/marquees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<WithdrawPolicy>> Postwithdraw(WithdrawPolicy withdraw)
        {
 
            _context.WithdrawPolicy.Add(withdraw);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = withdraw.ID,
                Content = withdraw.Content,
                Language = withdraw.Language,
            });
        }

        // DELETE: api/marquees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletewithdraw(int id)
        {
            var withdraw = await _context.WithdrawPolicy.FindAsync(id);
            if (withdraw == null)
            {
                return NotFound();
            }

            _context.WithdrawPolicy.Remove(withdraw);
            await _context.SaveChangesAsync();

            return Ok(new { });
        }

        private bool withdrawExists(int id)
        {
            return (_context.WithdrawPolicy?.Any(e => e.ID == id)).GetValueOrDefault();
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
