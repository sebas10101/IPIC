namespace IPIC.Domain.Entities;

public class User
{
    public int Id { get; set; }

    // Identificador del alumno que viaja dentro del QR (solo aplica a rol "Alumno")
    public Guid StudentId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // "Alumno", "Administrativo", "Directivo", "ScannerDevice"
    public string Role { get; set; } = string.Empty;
}