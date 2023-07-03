using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace snapcrateBackend.Model
{
    public class SharedFolders
    {
        [Key]
        public int Id { get; set; }
        public FolderModel Folder { get; set; }
        public IdentityUser? User { get; set; }
        public Boolean EnableEditing { get; set; }
    }
}
