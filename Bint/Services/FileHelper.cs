using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
