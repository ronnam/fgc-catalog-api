using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Fgc.Catalog.Infrastructure.Persistence;

namespace Fgc.Catalog.Infrastructure.Repositories
{
    public class UserLibraryRepository(CatalogDbContext context)
        : IUserLibraryRepository
    {

        public async Task AddAsync(UserLibrary userLibrary)
        {
            context.UserLibraries.Add(userLibrary);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid gameId)
        {
            return await context.UserLibraries
                .AnyAsync(u => u.UserId == userId && u.GameId == gameId);
        }
    }
}