using Fgc.Catalog.Domain.Entities;

namespace Fgc.Catalog.Application.Interfaces
{
    public interface IUserLibraryRepository
    {
        Task AddAsync(UserLibrary userLibrary);
        Task<bool> ExistsAsync(Guid userId, Guid gameId);
    }
}