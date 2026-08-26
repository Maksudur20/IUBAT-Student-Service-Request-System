using System.ComponentModel.DataAnnotations;

namespace StudentServiceRequestSystem.Models;

public enum RequestType
{
    [Display(Name = "ID Card Replacement")]
    IDCardReplacement = 1,

    [Display(Name = "Transcript Request")]
    TranscriptRequest = 2,

    [Display(Name = "Certificate Request")]
    CertificateRequest = 3
}

public static class RequestTypeExtensions
{
    public static string ToFriendlyName(this RequestType requestType)
    {
        return requestType switch
        {
            RequestType.IDCardReplacement => "ID Card Replacement",
            RequestType.TranscriptRequest => "Transcript Request",
            RequestType.CertificateRequest => "Certificate Request",
            _ => requestType.ToString()
        };
    }
}
