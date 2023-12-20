
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igs_backend.Data;
using igs_backend.Models;
using Microsoft.AspNetCore.Authorization;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace igs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncesController : ControllerBase
    {
        private readonly MyDB _context;
        private readonly string _folder;
        private readonly string _location;

        public AnnouncesController(MyDB context, IHostingEnvironment env)
        {
            _context = context;
            _folder = $@"{env.WebRootPath}\Announces";
            _location = $@"/Announces/";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Announce>>> GetAnnounce(string? language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                var announces = await _context.Announce.Where(x => x.Language == language).Select(x => new
                {
                    x.ID,
                    x.Title,
                    x.Content,
                    x.Language,
                    x.StartDate,
                    x.EndDate,
                    x.Sort,
                    x.IsPeriod
                }).ToListAsync();

                return Ok(announces);
            }
            else
            {
                var announces = await _context.Announce.Select(x => new
                {
                    x.ID,
                    x.Title,
                    x.Content,
                    x.Language,
                    x.StartDate,
                    x.EndDate,
                    x.Sort,
                    x.IsPeriod
                }).ToListAsync();
                return Ok(announces);
            }
        }

        // GET: api/Announces/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Announce>> GetAnnounce(int id)
        {
            if (_context.Announce == null)
            {
                return NotFound();
            }
            var announce = await _context.Announce.FindAsync(id);

            if (announce == null)
            {
                return NotFound();
            }

            return Ok(new
            {

                id = announce.ID,
                title = announce.Title,
                Content = announce.Content,
                Language = announce.Language,
                Sort = announce.Sort,
                StartDate = announce.StartDate,
                EndDate = announce.EndDate,
                IsPeriod = announce.IsPeriod
            });
        }

        [HttpGet]
        [Route("copy")]
        public async Task<IActionResult> GetAsyncLanguage(int id, string language)
        {
            Announce newAnnounce = new Announce();
            var announce = _context.Announce.FirstOrDefault(x => x.ID == id);
            var toUpdate = _context.Announce.FirstOrDefault(x => x.Language == language);
            if (announce == null)
            {
                return NotFound();
            }

            if (toUpdate != null)
            {
                _context.Announce.Remove(toUpdate);
                await _context.SaveChangesAsync();
                newAnnounce.Title = announce.Title;
                newAnnounce.Content = announce.Content;
                newAnnounce.StartDate = announce.StartDate;
                newAnnounce.EndDate = announce.EndDate;
                newAnnounce.IsPeriod = announce.IsPeriod;
                newAnnounce.UpdateAt = DateTime.Now;
                newAnnounce.Language = language;
            }
            else
            {
                newAnnounce.Title = announce.Title;
                newAnnounce.Content = announce.Content;
                newAnnounce.StartDate = announce.StartDate;
                newAnnounce.EndDate = announce.EndDate;
                newAnnounce.IsPeriod = announce.IsPeriod;
                newAnnounce.UpdateAt = DateTime.Now;
                newAnnounce.Language = language;
            }

            try
            {
                _context.Announce.Add(newAnnounce);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnnounceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var added = _context.Announce.Where(x => x.Language == language).FirstOrDefault();


            return Ok(added);

        }

        [HttpPatch("{id}"), Authorize]
        public async Task<IActionResult> PatchAsync(int id, DTO.AnnounceDTO announceDTO)
        {
            var announce = _context.Announce.FirstOrDefault(x => x.ID == id);
            if (announce == null)
            {
                return NotFound();
            }

            // Update other properties if needed
            if (!string.IsNullOrEmpty(announceDTO.Title))
            {
                announce.Title = announceDTO.Title;
            }
            else
            {
                announce.Title = string.Empty;
            }

            if (!string.IsNullOrEmpty(announceDTO.Content))
            {
                announce.Content = announceDTO.Content;
            }
            else
            {
                announce.Content = null;
            }


            if (!string.IsNullOrEmpty(announceDTO.Language))
            {
                announce.Language = announceDTO.Language;
            }

            if (announceDTO.StartDate != null)
            {
                announce.StartDate = announceDTO.StartDate;
            }

            switch (announceDTO.IsPeriod)
            {
                case true:
                    announce.IsPeriod = true;
                    break;
                case false:
                    announce.IsPeriod = false;
                    announce.EndDate = null;
                    break;

                default:
                    break;
            }

            if (announceDTO.EndDate != null)
            {
                announce.EndDate = announceDTO.EndDate;
            }

            announce.UpdateAt = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnnounceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(announce);
        }


        // PUT: api/Announces/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutAnnounce(int id, Announce announce)
        {
            if (id != announce.ID)
            {
                return BadRequest();
            }

            announce.UpdateAt = DateTime.Now;

            _context.Entry(announce).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnnounceExists(id))
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
                id = announce.ID,
                Title = announce.Title,
                Content = announce.Content,
                Language = announce.Language,
                StartDate = announce.StartDate,
                EndDate = announce.EndDate,
                Sort = announce.Sort
            });
        }


        // POST: api/Announces
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Announce>> PostAnnounce(Announce announce)
        {

            announce.CreatedAt = DateTime.Now;

            _context.Announce.Add(announce);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = announce.ID,
                Title = announce.Title,
                Content = announce.Content,
                Language = announce.Language,
                StartDate = announce.StartDate,
                EndDate = announce.EndDate,
                Sort = announce.Sort
            });

        }

        // DELETE: api/Announces/5
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteAnnounce(int id)
        {
            if (_context.Announce == null)
            {
                return NotFound();
            }
            var announce = await _context.Announce.FindAsync(id);
            if (announce == null)
            {
                return NotFound();
            }

            _context.Announce.Remove(announce);
            await _context.SaveChangesAsync();

            return Ok(new { });
        }


        private bool AnnounceExists(int id)
        {
            return (_context.Announce?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        //tinyMCE
        [HttpPost("upload")]

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

