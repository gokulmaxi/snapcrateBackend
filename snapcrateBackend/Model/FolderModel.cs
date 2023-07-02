using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace snapcrateBackend.Model
{
    public class FolderModel
    {
        [Key]
        public int Id { get; set; }
        public String Name { get; set; }
        [AllowNull]
        public IdentityUser? User{ get; set; }

    }
}
