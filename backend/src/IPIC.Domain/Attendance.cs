namespace IPIC.Domain.Entities;

public class Attendance
{
    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public AttendanceType Type { get; private set; }
    public DateTime ScanDateTime { get; private set; }

    public Attendance(Guid studentId, AttendanceType type)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        Type = type;
        ScanDateTime = DateTime.UtcNow;
    }
}