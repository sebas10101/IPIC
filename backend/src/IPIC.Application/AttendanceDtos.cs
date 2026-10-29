namespace IPIC.Application.DTOs;

public class ValidateAttendanceResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Type { get; set; }          // "Entry" o "Exit"
    public string? StudentName { get; set; }
    public DateTime? ScanDateTime { get; set; }
}

public class AttendanceRecordDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;   // "Entry" o "Exit"
    public DateTime ScanDateTime { get; set; }
}