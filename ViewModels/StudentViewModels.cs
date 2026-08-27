using System.ComponentModel.DataAnnotations;
using StudentServiceRequestSystem.Models;

namespace StudentServiceRequestSystem.ViewModels;

public class CreateRequestViewModel
{
    [Required(ErrorMessage = "Please select a service request type.")]
    [Display(Name = "Request Type")]
    public RequestType? RequestType { get; set; }

    [Required(ErrorMessage = "Please provide a description of your request.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters.")]
    [Display(Name = "Request Description")]
    public string Description { get; set; } = string.Empty;
}

public class StudentDashboardViewModel
{
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string? UniversityId { get; set; }
    public string? Department { get; set; }

    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ProcessingRequests { get; set; }
    public int CompletedRequests { get; set; }
    public int RejectedRequests { get; set; }

    public List<StudentRequestItemViewModel> RecentRequests { get; set; } = new();
}

public class StudentRequestItemViewModel
{
    public int Id { get; set; }
    public RequestType RequestType { get; set; }
    public string Description { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? StaffRemarks { get; set; }
}
