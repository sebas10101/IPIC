namespace IPIC.Application.Services;

public interface ITokenService
{
    /// <summary>Genera el token que se convierte en el código QR del alumno (dura 60 segundos).</summary>
    string GenerateQrToken(Guid studentId);

    /// <summary>Valida el token del QR y devuelve el StudentId, o Guid.Empty si es inválido o expiró.</summary>
    Guid ValidateQrToken(string token);
}