using Fgc.Catalog.Domain.Entities;

namespace Fgc.Catalog.Application.Interfaces
{
    public interface IGameRepository
    {
        Task AddAsync(Game game);
        Task<bool> ExistsByTitleAsync(string title);
        Task<List<Game>> GetAllAsync();
        Task<Game?> GetByIdAsync(Guid id);
        Task UpdateAsync(Game game);
        Task DeleteAsync(Game game);
    }
}
