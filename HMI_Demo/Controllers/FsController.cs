using Microsoft.AspNetCore.Mvc;

namespace HMI_Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FsController> _logger;

        public FsController(IWebHostEnvironment environment, ILogger<FsController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("FileUpload")]
        [RequestSizeLimit(50 * 1024 * 1024)] // 50MB max
        public async Task<IActionResult> FileUpload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Nessun file ricevuto");

                if (file.Length > 50 * 1024 * 1024)
                    return BadRequest("File troppo grande (max 50MB)");

                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".jpg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                    return BadRequest("Tipo file non supportato");

                // 2. Crea directory se non esiste
                var uploadsPath = Path.Combine(_environment.ContentRootPath, "Uploads");
                Directory.CreateDirectory(uploadsPath);

                // 3. Genera nome file univoco
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                // 4. Salva file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 5. Log e risposta
                _logger.LogInformation($"File salvato: {filePath}");
                return Ok(new
                {
                    OriginalName = file.FileName,
                    SavedName = uniqueFileName,
                    Size = file.Length,
                    Path = filePath
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio del file");
                return StatusCode(500, "Errore interno del server");
            }
        }

        [ApiController]
        [Route("api/test")]
        public class TestController : ControllerBase
        {
            [HttpGet]
            public IActionResult Ping() => Ok("Pong");
        }

    }
}
