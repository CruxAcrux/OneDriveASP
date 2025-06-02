    public interface IFolderRepository : IRepository<FolderEntity>
    {
        Task<FolderEntity?> GetByNameAndUserAsync(string name, string userId);
        Task<IEnumerable<FolderEntity>> GetAllByUserAsync(string userId);
    }