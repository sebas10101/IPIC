using IPIC.Domain.Entities;

namespace IPIC.Domain;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByStudentIdAsync(Guid studentId);
}