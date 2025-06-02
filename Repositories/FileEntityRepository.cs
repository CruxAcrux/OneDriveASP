using Microsoft.EntityFrameworkCore;

    public class FileEntityRepository : Repository<FileEntity>, IFileEntityRepository
    {
        public FileEntityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FileEntity>> GetAllByFolderAndUserAsync(Guid folderId, string userId)
        {
            return await _context.FileEntities
                .Where(f => f.FolderId == folderId && f.UserId == userId)
                .ToListAsync();
        }

        public async Task<FileEntity?> GetByNameAndFolderAsync(string name, Guid folderId, string userId)
        {
            return await _context.FileEntities
                .FirstOrDefaultAsync(f => f.Name == name && f.FolderId == folderId && f.UserId == userId);
        }

        public async Task<IEnumerable<FileEntity>> GetAllByUserAsync(string userId)
        {
            return await _context.FileEntities
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }
    }