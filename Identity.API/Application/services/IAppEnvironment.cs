using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.services
{
    public interface IAppEnvironment
    {
        public string ContentRootPath { get; }
        public string WebRootPath { get; }
    }
}
