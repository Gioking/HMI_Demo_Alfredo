using Microsoft.AspNetCore.Components.Forms;
using RestSharp;
using System.Net;

namespace HMI_Demo.Services
{
    public class FileUploadService
    {
        private readonly ILogger<FileUploadService> _logger;

        public FileUploadService(HttpClient httpClient, ILogger<FileUploadService> logger)
        {
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> UploadFile(IBrowserFile file)
        {
            try
            {
                using var stream = file.OpenReadStream(50 * 1024 * 1024); // max 50MB
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;

                var client = new RestClient("https://localhost:7079/"); 
                var request = new RestRequest("api/Fs/FileUpload", Method.Post);
                request.AlwaysMultipartFormData = true;

                request.AddFile("file", ms.ToArray(), file.Name, file.ContentType);

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    _logger.LogInformation($"Upload riuscito: {response.Content}");
                    return (true, "File caricato con successo");
                }

                var errorMessage = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => "Richiesta malformata",
                    HttpStatusCode.RequestEntityTooLarge => "File troppo grande (max 50MB)",
                    _ => $"Errore server: {response.StatusCode}"
                };

                _logger.LogWarning($"Upload fallito: {errorMessage}");
                return (false, errorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'upload con RestSharp");
                return (false, $"Errore: {ex.Message}");
            }
        }

    }
}
