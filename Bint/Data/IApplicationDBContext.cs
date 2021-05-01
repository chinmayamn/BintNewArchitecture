using Bint.Models;
using Microsoft.EntityFrameworkCore;

namespace Bint.Data
{
    public interface IApplicationDBContext
    {
        DbSet<CaptureDeviceData> CaptureDeviceData { get; set; }
        DbSet<RestrictedAccess> RestrictedAccess { get; set; }

        DbSet<ErrorLog> Log { get; set; }
        int SaveChanges();
    }
}
