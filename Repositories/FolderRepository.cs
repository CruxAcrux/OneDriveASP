using Microsoft.EntityFrameworkCore;

public class FolderRepository : Repository<FolderEntity>, IFolderRepository
{
    public FolderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<FolderEntity?> GetByNameAndUserAsync(string name, string userId)
    {
        return await _context.FolderEntities
            .FirstOrDefaultAsync(f => f.Name == name && f.UserId == userId);
    }

    public async Task<IEnumerable<FolderEntity>> GetAllByUserAsync(string userId)
    {
        return await _context.FolderEntities
            .Where(f => f.UserId == userId)
            .ToListAsync();
    }

}