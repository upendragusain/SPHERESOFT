using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chambers.API.Model
{
    public class FileUploadResult
    {
        public FileUploadResult(string url, string name)
        {
            this.Url = url;
            this.Name = name;
        }

        public string Url { get; }

        public string Name { get; }
    }
}
