using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace webserver.Pages.Manage;
[Authorize]
public class Overview : PageModel {

    public Overview() {

    }

    public void OnGet() {
    }
}