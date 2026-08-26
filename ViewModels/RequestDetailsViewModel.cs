using StudentServiceRequestSystem.Models;

namespace StudentServiceRequestSystem.ViewModels;

public class RequestDetailsViewModel
{
    public int Id { get; set; }
    public RequestType RequestType { get; set; }
    public string Description { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? StaffRemarks { get; set; }

    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;

    public bool IsStaffViewer { get; set; }
}
