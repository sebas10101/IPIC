using IPIC.Application.DTOs;
using IPIC.Application.Services;
using IPIC.Domain;
using IPIC.Domain.Entities;

namespace IPIC.Application.UseCases;

public class ValidateAttendanceUseCase
{
    // Tiempo mínimo entre dos escaneos del mismo alumno.
    // Para pruebas rápidas puedes bajarlo a TimeSpan.FromSeconds(10).
    private static readonly TimeSpan MinTimeBetweenScans = TimeSpan.FromSeconds(10);

    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public ValidateAttendanceUseCase(
        IAttendanceRepository attendanceRepository,
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _attendanceRepository = attendanceRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<ValidateAttendanceResultDto> ExecuteAsync(ValidateQrRequestDto request)
    {
        // 1. Validar y extraer el StudentId del token QR
        var studentId = _tokenService.ValidateQrToken(request.QrToken);
        if (studentId == Guid.Empty)
        {
            return Denied("Acceso Denegado: Código QR inválido o expirado.");
        }

        // 2. Buscar el último movimiento del alumno hoy
        var now = DateTime.UtcNow;
        var last = await _attendanceRepository.GetLastOfDayAsync(studentId, now.Date);

        // 3. Evitar dobles escaneos seguidos
        if (last != null && now - last.ScanDateTime < MinTimeBetweenScans)
        {
            return Denied("Acceso Denegado: El alumno acaba de escanear. Espera unos minutos.");
        }

        // 4. Alternar: sin registros o último = Salida -> Entrada; si no -> Salida
        var type = (last == null || last.Type == AttendanceType.Exit)
            ? AttendanceType.Entry
            : AttendanceType.Exit;

        // 5. Registrar el movimiento
        var attendance = new Attendance(studentId, type);
        await _attendanceRepository.AddAsync(attendance);
        await _attendanceRepository.SaveChangesAsync();

        var student = await _userRepository.GetByStudentIdAsync(studentId);
        var name = student?.FullName ?? "Alumno";
        var label = type == AttendanceType.Entry ? "Entrada" : "Salida";

        return new ValidateAttendanceResultDto
        {
            Success = true,
            Message = $"¡Acceso Permitido: {label} registrada para {name}!",
            Type = type.ToString(),
            StudentName = name,
            ScanDateTime = attendance.ScanDateTime
        };
    }

    private static ValidateAttendanceResultDto Denied(string message) =>
        new() { Success = false, Message = message };
}