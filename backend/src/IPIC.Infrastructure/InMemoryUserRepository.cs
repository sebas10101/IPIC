using IPIC.Domain;
using IPIC.Domain.Entities;

namespace IPIC.Infrastructure.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new()
    {
        new User
        {
            Id = 1,
            StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            FullName = "Juan Pérez",
            Username = "juan_alumno", Password = "123", Role = "Alumno"
        },
        new User
        {
            Id = 2,
            FullName = "María López",
            Username = "maria_admin", Password = "123", Role = "Administrativo"
        },
        new User
        {
            Id = 3,
            FullName = "Carlos Ramírez",
            Username = "carlos_directivo", Password = "123", Role = "Directivo"
        },

        // Dispositivo lector de QR en la puerta del plantel
        new User
        {
            Id = 4,
            FullName = "Lector Puerta 1",
            Username = "lector_puerta1", Password = "123", Role = "ScannerDevice"
        },

        // Segundo alumno para probar el historial con más de una persona
        new User
        {
            Id = 5,
            StudentId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FullName = "Ana Martínez",
            Username = "ana_alumna", Password = "123", Role = "Alumno"
        }
    };

    public Task<User?> GetByUsernameAsync(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<User?> GetByStudentIdAsync(Guid studentId)
    {
        var user = _users.FirstOrDefault(u => u.StudentId == studentId && u.StudentId != Guid.Empty);
        return Task.FromResult(user);
    }
}