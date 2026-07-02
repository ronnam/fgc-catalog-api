using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Infrastructure.Persistence;
using Fgc.Catalog.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fgc.Catalog.Infrastructure.Repositories
{

    public class GameRepository : IGameRepository
    {
        private readonly CatalogDbContext _context;

        public GameRepository(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Game game)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByTitleAsync(string title)
        {
            return await _context.Games.AnyAsync(g => g.Title == title);
        }

        public async Task<List<Game>> GetAllAsync()
        {
            return await _context.Games.ToListAsync();
        }

        public async Task<Game?> GetByIdAsync(Guid id)
        {
            return await _context.Games.FindAsync(id);
        }
         
        public async Task UpdateAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }
    }
}
