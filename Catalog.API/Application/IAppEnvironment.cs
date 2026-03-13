using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IAppEnvironment
    {
        public string ContentRootPath {  get; }

        public string WebRootPath { get; }
    }
}
