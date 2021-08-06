using Bint.Models;
using Microsoft.EntityFrameworkCore;

namespace Bint.Data
{
    public interface IApplicationDbContext
    {
        DbSet<CaptureDeviceData> CaptureDeviceData { get; set; }
        DbSet<RestrictedAccess> RestrictedAccess { get; set; }
        DbSet<ErrorLog> Log { get; set; }
        DbSet<Doc> Doc { get; set; }
        DbSet<RegId> RegId { get; set; }
        DbSet<ActivityLog> ActivityLog { get; set; }
        DbSet<TransferUsd> TransferUsd { get; set; }
        DbSet<DepositWithdraw> DepositWithdraw { get; set; }
        
    }
}
