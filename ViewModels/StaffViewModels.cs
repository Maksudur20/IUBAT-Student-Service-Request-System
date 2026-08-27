using System.ComponentModel.DataAnnotations;
using StudentServiceRequestSystem.Models;

namespace StudentServiceRequestSystem.ViewModels;

public class StaffDashboardViewModel
{
    public string StaffName { get; set; } = string.Empty;

    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ProcessingRequests { get; set; }
    public int CompletedRequests { get; set; }
    public int RejectedRequests { get; set; }

    public List<StaffRequestItemViewModel> RecentRequests { get; set; } = new();
}

public class StaffRequestItemViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string? StudentUniversityId { get; set; }
    public string? StudentDepartment { get; set; }
    public RequestType RequestType { get; set; }
    public string Description { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? StaffRemarks { get; set; }
}

public class UpdateRequestStatusViewModel
{
    public int Id { get; set; }

    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string? StudentUniversityId { get; set; }
    public string? StudentDepartment { get; set; }

    public RequestType RequestType { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }

    [Required(ErrorMessage = "Please select a valid status.")]
    [Display(Name = "Request Status")]
    public RequestStatus Status { get; set; }

    [Display(Name = "Staff Remarks / Notes for Student")]
    [MaxLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
    public string? StaffRemarks { get; set; }
}
