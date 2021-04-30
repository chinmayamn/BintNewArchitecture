﻿using System;
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
            var sd = new SavedStatsDashboard
            {
                Os = _context._captureDeviceData.GroupBy(s => s.OsName).Select(bn => new CaptureDeviceData { OsName = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                Browser = _context._captureDeviceData.GroupBy(s => s.BrowserName).Select(bn => new CaptureDeviceData { BrowserName = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                WifiMobile = _context._captureDeviceData.GroupBy(s => s.Ipv4 == "" ? "Mobile Data" : "Wifi").Select(bn => new CaptureDeviceData { PublicIp = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                DeviceName = _context._captureDeviceData.GroupBy(s => s.DeviceName).Select(bn => new CaptureDeviceData { DeviceName = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                BrandName = _context._captureDeviceData.Where(s => s.BrandName != "").GroupBy(s => s.BrandName).Select(bn => new CaptureDeviceData { BrandName = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                URole = _context._captureDeviceData.GroupBy(s => s.URole).Select(bn => new CaptureDeviceData { URole = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() }),

                PublicIp = _context._captureDeviceData.Where(s => s.PublicIp != "" && s.PublicIp != "::1").GroupBy(s => s.PublicIp).Select(bn => new CaptureDeviceData { PublicIp = bn.Key, UserId = bn.Select(u2 => u2.UserId).Distinct().Count().ToString() })
            };
            //bn.Select(u2 => u2.userid.Distinct().Count()).ToString()
            return View(sd);
        }
        public IActionResult Restricted()
        {
            var rdb = new RestrictedAccessDashboard
            {
                RestrictedAccess = _context._restrictedAccess
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
                var el = _context.Log.ToList();
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