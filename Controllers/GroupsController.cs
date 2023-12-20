using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igs_backend.Data;
using igs_backend.Models;
using Microsoft.CodeAnalysis;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using igs_backend.DTO;
using Microsoft.AspNetCore.Authorization;

namespace igs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly MyDB _context;
        private readonly string _folder;


        public GroupsController(MyDB context, IHostingEnvironment env)
        {
            _context = context;
            // 把上傳目錄設為：wwwroot\UploadFolder
            _folder = $@"{env.WebRootPath}\Groups";
        }

        // GET: api/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroup(string? language)
        {

            if (!string.IsNullOrEmpty(language))
            {
                var groups = await _context.Group.Where(x => x.Language == language).OrderBy(x=>x.Sort).Select(x => new
                {
                    x.ID,
                    x.Name,
                    x.ImagePath,
                    x.Language,
                    x.DefaultTab,
                    x.grp,
                    x.Sort
                }).ToListAsync();

                return Ok(groups);
            }
            else
            {
                var groups = await _context.Group.OrderBy(x=>x.Sort).Select(x => new
                {
                    x.ID,
                    x.Name,
                    x.ImagePath,
                    x.Language,
                    x.DefaultTab,
                    x.grp,
                    x.Sort
                }).ToListAsync();

                return Ok(groups);
            }
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(int id)
        {
            if (_context.Group == null)
            {
                return NotFound();
            }
            var @group = await _context.Group.FindAsync(id);

            if (@group == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = group.ID,
                name = @group.Name,
                imagepath = @group.ImagePath,
                language = @group.Language,
                defaulttab = @group.DefaultTab,
                grp = @group.grp,
                sort = @group.Sort,
            });
        }

        [HttpGet]
        [Route("copyall")]
        public async Task<IActionResult> GetAsyncAllLanguage(string fromLanguage, string toLanguage)
        {

            var copyFrom = _context.Group.Where(x => x.Language == fromLanguage).ToList();
            var copyTo = _context.Group.Where(x => x.Language == toLanguage).ToList();

            foreach (var item in copyTo)
            {
                var toBeCopied = await _context.Group.FindAsync(item.ID);

                _context.Group.Remove(toBeCopied);
                await _context.SaveChangesAsync();
            }


            if (copyFrom == null)
            {
                return NotFound();
            }

            foreach (var item in copyFrom)
            {
                Group group = new Group();
                
                group.Name = item.Name;
                group.ImagePath = item.ImagePath;
                group.Language = toLanguage;
                group.DefaultTab = item.DefaultTab;
                group.grp = item.grp;
                group.Sort = item.Sort;
                _context.Group.Add(group);
                await _context.SaveChangesAsync();

            }

            var copied = _context.Group.Where(x => x.Language == toLanguage).ToList();

            return Ok(copied);
        }




        [HttpGet]
        [Route("copynew")]
        public async Task<IActionResult> GetAsyncNewLanguage(int id, string language)
        {
            Group group = new Group();
            var copyFrom = _context.Group.FirstOrDefault(x => x.ID == id);

            if(copyFrom == null)
            {
                return NotFound();
            }
          
                group.Name = copyFrom.Name;
            group.ImagePath = copyFrom.ImagePath;
            group.Language = language;
            group.DefaultTab = copyFrom.DefaultTab;
            group.grp = copyFrom.grp;
            group.Sort = copyFrom.Sort;
            group.CreatedAt = DateTime.Now;


            try
            {
                _context.Group.Add(group);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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
                id = group.ID,
                name = @group.Name,
                imagepath = @group.ImagePath,
                language = @group.Language,
                defaulttab = @group.DefaultTab,
                grp = @group.grp,
                sort = @group.Sort,
            });
        }


        [HttpGet]
        [Route("copy")]
        public async Task<IActionResult> GetAsyncLanguage(int id, string language)
        {
            var group = _context.Group.FirstOrDefault(x => x.ID == id);
            var toUpdate = _context.Group.FirstOrDefault(x => x.Language == language);
            if (group == null)
            {
                return NotFound();
            }
            
            toUpdate.Name = group.Name;
            toUpdate.ImagePath = group.ImagePath;
            toUpdate.DefaultTab = group.DefaultTab;
            toUpdate.grp = group.grp;
            toUpdate.Sort = group.Sort;
            toUpdate.CreatedAt = DateTime.Now;


            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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
        public async Task<IActionResult> PatchAsync(int id, [FromForm] DTO.GroupDTO groupDTO)
        {
            var group = _context.Group.FirstOrDefault(x => x.ID == id);
            if (group == null)
            {
                return NotFound();
            }

            // Update other properties if needed

            if (!string.IsNullOrEmpty(groupDTO.Name))
            {
                group.Name = groupDTO.Name;
            }

            if (!string.IsNullOrEmpty(groupDTO.Language))
            {
                group.Language = groupDTO.Language;
            }

            if (groupDTO.grp != null)
            {
                group.grp = groupDTO.grp;
            }

            if (groupDTO.DefaultTab != null)
            {
               group.DefaultTab = groupDTO.DefaultTab;
            }

            if (groupDTO.Sort != null)
            {
                group.Sort = groupDTO.Sort;
            }

            

            if (groupDTO.Image != null)
            {
                // Process the uploaded image and save it
                var imageName = SaveImage(groupDTO.Image);
                group.ImagePath = imageName;
            }


            group.UpdatedAt = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(group);
        }

        // PUT: api/Groups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutGroup(int id,[FromForm] Group @group)
        {
            if (id != @group.ID)
            {
                return BadRequest();
            }

        
            if (group.Image!= null)
            {
                string contentType = group.Image.ContentType;
                if (contentType != "image/svg+xml")
                {
                    return BadRequest("Only SVG is allowed.");
                }
                else {
                    var filename = $@"{Path.GetFileNameWithoutExtension(group.Image.FileName)}-{DateTime.Now:yyyyMMddHHmm}{Path.GetExtension(group.Image.FileName)}";
                    var path = $@"{_folder}\{group.Image.FileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await group.Image.CopyToAsync(stream);
                }
                group.ImagePath = $@"groups/{group.Image.FileName}";
                }
            }


            group.UpdatedAt = DateTime.Now;
            _context.Entry(@group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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
                id = group.ID,
                name = @group.Name,
                imagepath = @group.ImagePath,
                language = @group.Language,
                defaulttab = @group.DefaultTab,
                grp=@group.grp,
                sort = group.Sort
            });
        }




        // POST: api/Groups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Group>> PostGroup([FromForm]Group @group)
        {
            if (_context.Group == null)
            {
                return Problem("Entity set 'MyDB.Group'  is null.");
            }

            if (group.Image != null)
            {
                string contentType = group.Image.ContentType;
                if (contentType != "image/svg+xml")
                {
                    return BadRequest("Only SVG is allowed.");
                }
                else
                {
                    var filename = $@"{Path.GetFileNameWithoutExtension(group.Image.FileName)}-{DateTime.Now:yyyyMMddHHmm}{Path.GetExtension(group.Image.FileName)}";
                    var path = $@"{_folder}\{filename}";
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await group.Image.CopyToAsync(stream);
                    }
                    group.ImagePath = $@"groups/{filename}";
                }
            }

            group.CreatedAt = DateTime.Now;
            _context.Group.Add(@group);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = group.ID,
                name = @group.Name,
                imagepath = @group.ImagePath,
                language = @group.Language,
                defaulttab = @group.DefaultTab,
                grp = group.grp
            });
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            if (_context.Group == null)
            {
                return NotFound();
            }
            var @group = await _context.Group.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }

            _context.Group.Remove(@group);
            await _context.SaveChangesAsync();

            return Ok(new{ });
        }

        private bool GroupExists(int id)
        {
            return (_context.Group?.Any(e => e.ID == id)).GetValueOrDefault();
        }
        private string SaveImage(IFormFile file)
        {
            var filename = $@"{Path.GetFileNameWithoutExtension(file.FileName)}-{DateTime.Now:yyyyMMddHHmm}{Path.GetExtension(file.FileName)}";
            var path = $@"{_folder}\{filename}";
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var newPath = $@"groups/{filename}";

            return newPath;
        }
    }
}
