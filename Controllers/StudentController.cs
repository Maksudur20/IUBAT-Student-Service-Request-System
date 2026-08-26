using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentServiceRequestSystem.Data;
using StudentServiceRequestSystem.Models;
using StudentServiceRequestSystem.ViewModels;

namespace StudentServiceRequestSystem.Controllers;

[Authorize(Roles = nameof(UserRole.Student))]
public class StudentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StudentController> _logger;

    public StudentController(ApplicationDbContext context, ILogger<StudentController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim, out int userId))
        {
            return userId;
        }
        throw new InvalidOperationException("User ID claim not found or invalid.");
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var studentId = GetCurrentUserId();
        var studentName = User.FindFirstValue(ClaimTypes.Name) ?? "Student";
        var studentEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";

        var requestsQuery = _context.ServiceRequests
            .Where(r => r.UserId == studentId);

        var total = await requestsQuery.CountAsync();
        var pending = await requestsQuery.CountAsync(r => r.Status == RequestStatus.Pending);
        var processing = await requestsQuery.CountAsync(r => r.Status == RequestStatus.Processing);
        var completed = await requestsQuery.CountAsync(r => r.Status == RequestStatus.Completed);
        var rejected = await requestsQuery.CountAsync(r => r.Status == RequestStatus.Rejected);

        var recentRequests = await requestsQuery
            .OrderByDescending(r => r.RequestDate)
            .Take(5)
            .Select(r => new StudentRequestItemViewModel
            {
                Id = r.Id,
                RequestType = r.RequestType,
                Description = r.Description,
                Status = r.Status,
                RequestDate = r.RequestDate,
                UpdatedAt = r.UpdatedAt,
                StaffRemarks = r.StaffRemarks
            })
            .ToListAsync();

        var viewModel = new StudentDashboardViewModel
        {
            StudentName = studentName,
            StudentEmail = studentEmail,
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
    public IActionResult CreateRequest()
    {
        return View(new CreateRequestViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRequest(CreateRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var studentId = GetCurrentUserId();

        var serviceRequest = new ServiceRequest
        {
            UserId = studentId,
            RequestType = model.RequestType!.Value,
            Description = model.Description.Trim(),
            Status = RequestStatus.Pending,
            RequestDate = DateTime.UtcNow
        };

        _context.ServiceRequests.Add(serviceRequest);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Student {StudentId} submitted request {RequestId} of type {Type}",
            studentId, serviceRequest.Id, serviceRequest.RequestType);

        TempData["SuccessMessage"] = $"Service request #{serviceRequest.Id} ({serviceRequest.RequestType.ToFriendlyName()}) submitted successfully.";
        return RedirectToAction(nameof(MyRequests));
    }

    [HttpGet]
    public async Task<IActionResult> MyRequests(RequestStatus? statusFilter = null)
    {
        var studentId = GetCurrentUserId();

        var query = _context.ServiceRequests
            .Where(r => r.UserId == studentId);

        if (statusFilter.HasValue)
        {
            query = query.Where(r => r.Status == statusFilter.Value);
        }

        var requests = await query
            .OrderByDescending(r => r.RequestDate)
            .Select(r => new StudentRequestItemViewModel
            {
                Id = r.Id,
                RequestType = r.RequestType,
                Description = r.Description,
                Status = r.Status,
                RequestDate = r.RequestDate,
                UpdatedAt = r.UpdatedAt,
                StaffRemarks = r.StaffRemarks
            })
            .ToListAsync();

        ViewData["CurrentFilter"] = statusFilter;
        return View(requests);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var studentId = GetCurrentUserId();

        var request = await _context.ServiceRequests
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            return NotFound();
        }

        // CRITICAL OWNERSHIP CHECK: Student can only view their own requests!
        if (request.UserId != studentId)
        {
            _logger.LogWarning("Security Alert: Student {StudentId} attempted to access unauthorized request {RequestId} owned by {OwnerId}",
                studentId, id, request.UserId);
            return Forbid();
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
            StudentName = request.User?.Name ?? "N/A",
            StudentEmail = request.User?.Email ?? "N/A",
            IsStaffViewer = false
        };

        return View(viewModel);
    }
}
