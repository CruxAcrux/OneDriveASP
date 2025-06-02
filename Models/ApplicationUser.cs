using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public virtual ICollection<FolderEntity> Folders { get; set; } = new List<FolderEntity>();
    public virtual ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();
}
