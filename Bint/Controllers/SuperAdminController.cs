using Bint.Data;
using Bint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Bint.Controllers
{

    public class SuperAdminController : Controller
    {
        private readonly IApplicationDBContext _context;
        private readonly ILogger<SuperAdminController> _logger;
        public SuperAdminController(IApplicationDBContext context, ILogger<SuperAdminController> logger)
        {
            _context = context; _logger = logger;
        }
        public IActionResult Dashboard()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }
        public IActionResult Statistics()
        {
            var sd = new SavedStatsDashboard
            {
                Os = _context.CaptureDeviceData.GroupBy(s => s.OsName).Select(bn => new CaptureDeviceData { OsName = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                Browser = _context.CaptureDeviceData.GroupBy(s => s.BrowserName).Select(bn => new CaptureDeviceData { BrowserName = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                WifiMobile = _context.CaptureDeviceData.GroupBy(s => s.Ipv4 == "" ? "Mobile Data" : "Wifi").Select(bn => new CaptureDeviceData { PublicIp = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                DeviceName = _context.CaptureDeviceData.GroupBy(s => s.DeviceName).Select(bn => new CaptureDeviceData { DeviceName = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                BrandName = _context.CaptureDeviceData.Where(s => s.BrandName != "").GroupBy(s => s.BrandName).Select(bn => new CaptureDeviceData { BrandName = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                URole = _context.CaptureDeviceData.GroupBy(s => s.URole).Select(bn => new CaptureDeviceData { URole = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                PublicIp = _context.CaptureDeviceData.Where(s => s.PublicIp != "" && s.PublicIp != "::1").GroupBy(s => s.PublicIp).Select(bn => new CaptureDeviceData { PublicIp = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() })
            };
            //bn.Select(u2 => u2.userid.Distinct().Count()).ToString()
            return View(sd);
        }
        public IActionResult Restricted()
        {
            var rdb = new RestrictedAccessDashboard
            {
                RestrictedAccess = _context.RestrictedAccess
            };
            return View(rdb);
        }
        public IActionResult Settings()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return View();
        }

        public IActionResult ErrorLogs()
        {
            try
            {
                _context.Log.ToList();
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }
    }
}