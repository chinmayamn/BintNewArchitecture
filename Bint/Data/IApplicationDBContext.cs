using Bint.Models;
using Microsoft.EntityFrameworkCore;

namespace Bint.Data
{
    public interface IApplicationDbContext
    {
        DbSet<CaptureDeviceData> CaptureDeviceData { get; set; }
        DbSet<RestrictedAccess> RestrictedAccess { get; set; }

        DbSet<ErrorLog> Log { get; set; }
        int SaveChanges();
    }
}
