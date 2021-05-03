using Bint.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bint.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>,IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public virtual DbSet<CaptureDeviceData> CaptureDeviceData{ get; set; }
        public virtual DbSet<RestrictedAccess> RestrictedAccess { get; set; }
        public virtual DbSet<ErrorLog> Log { get; set; }
        public virtual DbSet<Doc> Doc { get; set; }
        public virtual DbSet<RegId> RegId { get; set; }
        public virtual DbSet<ActivityLog> ActivityLog { get; set; }
        public virtual DbSet<TransferUsd> TransferUsd { get; set; }
        public virtual DbSet<DepositWithdraw> DepositWithdraw { get; set; }
    }
}
