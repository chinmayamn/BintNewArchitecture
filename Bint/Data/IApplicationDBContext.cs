using Bint.Models;
using Microsoft.EntityFrameworkCore;

namespace Bint.Data
{
    public interface IApplicationDBContext
    {
        DbSet<CaptureDeviceData> _captureDeviceData { get; set; }
        DbSet<RestrictedAccess> _restrictedAccess { get; set; }

        DbSet<ErrorLog> Log { get; set; }
        int SaveChanges();
    }
}
