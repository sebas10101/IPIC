using IPIC.Application.DTOs;
using IPIC.Domain;
using IPIC.Domain.Entities;

namespace IPIC.Application.UseCases;

public class GetAttendanceHistoryUseCase
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IUserRepository _userRepository;

    public GetAttendanceHistoryUseCase(
        IAttendanceRepository attendanceRepository,
        IUserRepository userRepository)
    {
        _attendanceRepository = attendanceRepository;
        _userRepository = userRepository;
    }

    // Historial de un alumno. Sin fechas: últimos 30 días. "to" incluye todo ese día.
    public async Task<IReadOnlyList<AttendanceRecordDto>> GetByStudentAsync(
        Guid studentId, DateTime? from, DateTime? to)
    {
        var fromUtc = (from ?? DateTime.UtcNow.AddDays(-30)).Date;
        var toUtc = (to ?? DateTime.UtcNow).Date.AddDays(1);

        var records = await _attendanceRepository.GetByStudentAsync(studentId, fromUtc, toUtc);
        return await ToDtosAsync(records);
    }

    // Todos los movimientos de un día. Sin fecha: hoy.
    public async Task<IReadOnlyList<AttendanceRecordDto>> GetByDayAsync(DateTime? date)
    {
        var day = (date ?? DateTime.UtcNow).Date;

        var records = await _attendanceRepository.GetByDayAsync(day);
        return await ToDtosAsync(records);
    }

    private async Task<IReadOnlyList<AttendanceRecordDto>> ToDtosAsync(IReadOnlyList<Attendance> records)
    {
        var names = new Dictionary<Guid, string>();
        var result = new List<AttendanceRecordDto>();

        foreach (var record in records)
        {
            if (!names.TryGetValue(record.StudentId, out var name))
            {
                var user = await _userRepository.GetByStudentIdAsync(record.StudentId);
                name = user?.FullName ?? "Desconocido";
                names[record.StudentId] = name;
            }

            result.Add(new AttendanceRecordDto
            {
                StudentId = record.StudentId,
                StudentName = name,
                Type = record.Type.ToString(),
                ScanDateTime = record.ScanDateTime
            });
        }

        return result;
    }
}