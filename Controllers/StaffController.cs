using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentServiceRequestSystem.Data;
using StudentServiceRequestSystem.Models;
using StudentServiceRequestSystem.ViewModels;

namespace StudentServiceRequestSystem.Controllers;

[Authorize(Roles = nameof(UserRole.Staff))]
public class StaffController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StaffController> _logger;

    public StaffController(ApplicationDbContext context, ILogger<StaffController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var staffName = User.FindFirstValue(ClaimTypes.Name) ?? "Staff Admin";

        var total = await _context.ServiceRequests.CountAsync();
        var pending = await _context.ServiceRequests.CountAsync(r => r.Status == RequestStatus.Pending);
        var processing = await _context.ServiceRequests.CountAsync(r => r.Status == RequestStatus.Processing);
        var completed = await _context.ServiceRequests.CountAsync(r => r.Status == RequestStatus.Completed);
        var rejected = await _context.ServiceRequests.CountAsync(r => r.Status == RequestStatus.Rejected);

        var recentRequests = await _context.ServiceRequests
            .Include(r => r.User)
            .OrderByDescending(r => r.RequestDate)
            .Take(6)
            .Select(r => new StaffRequestItemViewModel
            {
                Id = r.Id,
                UserId = r.UserId,
                StudentName = r.User != null ? r.User.Name : "Unknown",
                StudentEmail = r.User != null ? r.User.Email : "Unknown",
                RequestType = r.RequestType,
                Description = r.Description,
                Status = r.Status,
                RequestDate = r.RequestDate,
                UpdatedAt = r.UpdatedAt,
                StaffRemarks = r.StaffRemarks
            })
            .ToListAsync();

        var viewModel = new StaffDashboardViewModel
        {
            StaffName = staffName,
            TotalRequests = total,
            PendingRequests = pending,
            ProcessingRequests = processing,
            CompletedRequests = completed,
            RejectedRequests = rejected,
            RecentRequests = recentRequests
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Requests(RequestStatus? statusFilter = null, RequestType? typeFilter = null, string? search = null)
    {
        var query = _context.ServiceRequests
            .Include(r => r.User)
            .AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(r => r.Status == statusFilter.Value);
        }

        if (typeFilter.HasValue)
        {
            query = query.Where(r => r.RequestType == typeFilter.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(r =>
                (r.User != null && r.User.Name.ToLower().Contains(searchLower)) ||
                (r.User != null && r.User.Email.ToLower().Contains(searchLower)) ||
                r.Description.ToLower().Contains(searchLower) ||
                r.Id.ToString() == searchLower);
        }

        var requests = await query
            .OrderByDescending(r => r.RequestDate)
            .Select(r => new StaffRequestItemViewModel
            {
                Id = r.Id,
                UserId = r.UserId,
                StudentName = r.User != null ? r.User.Name : "N/A",
                StudentEmail = r.User != null ? r.User.Email : "N/A",
                RequestType = r.RequestType,
                Description = r.Description,
                Status = r.Status,
                RequestDate = r.RequestDate,
                UpdatedAt = r.UpdatedAt,
                StaffRemarks = r.StaffRemarks
            })
            .ToListAsync();

        ViewData["StatusFilter"] = statusFilter;
        ViewData["TypeFilter"] = typeFilter;
        ViewData["SearchQuery"] = search;

        return View(requests);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var request = await _context.ServiceRequests
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            return NotFound();
        }

        var viewModel = new RequestDetailsViewModel
        {
            Id = request.Id,
            RequestType = request.RequestType,
            Description = request.Description,
            Status = request.Status,
            RequestDate = request.RequestDate,
            UpdatedAt = request.UpdatedAt,
            StaffRemarks = request.StaffRemarks,
            StudentId = request.UserId,
            StudentName = request.User?.Name ?? "Unknown",
            StudentEmail = request.User?.Email ?? "Unknown",
            IsStaffViewer = true
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> UpdateStatus(int id)
    {
        var request = await _context.ServiceRequests
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            return NotFound();
        }

        var viewModel = new UpdateRequestStatusViewModel
        {
            Id = request.Id,
            StudentName = request.User?.Name ?? "Unknown",
            StudentEmail = request.User?.Email ?? "Unknown",
            RequestType = request.RequestType,
            Description = request.Description,
            RequestDate = request.RequestDate,
            Status = request.Status,
            StaffRemarks = request.StaffRemarks
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, UpdateRequestStatusViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = await _context.ServiceRequests
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            return NotFound();
        }

        var oldStatus = request.Status;
        request.Status = model.Status;
        request.StaffRemarks = string.IsNullOrWhiteSpace(model.StaffRemarks) ? null : model.StaffRemarks.Trim();
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Staff {StaffEmail} updated request #{RequestId} status from {OldStatus} to {NewStatus}",
            User.FindFirstValue(ClaimTypes.Email), id, oldStatus, model.Status);

        TempData["SuccessMessage"] = $"Service Request #{id} status was successfully updated to '{model.Status}'.";
        return RedirectToAction(nameof(Details), new { id = request.Id });
    }
}
