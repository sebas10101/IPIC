using IPIC.Application.DTOs;
using IPIC.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPIC.WebAPI.Controllers;

[ApiController]
[Route("api/v1/attendance")]
public class AttendanceController : ControllerBase
{
    private readonly ValidateAttendanceUseCase _validateAttendanceUseCase;
    private readonly GetAttendanceHistoryUseCase _historyUseCase;

    public AttendanceController(
        ValidateAttendanceUseCase validateAttendanceUseCase,
        GetAttendanceHistoryUseCase historyUseCase)
    {
        _validateAttendanceUseCase = validateAttendanceUseCase;
        _historyUseCase = historyUseCase;
    }

    // Escaneo del QR: registra Entrada o Salida
    [HttpPost("validate")]
    [Authorize(Roles = "ScannerDevice,Administrativo,Directivo")]
    public async Task<IActionResult> ValidateQr([FromBody] ValidateQrRequestDto request)
    {
        try
        {
            var result = await _validateAttendanceUseCase.ExecuteAsync(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    // El alumno consulta su propio historial
    [HttpGet("my-history")]
    [Authorize(Roles = "Alumno")]
    public async Task<IActionResult> GetMyHistory([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var claim = User.FindFirst("student_id")?.Value;

        if (!Guid.TryParse(claim, out var studentId) || studentId == Guid.Empty)
        {
            return Unauthorized(new { message = "El usuario no tiene un alumno asociado." });
        }

        var history = await _historyUseCase.GetByStudentAsync(studentId, from, to);
        return Ok(history);
    }

    // Personal: historial de cualquier alumno
    [HttpGet("student/{studentId:guid}")]
    [Authorize(Roles = "Administrativo,Directivo")]
    public async Task<IActionResult> GetStudentHistory(
        Guid studentId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var history = await _historyUseCase.GetByStudentAsync(studentId, from, to);
        return Ok(history);
    }

    // Personal: todos los movimientos de un día (sin fecha = hoy)
    [HttpGet("by-day")]
    [Authorize(Roles = "Administrativo,Directivo")]
    public async Task<IActionResult> GetByDay([FromQuery] DateTime? date)
    {
        var history = await _historyUseCase.GetByDayAsync(date);
        return Ok(history);
    }
}