using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VeggieAPI.Models
{
    public class Routes
    {
        public IWebHostEnvironment hostEnvironment { get; set; }

        public string webRoot()
        {
            return hostEnvironment.WebRootPath;
        }

        public string setCDirectory()
        {
            return hostEnvironment.WebRootPath + "\\Cloud\\";
        }

        public string setRoute(string fileName)
        {
            return hostEnvironment.WebRootPath + "\\Cloud\\" + fileName;
        }

        public string getRoute(string fileName)
        {
            return hostEnvironment.WebRootPath + "\\Cloud\\" + fileName;
        }
    }
}
