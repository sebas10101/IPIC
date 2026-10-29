using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IPIC.Application.Services;

public class QrTokenService : ITokenService
{
    private const string StudentClaim = "student_id";
    private const string PurposeClaim = "purpose";
    private const string QrPurpose = "qr";
    private const int QrLifetimeSeconds = 60;

    private readonly SymmetricSecurityKey _key;

    public QrTokenService(IConfiguration configuration)
    {
        var secret = configuration["Jwt:Key"]
            ?? "clave_secreta_super_larga_para_seguridad_123!";
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }

    public string GenerateQrToken(Guid studentId)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(StudentClaim, studentId.ToString()),
                new Claim(PurposeClaim, QrPurpose)
            }),
            Expires = DateTime.UtcNow.AddSeconds(QrLifetimeSeconds),
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }

    public Guid ValidateQrToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Guid.Empty;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(5)
            }, out _);

            // Evita que un token de login se use como si fuera un QR
            if (principal.FindFirst(PurposeClaim)?.Value != QrPurpose)
                return Guid.Empty;

            return Guid.TryParse(principal.FindFirst(StudentClaim)?.Value, out var id)
                ? id
                : Guid.Empty;
        }
        catch (Exception)
        {
            return Guid.Empty;
        }
    }
}