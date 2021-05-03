using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bint.Services
{
    public interface IFileHelper
    {
        FileInfo GetFileInfo(string url);
        void Delete(FileInfo fileInfo);
        bool Exists(FileInfo fileInfo);
    }
}
