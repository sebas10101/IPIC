using IPIC.Domain.Entities;

namespace IPIC.Domain;

public interface IAttendanceRepository
{
    Task<Attendance?> GetLastOfDayAsync(Guid studentId, DateTime date);
    Task<IReadOnlyList<Attendance>> GetByStudentAsync(Guid studentId, DateTime fromUtc, DateTime toUtc);
    Task<IReadOnlyList<Attendance>> GetByDayAsync(DateTime date);
    Task AddAsync(Attendance attendance);
    Task SaveChangesAsync();
}