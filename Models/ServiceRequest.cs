using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServiceRequestSystem.Models;

public class ServiceRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }

    [Required]
    public RequestType RequestType { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    [MaxLength(500)]
    public string? StaffRemarks { get; set; }

    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
