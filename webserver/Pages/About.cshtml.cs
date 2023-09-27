using Microsoft.AspNetCore.Mvc.RazorPages;

namespace webserver.Pages {
    public class About : PageModel {
        private readonly ILogger<About> _logger;

        public About(ILogger<About> logger) {
            _logger = logger;
        }

        public void OnGet() {
        }
    }
}