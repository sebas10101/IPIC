using IPIC.Domain;
using IPIC.Domain.Entities;

namespace IPIC.Infrastructure.Repositories;

public class InMemoryAttendanceRepository : IAttendanceRepository
{
    private readonly List<Attendance> _records = new();
    private readonly object _lock = new();

    public Task<Attendance?> GetLastOfDayAsync(Guid studentId, DateTime date)
    {
        lock (_lock)
        {
            var last = _records
                .Where(a => a.StudentId == studentId && a.ScanDateTime.Date == date.Date)
                .OrderByDescending(a => a.ScanDateTime)
                .FirstOrDefault();

            return Task.FromResult(last);
        }
    }

    public Task<IReadOnlyList<Attendance>> GetByStudentAsync(Guid studentId, DateTime fromUtc, DateTime toUtc)
    {
        lock (_lock)
        {
            IReadOnlyList<Attendance> result = _records
                .Where(a => a.StudentId == studentId
                         && a.ScanDateTime >= fromUtc
                         && a.ScanDateTime < toUtc)
                .OrderBy(a => a.ScanDateTime)
                .ToList();

            return Task.FromResult(result);
        }
    }

    public Task<IReadOnlyList<Attendance>> GetByDayAsync(DateTime date)
    {
        lock (_lock)
        {
            IReadOnlyList<Attendance> result = _records
                .Where(a => a.ScanDateTime.Date == date.Date)
                .OrderBy(a => a.ScanDateTime)
                .ToList();

            return Task.FromResult(result);
        }
    }

    public Task AddAsync(Attendance attendance)
    {
        lock (_lock)
        {
            _records.Add(attendance);
        }
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}