using System.ComponentModel.DataAnnotations;

namespace StudentServiceRequestSystem.Models;

public enum RequestStatus
{
    [Display(Name = "Pending")]
    Pending = 1,

    [Display(Name = "Processing")]
    Processing = 2,

    [Display(Name = "Completed")]
    Completed = 3,

    [Display(Name = "Rejected")]
    Rejected = 4
}

public static class RequestStatusExtensions
{
    public static string ToBadgeClass(this RequestStatus status)
    {
        return status switch
        {
            RequestStatus.Pending => "bg-warning text-dark",
            RequestStatus.Processing => "bg-primary",
            RequestStatus.Completed => "bg-success",
            RequestStatus.Rejected => "bg-danger",
            _ => "bg-secondary"
        };
    }
}
