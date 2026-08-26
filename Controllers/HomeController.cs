using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StudentServiceRequestSystem.Models;

namespace StudentServiceRequestSystem.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == nameof(UserRole.Staff))
            {
                return RedirectToAction("Dashboard", "Staff");
            }
            return RedirectToAction("Dashboard", "Student");
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
