using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CorreoFei.Services.ErrorLog
{
    public class ErrorLog : IErrorLog
    {
        private readonly IWebHostEnvironment _webhostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ErrorLog(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _webhostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task ErrorLogAsync(string Mensaje)
        {
            try
            {
                string webRootPath = _webhostEnvironment.WebRootPath;

                string path = "";
                path = Path.Combine(webRootPath, "Log");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var feature = _httpContextAccessor.HttpContext.Features.Get<IHttpConnectionFeature>();
                string LocalIPAddr = feature?.LocalIpAddress?.ToString();

                using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "log.txt"), true))
                {
                    await outputFile.WriteLineAsync(Mensaje + " - " +
                        _httpContextAccessor.HttpContext.User.Identity.Name +
                        " - " + LocalIPAddr + " - " + DateTime.Now.ToString());
                }
            }
            catch
            {
                //No hace nada
            }
        }
    }
}
