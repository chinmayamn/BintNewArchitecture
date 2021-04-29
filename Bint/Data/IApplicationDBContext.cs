using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bint.Models;
using Microsoft.AspNetCore.Mvc;
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
