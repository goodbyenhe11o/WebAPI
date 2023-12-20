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
    public class MarqueeController : ControllerBase
    {
        private readonly MyDB _context;

        private readonly string _folder;
        private readonly string _location;

        public MarqueeController(MyDB context, IHostingEnvironment env)
        {
            _context = context;
            _folder = $@"{env.WebRootPath}\Marquees";
            _location = $@"/Marquees/";
        }

        // GET: api/marquees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marquee>>> GetMarquee(string? language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                var marquees = await _context.Marquee.Where(x => x.Language == language).Select(x => new
                {
                    x.ID,
                    x.Content,
                    x.Language,
                    x.Sort,
                    x.StartDate,
                    x.EndDate,
                    x.IsPeriod,
                }).ToListAsync();

                return Ok(marquees);
            }
            else
            {
                var marquees = await _context.Marquee.Select(x => new
                {
                    x.ID,
                    x.Content,
                    x.Language,
                    x.Sort,
                    x.StartDate,
                    x.EndDate,
                    x.IsPeriod,
                }).ToListAsync();
                return Ok(marquees);
            }
        }

        // GET: api/marquees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Marquee>> GetMarquee(int id)
        {
            var marquee = await _context.Marquee.FindAsync(id);

            if (marquee == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = marquee.ID,
                Content = marquee.Content,
                Language = marquee.Language,
                Sort = marquee.Sort,
                StartDate = marquee.StartDate,
                EndDate = marquee.EndDate,
                isPeriod = marquee.IsPeriod,
            });
        }

        [HttpGet]
        [Route("copy")]
        public async Task<IActionResult> GetAsyncLanguage(int id, string language)
        {
            Marquee newMarquee = new Marquee();
            var marquee = _context.Marquee.FirstOrDefault(x => x.ID == id);
            var toUpdate = _context.Marquee.FirstOrDefault(x => x.Language == language);

            var a = "";
            if (marquee == null)
            {
                return NotFound();
            }

            if (toUpdate != null)
            {
                _context.Marquee.Remove(toUpdate);
                await _context.SaveChangesAsync();

              
                newMarquee.Content = marquee.Content;
                newMarquee.Language = language;
                newMarquee.Sort = marquee.Sort;
                newMarquee.StartDate = marquee.StartDate;
                newMarquee.EndDate = marquee.EndDate;
                newMarquee.IsPeriod = marquee.IsPeriod;
            }
            else
            {
               
                newMarquee.Content = marquee.Content;
                newMarquee.Language = language;
                newMarquee.Sort = marquee.Sort;
                newMarquee.StartDate = marquee.StartDate;
                newMarquee.EndDate = marquee.EndDate;
                newMarquee.IsPeriod = marquee.IsPeriod;
            }

            try
            {
                _context.Marquee.Add(newMarquee);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarqueeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var added = _context.Marquee.Where(x => x.Language == language).FirstOrDefault();

            return Ok(added);

        }

        [HttpPatch("{id}"), Authorize]
        public async Task<IActionResult> PatchAsync(int id,  DTO.MarqueeDTO marqueeDTO)
        {
            var marquee = _context.Marquee.FirstOrDefault(x => x.ID == id);
            if (marquee == null)
            {
                return NotFound();
            }

            // Update other properties if needed

            if (!string.IsNullOrEmpty(marqueeDTO.Content))
            {
                marquee.Content = marqueeDTO.Content;
            }
            else
            {
                marquee.Content = string.Empty;
            }


            if (!string.IsNullOrEmpty(marqueeDTO.Language))
            {
                marquee.Language = marqueeDTO.Language;
            }

            if (marqueeDTO.StartDate != null)
            {
                marquee.StartDate = marqueeDTO.StartDate;

            }
            
            if(marqueeDTO.EndDate != null)
            {
                marquee.EndDate = marqueeDTO.EndDate;
            }

            switch (marqueeDTO.IsPeriod)
            {
                case true:
                    marquee.IsPeriod = true;
                    break;
                case false:
                    marquee.IsPeriod = false;
                    marquee.EndDate = null;
                    break;
                
                    default:
                    break;
            }


            marquee.UpdatedAt = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarqueeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(marquee);
           

        }

        // PUT: api/marquees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutMarquee(int id, Marquee marquee)
        {
            if (id != marquee.ID)
            {
                return BadRequest();
            }

            marquee.UpdatedAt = DateTime.Now;
            _context.Entry(marquee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarqueeExists(id))
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
                id = marquee.ID,
                Content = marquee.Content,
                Language = marquee.Language,
                Sort = marquee.Sort,
                StartDate = marquee.StartDate,
                EndDate = marquee.EndDate,
                isPeriod = marquee.IsPeriod,
            });
        }

        // POST: api/marquees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Marquee>> PostMarquee(Marquee marquee)
        {

            _context.Marquee.Add(marquee);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = marquee.ID,
                Content = marquee.Content,
                Language = marquee.Language,
                Sort = marquee.Sort,
                StartDate = marquee.StartDate,
                EndDate = marquee.EndDate,
                isPeriod = marquee.IsPeriod,
            });
        }

        // DELETE: api/marquees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarquee(int id)
        {
            var marquee = await _context.Marquee.FindAsync(id);
            if (marquee == null)
            {
                return NotFound();
            }

            _context.Marquee.Remove(marquee);
            await _context.SaveChangesAsync();

            return Ok(new{ });
        }

        private bool MarqueeExists(int id)
        {
            return (_context.Marquee?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        [HttpPost("upload"), Authorize]
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
