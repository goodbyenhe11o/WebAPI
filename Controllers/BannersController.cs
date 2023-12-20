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
    public class BannersController : ControllerBase
    {
        private readonly MyDB _context;
        private readonly string _folder;
        public BannersController(MyDB context, IHostingEnvironment env)
        {
            _context = context;
            _folder = $@"{env.WebRootPath}\Banners";
        }

        // GET: api/Banners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Banner>>> GetBanner(string? language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                var banners = await _context.Banner.Where(x => x.Language == language).Select(x => new
                {
                    x.ID,
                    x.Language,
                    x.ImagePath,
                    x.Sort,

                }).ToListAsync();
                return Ok(banners);
            }
            else
            {
                var banners = await _context.Banner.Select(x => new
                {
                    x.ID,
                    x.Language,
                    x.ImagePath,
                    x.Sort,
                }).ToListAsync();

                return Ok(banners);
            }

        }

        // GET: api/Banners/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Banner>> GetBanner(int id)
        {
            if (_context.Banner == null)
            {
                return NotFound();
            }
            var banner = await _context.Banner.FindAsync(id);

            if (banner == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = banner.ID,
                language = banner.Language,
                imagepath = banner.ImagePath,
                sort = banner.Sort
            });
        }

        [HttpGet]
        [Route("copyall")]
        public async Task<IActionResult> GetAsyncAllLanguage(string fromLanguage, string toLanguage)
        {

            var copyFrom = _context.Banner.Where(x => x.Language == fromLanguage).ToList();
            var copyTo = _context.Banner.Where(x => x.Language == toLanguage).ToList();

            foreach (var item in copyTo)
            {
                var toBeCopied = await _context.Banner.FindAsync(item.ID);

                _context.Banner.Remove(toBeCopied);
                await _context.SaveChangesAsync();
            }

            if (copyFrom == null)
            {
                return NotFound();
            }

            foreach (var item in copyFrom)
            {
                Banner banner = new Banner();

                banner.Language = toLanguage ;
                banner.ImagePath = item.ImagePath;
                banner.Sort = item.Sort;
                _context.Banner.Add(banner);
                await _context.SaveChangesAsync();

            }


            var copied = _context.Banner.Where(x => x.Language == toLanguage).ToList();
            return Ok(copied);
        }

        [HttpGet]
        [Route("copynew")]
        public async Task<IActionResult> GetAsyncNewLanguage(int id, string language)
        {
            Banner banner = new Banner();
            var copyFrom = _context.Banner.FirstOrDefault(x => x.ID == id);

            banner.ImagePath = copyFrom.ImagePath;
            banner.Sort = copyFrom.Sort;
            banner.Language = language;
            banner.CreatedAt = DateTime.Now;

            try
            {
                _context.Banner.Add(banner);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BannerExists(id))
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
                id = banner.ID,
                language = banner.Language,
                imagepath = banner.ImagePath,
                sort = banner.Sort
            });
        }


        [HttpGet]
        [Route("copy")]
        public async Task<IActionResult> GetAsyncLanguage(int id, string language)
        {
            var banner = _context.Banner.FirstOrDefault(x => x.ID == id);
            var toUpdate = _context.Banner.FirstOrDefault(x => x.Language == language);
            if (banner == null)
            {
                return NotFound();
            }

            toUpdate.ImagePath = banner.ImagePath;
            toUpdate.Sort = banner.Sort;
            toUpdate.UpdatedAt = DateTime.Now;


            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BannerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(toUpdate);
        }


        [HttpPatch("{id}"), Authorize]
        public async Task<IActionResult> PatchAsync(int id, [FromForm] DTO.BannerDTO bannerDTO)
        {
            var banner = _context.Banner.FirstOrDefault(x => x.ID == id);

            if (bannerDTO.Sort != null)
            {
                banner.Sort = bannerDTO.Sort;
            }

            if (!string.IsNullOrEmpty(bannerDTO.Language))
            {
                banner.Language = bannerDTO.Language;
            }

            if(bannerDTO.Image != null)
            {
                // Process the uploaded image and save it
                var imageName = SaveImage(bannerDTO.Image);
                banner.ImagePath = imageName;
            }


            banner.UpdatedAt = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BannerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(banner);
        }


        // PUT: api/Banners/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutBanner(int id, [FromForm] Banner banner)
        {
            if (id != banner.ID)
            {
                return BadRequest();
            }

            if (banner.Image != null)
            {
                string contentType = banner.Image.ContentType;
                if (contentType != "image/jpeg" && contentType != "image/png" && contentType != "image/jpg")
                {
                    return BadRequest("Only JPG, PNG, and JPEG image types are allowed.");
                }
                else
                {
                    var filename = $@"{Path.GetFileNameWithoutExtension(banner.Image.FileName)}-{DateTime.Now:yyyyMMddHHmm}{Path.GetExtension(banner.Image.FileName)}";
                    var path = $@"{_folder}\{filename}";
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await banner.Image.CopyToAsync(stream);
                    }
                    banner.ImagePath = $@"banners/{banner.Image.FileName}";
                }
            }

            _context.Entry(banner).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BannerExists(id))
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
                id = banner.ID,
                language = banner.Language,
                imagepath = banner.ImagePath,
                sort = banner.Sort
            });
        }

        // POST: api/Banners
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Banner>> PostBanner([FromForm] Banner banner)
        {
            if (banner.Image != null)
            {
                string contentType = banner.Image.ContentType;
                if (contentType != "image/jpeg" && contentType != "image/png" && contentType != "image/jpg")
                {
                    return BadRequest("Only JPG, PNG, and JPEG image types are allowed.");
                }
                else
                {
                   var filename =  $@"{Path.GetFileNameWithoutExtension(banner.Image.FileName)}-{DateTime.Now:yyyyMMddHHmm}{Path.GetExtension(banner.Image.FileName)}";

                    var path = $@"{_folder}\{filename}";
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await banner.Image.CopyToAsync(stream);
                    }
                    banner.ImagePath = $@"banners/{filename}";
                }
            }

            banner.CreatedAt = DateTime.Now;
            _context.Banner.Add(banner);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = banner.ID,
                language = banner.Language,
                imagepath = banner.ImagePath,
                sort = banner.Sort
            });
        }

        // DELETE: api/Banners/5
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var banner = await _context.Banner.FindAsync(id);
            if (banner == null)
            {
                return NotFound();
            }

            _context.Banner.Remove(banner);
            await _context.SaveChangesAsync();

            return Ok(new { });
        }

        private bool BannerExists(int id)
        {
            return (_context.Banner?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        private string SaveImage(IFormFile file)
        {
            var filename = $@"{Path.GetFileNameWithoutExtension(file.FileName)}-{DateTime.Now:yyyyMMddHHmm}{Path.GetExtension(file.FileName)}";
            var path = $@"{_folder}\{filename}";
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var newPath = $@"banners/{filename}";
           
            return newPath;
        }
    }
}

