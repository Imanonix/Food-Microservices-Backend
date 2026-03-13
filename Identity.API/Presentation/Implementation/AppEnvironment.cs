using Application.services;

namespace Presentation.Implementation
{
    public class AppEnvironment : IAppEnvironment
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AppEnvironment(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public string ContentRootPath => _webHostEnvironment.ContentRootPath;

        public string WebRootPath => _webHostEnvironment.WebRootPath;

    }
}
