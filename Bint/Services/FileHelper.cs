using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Bint.Services
{
    public class FileHelper:IFileHelper
    {
        public FileInfo GetFileInfo(string url)
        {
            return new FileInfo(url);
        }
        public void Delete(FileInfo fileInfo)
        {
            fileInfo.Delete();
        }
        public bool Exists(FileInfo fileInfo)
        {
            return fileInfo.Exists;
        }
        public string DocumentUploadPath(IFormFile formFile, string uploadFolder)
        {
            var z1 = Path.GetFileNameWithoutExtension(formFile.FileName) + "_" + DateTime.Now.ToString("yyyyMMddTHHmmssfff") + Path.GetExtension(formFile.FileName);//file extension
            return Path.Combine("wwwroot", uploadFolder, z1);
        }
    }
}
