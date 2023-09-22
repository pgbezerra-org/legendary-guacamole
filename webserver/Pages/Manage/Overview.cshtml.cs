using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace webserver.Pages.Manage {
    [Authorize]
    public class Overview : PageModel {
        private readonly ILogger<Overview> _logger;

        public Overview(ILogger<Overview> logger) {
            _logger = logger;
        }

        public void OnGet() {
        }
    }
}