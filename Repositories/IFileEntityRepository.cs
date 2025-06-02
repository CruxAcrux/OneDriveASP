public interface IFileEntityRepository : IRepository<FileEntity>
{
    Task<IEnumerable<FileEntity>> GetAllByFolderAndUserAsync(Guid folderId, string userId);
    Task<FileEntity?> GetByNameAndFolderAsync(string name, Guid folderId, string userId);
    Task<IEnumerable<FileEntity>> GetAllByUserAsync(string userId); // New method
}