using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igs_backend.Data;
using igs_backend.Models;
using Microsoft.CodeAnalysis;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Authorization;

namespace igs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly MyDB _context;
        private readonly string _folder;
        private readonly string _location;
        public NewsController(MyDB context, IHostingEnvironment env)
        {
            _context = context;
            _folder = $@"{env.WebRootPath}\News";
            _location = $@"/News/";
        }

        // GET: api/News
        [HttpGet]
        public async Task<ActionResult<IEnumerable<News>>> GetNews(string? language)
        {
            if (!string.IsNullOrEmpty(language))
            {

                var news = await _context.News.Where(x => x.Language == language).Select(x => new
                {
                    x.ID,
                    x.Title,
                    x.Content,
                    x.ImagePath,
                    x.Language,
                    x.Sort
                }).ToListAsync();

                return Ok(news);
            }
            else
            {
                var news = await _context.News.Select(x => new
                {
                    x.ID,
                    x.Title,
                    x.Content,
                    x.ImagePath,
                    x.Language,
                    x.Sort
                }).ToListAsync();


                return Ok(news);
            }
        }

        // GET: api/News/5
        [HttpGet("{id}")]
        public async Task<ActionResult<News>> GetNews(int id)
        {
            if (_context.News == null)
            {
                return NotFound();
            }
            var news = await _context.News.FindAsync(id);

            if (news == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = news.ID,
                Title = news.Title,
                Content = news.Content,
                Language = news.Language,
                imagepath = news.ImagePath,
                Sort = news.Sort,
            });
        }

        [HttpGet]
        [Route("copyall")]
        public async Task<IActionResult> GetAsyncAllLanguage(string fromLanguage, string toLanguage)
        {
            
            var copyFrom = _context.News.Where(x => x.Language == fromLanguage).ToList();
            var copyTo = _context.News.Where(x => x.Language == toLanguage).ToList();

            foreach (var item in copyTo)
            {
                var toBeCopied = await _context.News.FindAsync(item.ID);

                _context.News.Remove(toBeCopied);
                await _context.SaveChangesAsync();
            }

            if (copyFrom == null)
            {
                return NotFound();
            }

            foreach (var item in copyFrom)
            {
                News news = new News();
                news.Title = item.Title;
                news.Content = item.Content;
                news.Language = toLanguage;
                news.ImagePath = item.ImagePath;
                news.Sort = item.Sort;
                _context.News.Add(news);
                await _context.SaveChangesAsync();
            }

            var copied = _context.News.Where(x => x.Language == toLanguage).ToList();
            return Ok(copied);
        }


        [HttpGet]
        [Route("copynew")]
        public async Task<IActionResult> GetAsyncNewLanguage(int id, string language)
        {
            News news = new News();
            var copyFrom = _context.News.FirstOrDefault(x => x.ID == id);

            if (copyFrom == null)
            {
                return NotFound();
            }

            news.Title = copyFrom.Title;
            news.Content = copyFrom.Content;
            news.Language = language;
            news.ImagePath = copyFrom.ImagePath;
            news.Sort = copyFrom.Sort;


            try
            {
                _context.News.Add(news);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
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
                id = news.ID,
                Title = news.Title,
                Content = news.Content,
                Language = news.Language,
                imagepath = news.ImagePath,
                Sort = news.Sort,
            });
        }


        [HttpGet]
        [Route("copy")]
        public async Task<IActionResult> GetAsyncLanguage(int id, string language)
        {
            var news = _context.News.FirstOrDefault(x => x.ID == id);
            var toUpdate = _context.News.FirstOrDefault(x => x.Language == language);
            if (news == null)
            {
                return NotFound();
            }

            toUpdate.Title = news.Title;
            toUpdate.Content = news.Content;
            toUpdate.ImagePath = news.ImagePath;
            toUpdate.Sort = news.Sort;


            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
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
        public async Task<IActionResult> PatchAsync(int id, [FromForm] DTO.NewsDTO newsDTO)
        {
            var news = _context.News.FirstOrDefault(x => x.ID == id);
            if (news == null)
            {
                return NotFound();
            }

            // Update other properties if needed

            if (!string.IsNullOrEmpty(newsDTO.Title))
            {
                news.Title = newsDTO.Title;
            }
            else
            {
                news.Title = string.Empty;
            }

            if (!string.IsNullOrEmpty(newsDTO.Content))
            {
                news.Content = newsDTO.Content;
            }

            if (!string.IsNullOrEmpty(newsDTO.Language))
            {
                news.Language = newsDTO.Language;
            }

            if (newsDTO.Sort != null)
            {
                news.Sort = newsDTO.Sort;
            }



            if (newsDTO.Image != null)
            {
                // Process the uploaded image and save it
                var imageName = SaveImage(newsDTO.Image);
                news.ImagePath = imageName;
            }


            news.UpdatedAt = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(news);
        }

        // PUT: api/News/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutNews(int id, [FromForm] News news)
        {
            if (id != news.ID)
            {
                return BadRequest();
            }
            if (news.Image != null)
            {
                string contentType = news.Image.ContentType;
                if (contentType != "image/jpeg" && contentType != "image/png" && contentType != "image/jpg")
                {
                    return BadRequest("Only JPG, PNG, and JPEG image types are allowed.");
                }
                else
                {
                    var filename = $@"{Path.GetFileNameWithoutExtension(news.Image.FileName)}-{DateTime.Now:yyyyMMddHHmm}{Path.GetExtension(news.Image.FileName)}";
                    var path = $@"{_folder}\{filename}";
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await news.Image.CopyToAsync(stream);
                    }
                    news.ImagePath = $@"news/{filename}";
                }
            }

            news.UpdatedAt = DateTime.Now;
            _context.Entry(news).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
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
                id = news.ID,
                Title = news.Title,
                Content = news.Content,
                Language = news.Language,
                imagepath = news.ImagePath,
                Sort = news.Sort,
            });
        }

        // POST: api/News
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<News>> PostNews([FromForm] News news)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'MyDB.News'  is null.");
            }
            if (news.Image != null)
            {
                string contentType = news.Image.ContentType;
                if (contentType != "image/jpeg" && contentType != "image/png" && contentType != "image/jpg")
                {
                    return BadRequest("Only JPG, PNG, and JPEG image types are allowed.");
                }
                else
                {
                    var filename = $@"{Path.GetFileNameWithoutExtension(news.Image.FileName)}-{DateTime.Now:yyyyMMddHHmm}{Path.GetExtension(news.Image.FileName)}";
                    var path = $@"{_folder}\{filename}";
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await news.Image.CopyToAsync(stream);
                    }
                    news.ImagePath = $@"news/{filename}";
                }
            }


            news.CreatedAt = DateTime.Now;

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = news.ID,
                Title = news.Title,
                Content = news.Content,
                Language = news.Language,
                imagepath = news.ImagePath,
                Sort = news.Sort,
            });
        }

        // DELETE: api/News/5
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteNews(int id)
        {
            if (_context.News == null)
            {
                return NotFound();
            }
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            return Ok(new { });
        }

        private bool NewsExists(int id)
        {
            return (_context.News?.Any(e => e.ID == id)).GetValueOrDefault();
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
        private string SaveImage(IFormFile file)
        {
            var filename = $@"{file.FileName}{DateTime.Now:yyyyMMddHHmm}";
            var path = $@"{_folder}\{filename}";
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var newPath = $@"news/{filename}";

            return newPath;
        }
    }
}
