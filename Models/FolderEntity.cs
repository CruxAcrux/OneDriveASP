    public class FolderEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public virtual ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();
        public DateTime CreatedAt { get; set; }
    }