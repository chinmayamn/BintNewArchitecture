﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bint.Models;
using Microsoft.Azure.KeyVault.Models;

namespace Bint.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>,IApplicationDBContext
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

        public virtual DbSet<CaptureDeviceData> _captureDeviceData { get; set; }
        public virtual DbSet<RestrictedAccess> _restrictedAccess { get; set; }
        public virtual DbSet<ErrorLog> Log { get; set; }
        public virtual DbSet<Doc> Doc { get; set; }
        public virtual DbSet<RegId> regId { get; set; }
        public virtual DbSet<ActivityLog> activitylog { get; set; }
        public virtual DbSet<TransferUSD> transferusd { get; set; }
        public virtual DbSet<DepositWithdraw> depositwithdraw { get; set; }
    }
}
