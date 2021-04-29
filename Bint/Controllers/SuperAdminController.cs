using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Bint.Models;
using Bint.Models.AccountViewModels;
using Bint.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using DeviceDetectorNET;
using DeviceDetectorNET.Parser;
using DeviceDetectorNET.Cache;
using Bint.Data;
using Microsoft.AspNetCore.Cors;

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
            SavedStatsDashboard sd = new SavedStatsDashboard();
            sd._OS = _context._captureDeviceData.GroupBy(s => s.OSName).Select(bn => new CaptureDeviceData { OSName = bn.Key, userid = bn.Select(u2 => u2.userid).Distinct().Count().ToString() });

            sd._Browser = _context._captureDeviceData.GroupBy(s => s.BrowserName).Select(bn => new CaptureDeviceData { BrowserName = bn.Key,userid = bn.Select(u2 => u2.userid).Distinct().Count().ToString() });

            sd._WifiMobile = _context._captureDeviceData.GroupBy(s => s.IPv4 == "" ? "Mobile Data" : "Wifi").Select(bn => new CaptureDeviceData { PublicIp = bn.Key, userid = bn.Select(u2 => u2.userid).Distinct().Count().ToString() });

            sd._DeviceName = _context._captureDeviceData.GroupBy(s => s.DeviceName).Select(bn => new CaptureDeviceData { DeviceName = bn.Key, userid = bn.Select(u2 => u2.userid).Distinct().Count().ToString() });

            sd._BrandName = _context._captureDeviceData.Where(s => s.BrandName != "").GroupBy(s =>  s.BrandName).Select(bn => new CaptureDeviceData { BrandName = bn.Key, userid = bn.Select(u2 => u2.userid).Distinct().Count().ToString() });

            sd._URole = _context._captureDeviceData.GroupBy(s => s.urole).Select(bn => new CaptureDeviceData { urole = bn.Key, userid = bn.Select(u2 => u2.userid).Distinct().Count().ToString() });

            sd._PublicIp = _context._captureDeviceData.Where(s => s.PublicIp != "" && s.PublicIp != "::1").GroupBy(s => s.PublicIp).Select(bn => new CaptureDeviceData { PublicIp = bn.Key, userid = bn.Select(u2 => u2.userid).Distinct().Count().ToString() });
            //bn.Select(u2 => u2.userid.Distinct().Count()).ToString()
            return View(sd);
        }
        public IActionResult Restricted()
        {
            RestrictedAccessDashboard rdb = new RestrictedAccessDashboard();
            rdb._restrictedaccess = _context._restrictedAccess;


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
                List<ErrorLog> el = new List<ErrorLog>();
                el = _context.Log.ToList();
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