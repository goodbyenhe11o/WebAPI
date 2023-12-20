using Microsoft.AspNetCore.Mvc;
using igs_backend.Data;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace igs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoController : ControllerBase
    {
        private readonly MyDB _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _folder;
        private readonly string _location;
        private readonly string _jsonPath;
        private readonly string _json;


        public LogoController(MyDB context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _folder = $@"{environment.WebRootPath}\Images";
            _location = $@"/Images/";
            _jsonPath = $@"{environment.WebRootPath}\Utility\ThemeColor.json";
            _json = $@"/Utility/ThemeColor.json";
        }



        [HttpPost("upload"), Authorize]
        public IActionResult UploadImages(string filename, IFormFile Image)
        {
            var domain = HttpContext.Request.Host;

            var oldPath = Path.Combine(_folder, Image.FileName);
            // Check if a file with the new filename already exists
            CheckAndDeleteExistingFile(oldPath);

            string currentfile = Path.GetFileName(Image.FileName);
            string fileExtension = Path.GetExtension(Image.FileName);
            string newFileName = $"{filename}{fileExtension}";

            var filePath = Path.Combine(_folder, newFileName);

            // Save the uploaded file with the new filename
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                Image.CopyTo(fileStream);
            }

            var path = $@"{HttpContext.Request.Scheme}://" + domain + _location + newFileName;
            return Ok(new { newfilename = newFileName, path = path });

        }

        [HttpGet("color")]
        public IActionResult Color()
        {
            var domain = HttpContext.Request.Host;

            string fileContent = System.IO.File.ReadAllText(_jsonPath);
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(fileContent);
            JsonDocument jsonDocument = JsonDocument.Parse(fileContent);

            var path = $@"{HttpContext.Request.Scheme}://" + domain + _json;

            return Ok(jsonDocument);
        }

        [HttpPatch("color"), Authorize]
        public IActionResult ThemeColor([FromBody] JsonDocument json)
        {
            var domain = HttpContext.Request.Host;
            try
            {
                string jsonString = ConvertToJsonString(json);

                // Save the JSON string to a file
                System.IO.File.WriteAllText(_jsonPath, jsonString);
                var path = $@"{HttpContext.Request.Scheme}://" + domain + _json;
                var a = System.IO.File.ReadAllText(_jsonPath);
                return Ok(path);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the file: {ex.Message}");
            }
        }

        public static string ConvertToJsonString(JsonDocument jsonDocument)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
            {
                Indented = false
            });

            jsonDocument.WriteTo(writer);
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
        private void CheckAndDeleteExistingFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}

