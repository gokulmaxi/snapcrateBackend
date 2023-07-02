using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using snapcrateBackend.Model;


namespace snapcrateBackend.Auth
{
    public class SnapCrateDbContext : IdentityDbContext<IdentityUser>
    {
        public SnapCrateDbContext(DbContextOptions<SnapCrateDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<snapcrateBackend.Model.FolderModel>? FolderModel { get; set; }
    }
}