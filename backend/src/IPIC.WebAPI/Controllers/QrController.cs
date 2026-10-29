using IPIC.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPIC.WebAPI.Controllers;

[ApiController]
[Route("api/v1/qr")]
[Authorize(Roles = "Alumno")]
public class QrController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public QrController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    // La app móvil llama a este endpoint y convierte el "qrToken" en un código QR.
    [HttpGet]
    public IActionResult GetMyQr()
    {
        var claim = User.FindFirst("student_id")?.Value;

        if (!Guid.TryParse(claim, out var studentId) || studentId == Guid.Empty)
        {
            return Unauthorized(new { message = "El usuario no tiene un alumno asociado." });
        }

        var qrToken = _tokenService.GenerateQrToken(studentId);
        return Ok(new { qrToken, expiresInSeconds = 60 });
    }
}